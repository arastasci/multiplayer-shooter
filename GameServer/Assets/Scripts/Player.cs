using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;

    public Rigidbody rb;
    public Transform shootOrigin;


    public bool isGrounded = true;
    public float gravity = -9.81f;
    public float moveSpeed = 5f;
    public float jumpspeed = 5f;
    float speedMultiplier = 1f;
    public int health;
    public int maxHealth;
    public float projectileForceMultiplier = 10f;
    [SerializeField] float groundDragForce = 0.1f;
    [SerializeField] float airDragForce = 0.05f;
    public float planarSpeed;
    private PlayerInput playerInput;

    private int killCount = 0;
    private int deathCount = 0;
    public float maxSpeed = 4f;
    bool hasItem = false;
    bool isMoving;
    bool isJumping;
    public Weapon[] weapons = new Weapon[2];
    [HideInInspector] public int activeWeaponID;

    public void Initialize(int id, string username)
    {
        this.id = id;
        this.username = username;
        health = maxHealth;
        playerInput = new PlayerInput();
        activeWeaponID = weapons[0].id;
     
        
    }

    public void FixedUpdate()
    {
        if (health <= 0f) return;

        Vector2 inputDir = new Vector2(playerInput.x, playerInput.z);

        Move(inputDir);
    }

    void CounterMovement()
    {

    }
    float GetMagnitude(float x, float z)
    {
        return Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(z, 2));
    }
    public bool debug;
    private void Move(Vector2 inputDirection)
    {
        rb.AddForce(Vector3.down * Time.deltaTime * 10);
        Vector3 moveDirection = Vector3.zero;
        isJumping = playerInput.isJumping;
        Vector3 velocity = rb.velocity;
        planarSpeed = GetMagnitude(velocity.x, velocity.z);
        if (inputDirection.magnitude == 0)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }
        if (!isMoving && planarSpeed > 0.5f)
        {
            if (isGrounded)
            {
                moveDirection = -rb.velocity.normalized * groundDragForce;
            }
            else
            {
                moveDirection = -rb.velocity.normalized * airDragForce;
            }
        }

        debug = planarSpeed > maxSpeed;
        if (!debug)
        {
            moveDirection = transform.right * inputDirection.x + transform.forward * inputDirection.y;
            moveDirection *= moveSpeed * speedMultiplier;
        }
        

        if (isGrounded && isJumping)
        {
            moveDirection += transform.up * jumpspeed * speedMultiplier;
            isGrounded = false;
        }
        rb.AddForce(moveDirection, ForceMode.VelocityChange);
        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);

    }

    public void SetInput(PlayerInput input, Quaternion rotation)
    {
        playerInput = input;
        transform.rotation = rotation;
    }
    #region shoot
    public void Shoot(Vector3 direction)
    {

        if (Physics.Raycast(shootOrigin.position, direction, out RaycastHit hitInfo, 25f))
        {
            Debug.DrawRay(shootOrigin.position, hitInfo.point, Color.yellow, 4f);

            if (hitInfo.collider.CompareTag("Player"))
            {
                hitInfo.collider.GetComponent<Player>().TakeDamage(35, id);
            }
        }

    }

    public void LaunchProjectile(Vector3 direction)
    {
        ProjectileInfo projectileInfo = ProjectileManager.instance.InitializeProjectile(this, direction);

        ServerSend.ProjectileLaunched(this, projectileInfo.ID, projectileInfo.projectileShotFrom);
    }
    
    public void Fire(Vector3 direction)
    {
        if (!weapons[activeWeaponID].canShoot) return;
        weapons[activeWeaponID].DecrementBullet(id);
        switch (activeWeaponID)
        {
            case 0:
                Shoot(direction);
                break;
            case 1:
                LaunchProjectile(direction);
                Debug.Log("player fired");
                break;
        }
        ServerSend.PlayerWeaponInfo(id, weapons[activeWeaponID]);
        
    }
    public void Reload()
    {
        weapons[activeWeaponID].Reload(id);

        ServerSend.PlayerReloading(id);
    }
    public void SetActiveWeapon(int weaponID)
    {
        activeWeaponID = weaponID;
        ServerSend.PlayerChangeWeapon(-1,id, weaponID);
        ServerSend.PlayerWeaponInfo(id, weapons[weaponID]);
    }
    #endregion

    #region get set killdeath

    public void IncrementKill()
    {
        killCount++;

    }
    public void IncrementDeath()
    {
        deathCount++;
        
    }
    public int GetKill() => killCount;
    public int GetDeath()
    {
        return deathCount;
    }
    #endregion

    #region handle health and speed
    public void TakeDamage(int damage,int byPlayer)
    {
        if(health <= 0)
        {
            
            return;
        }
        health -= damage;
        if(health <= 0f)
        {

            Server.clients[byPlayer].player.IncrementKill();
            IncrementDeath();
            ServerSend.UpdateScoreBoard();
            health = 0;
            rb.isKinematic = true;
            
            
            StartCoroutine(Respawn());
        }

        ServerSend.PlayerHealth(this, byPlayer);
    }

    void GetHealthPack()
    {
        health += 40;
        if (health > maxHealth) health = maxHealth;
        ServerSend.PlayerHealth(this, -1);
        
    }
    void GetSpeedBoost()
    {
        speedMultiplier = 1.75f;
        StartCoroutine(SpeedBoostEnded());
    }
    IEnumerator SpeedBoostEnded()
    {
        yield return new WaitForSeconds(5f);
        speedMultiplier = 1f;
        hasItem = false;
    }
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);
        transform.position = NetworkManager.instance.FindSpawnPosition();
        ServerSend.PlayerPosition(this);
        health = maxHealth;
        rb.isKinematic = false;
        ServerSend.PlayerRespawned(this);
    }
    


    public bool AttemptPickupItem(int itemType)
    {
        if (hasItem) return false;

        hasItem = true;
        switch (itemType)
        {
            case 0:
                GetHealthPack();
                hasItem = false;
                break;
            case 1:
                GetSpeedBoost();
                break;

        }
        return true;
    }
    #endregion

    public LayerMask whatIsGround;
    public float maxSlopeAngle = 45f;

    private bool IsFloor(Vector3 normal)
    {
        float angle = Vector3.Angle(normal, Vector3.up);
        return angle < maxSlopeAngle;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & whatIsGround) == 0 || isGrounded)
        {
            return;
        }
        for (int i = 0; i < collision.contactCount; i++)
        {

            Vector3 normal = collision.contacts[i].normal;
            if (IsFloor(normal))
            {
                isGrounded = true;
                break;
            }


        }
    }
}
