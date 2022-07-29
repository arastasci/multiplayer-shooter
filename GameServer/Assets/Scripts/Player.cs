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
    float moveMultiplier = 1f;
    float jumpMultiplier = 1f;
    public int health;
    public int maxHealth;
    public float projectileForceMultiplier = 10f;
    [SerializeField] float groundDragForce = 0.1f;
    [SerializeField] float airDragForce = 3f;
    [SerializeField] float crouchDragForce = 1f;
    [SerializeField] float wallDashForce = 1f;
    public float planarSpeed;
    private PlayerInput playerInput;

    private int killCount = 0;
    private int deathCount = 0;
    public float maxSpeed = 4f;
    bool hasItem = false;
    bool isMoving;
    bool isJumping;
    bool isCrouching;
    public bool isWallWalking;
    public bool slidingOff = false;
    public Weapon[] weapons = new Weapon[2];
    [HideInInspector] public int activeWeaponID;

    public Vector3 wallNormal;
    Vector3 moveDirection;

    public bool affectedByExplosion = false;
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
        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    void CounterMovement(Vector3 velocity, float air, float ground)
    {
        if (isGrounded)
        {
            moveDirection = -velocity.normalized * ground;
            Debug.Log("ground force");

        }
        else
        {
            Debug.Log("air force");
            moveDirection = -velocity.normalized * air;
        }
        if (moveDirection.magnitude > velocity.magnitude)
        {
            moveDirection = Vector3.zero;
            rb.velocity = Vector3.zero;
        }
    }
    float GetMagnitude(float x, float z)
    {
        return Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(z, 2));
    }
    public bool debug;
    void Crouch()
    {
        isCrouching = true;
        transform.localScale = new Vector3(1,0.5f,1f);
        CounterMovement(rb.velocity,crouchDragForce,airDragForce);
    }
    void StopCrouch()
    {
        isCrouching = false;
        transform.localScale = new Vector3(1, 1, 1);

    }
    private void Move(Vector2 inputDirection)
    {
        if (slidingOff)
        {
            rb.AddForce(Vector3.down * Time.deltaTime * 10);
        }
        if (playerInput.isCrouching)
        {
            Crouch();
            ServerSend.PlayerCrouch(this, true);
            return;
        }
        else if(!playerInput.isCrouching && isCrouching)
        {
            StopCrouch();
            ServerSend.PlayerCrouch(this, false);
        }

        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        moveDirection = Vector3.zero;
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
            AudioManager.instance.PlayAudio(FXID.walk, FXEntity.player, id);
        }
        if (!isMoving && planarSpeed > 0f)
        {
            CounterMovement(velocity,airDragForce,groundDragForce);
        }

        debug = planarSpeed > maxSpeed;
        if (!debug)
        {
            moveDirection = transform.right * inputDirection.x + transform.forward * inputDirection.y;
            moveDirection *= moveSpeed * moveMultiplier;
        }
        //else if(!affectedByExplosion)
        //{
        //    rb.velocity = new Vector3(velocity.normalized.x * maxSpeed, velocity.y, velocity.normalized.z * maxSpeed);
        //}
        

        if (isGrounded && isJumping && !isWallWalking)
        {
            if (Physics.Raycast(shootOrigin.position, transform.forward,out RaycastHit hit,8f))
            {
                if (PlayerMovement.IsWallJumpable(hit.normal))
                {
                    Debug.Log("wall jumpable");
                    moveDirection += wallDashForce * transform.forward + jumpspeed * 1.5f * transform.up;
                }
            }
            else
            {
                moveDirection += jumpMultiplier * jumpspeed * transform.up;
            }
            isGrounded = false;
            AudioManager.instance.PlayAudio(FXID.jump, FXEntity.player, id);
        }
        if (isWallWalking && isJumping)
        {
            moveDirection += (wallNormal + Vector3.Cross(Vector3.up,wallNormal) +Vector3.up).normalized * jumpMultiplier * jumpspeed;
            AudioManager.instance.PlayAudio(FXID.wallJump, FXEntity.player, id);
        }
        rb.AddForce(moveDirection, ForceMode.VelocityChange);
        
        
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
        if (!weapons[activeWeaponID].canShoot)
        {
            if (weapons[activeWeaponID].canReload && weapons[activeWeaponID].bulletLeftInMag == 0) Reload();
            return;
        }
        AudioManager.instance.PlayAudio(FXID.fire, FXEntity.player, id);
        weapons[activeWeaponID].DecrementBullet(id);
        switch (activeWeaponID)
        {
            case 0:
                Shoot(direction);
                break;
            case 1:
                LaunchProjectile(direction);
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
        moveMultiplier = 1.75f;
        jumpMultiplier = 1.15f;
        StartCoroutine(SpeedBoostEnded());
    }
    IEnumerator SpeedBoostEnded()
    {
        yield return new WaitForSeconds(5f);
        moveMultiplier = 1f;
        jumpMultiplier = 1f;
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

    
    
    

}
