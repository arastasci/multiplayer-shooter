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
    public int health;
    public int maxHealth;

    public float projectileForceMultiplier = 10f;

    public int itemAmount;
    public int maxItemAmount;
    private bool[] inputs;

    private int killCount = 0;
    private int deathCount = 0;
    public float maxVelocity = 4f;

    public Weapon[] weapons = new Weapon[2];

    int activeWeaponID;

    public void Initialize(int id, string username)
    {
        this.id = id;
        this.username = username;
        health = maxHealth;
        inputs = new bool[5];
        activeWeaponID = weapons[0].id;
        ServerSend.PlayerWeaponInfo(id, weapons[activeWeaponID]);
    }

    public void FixedUpdate()
    {
        if (health <= 0f) return;

        Vector2 inputDir = Vector2.zero;
        if (inputs[0])
        {
            inputDir.y += 1;
        }
        if (inputs[1])
        {
            inputDir.y -= 1;
        }
        if (inputs[2])
        {
            inputDir.x -= 1;
        }
        if (inputs[3])
        {
            inputDir.x += 1;
        }

        Move(inputDir);
    }


    private void Move(Vector2 inputDirection)
    {
        Vector3 moveDirection = Vector3.zero;

        Vector3 velocity = rb.velocity;
        float planarVelocity = Mathf.Sqrt(Mathf.Pow(velocity.x, 2) + Mathf.Pow(velocity.z, 2));
        if (planarVelocity < maxVelocity)
        {
            moveDirection = transform.right * inputDirection.x + transform.forward * inputDirection.y;
            moveDirection *= moveSpeed;
        }

        if (isGrounded && inputs[4])
        {
            moveDirection += transform.up * jumpspeed;
            isGrounded = false;
        }
        rb.AddForce(moveDirection, ForceMode.VelocityChange);
        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);

    }

    public void SetInput(bool[] inputs, Quaternion rotation)
    {
        this.inputs = inputs;
        transform.rotation = rotation;
    }
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
        ServerSend.PlayerChangeWeapon(id, weaponID);
        ServerSend.PlayerWeaponInfo(id, weapons[weaponID]);
    }

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
            health = 0;
            rb.isKinematic = true;
            transform.position = new Vector3(0f, 25f, 0f);
            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }

        ServerSend.PlayerHealth(this);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);
        health = maxHealth;
        rb.isKinematic = false;
        ServerSend.PlayerRespawned(this);
    }
    
    public bool AttemptPickupItem()
    {
        if (itemAmount >= maxHealth) return false;

        itemAmount++;
        return true;
    }
    
    
}
