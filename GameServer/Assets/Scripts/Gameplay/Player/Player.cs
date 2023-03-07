using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;

    public Rigidbody rb;
    public Transform shootOrigin;


    
    
    public float moveMultiplier = 1f;
    public float jumpMultiplier = 1f;
    public int health;
    public int maxHealth;
    public float projectileForceMultiplier = 10f;
    
    private PlayerInput playerInput;

    private int killCount = 0;
    private int deathCount = 0;
    
    public float maxSpeedMultiplier;

    bool hasItem = false;
    public Quaternion weaponRotation;
    
    public Weapon[] weapons = new Weapon[2];
    [HideInInspector] public int activeWeaponID;
    public Vector3 lastExplodedPosition;
    
    public PlayerMovement playerMovement;
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
        if(rb.isKinematic) return;
        // if player falls out the map, kill
        if (transform.position.y < -8)
        {
            Die(id);
            return;
            
        }
        
        if (health <= 0f) return;
        
        

        playerMovement.Move(playerInput);
        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

   

    public void SetInput(PlayerInput input, Quaternion rotation, Quaternion weaponRotation)
    {
        playerInput = input;
        transform.rotation = rotation;
        this.weaponRotation = weaponRotation;

    }
    #region shoot
    /// <summary>
    /// Shoots a raycast with the pistol.
    /// </summary>
    /// <param name="direction"></param>
     
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
    /// <summary>
    /// Launcehs a projectile with the rocket launcher.
    /// </summary>
    /// <param name="direction"></param>
    public void LaunchProjectile(Vector3 direction)
    {
        ProjectileInfo projectileInfo = ProjectileManager.instance.InitializeProjectile(this, direction);

        ServerSend.ProjectileLaunched(this, projectileInfo.ID, projectileInfo.projectileShotFrom);
    }
    /// <summary>
    /// Fires the active gun when prompted.
    /// </summary>
    /// <param name="direction"></param>
    public void Fire(Vector3 direction)
    {
        if (!weapons[activeWeaponID].canShoot)
        {
            if (weapons[activeWeaponID].canReload && weapons[activeWeaponID].bulletLeftInMag == 0) Reload();
            return;
        }
        AudioManager.instance.PlayAudio(FXID.fire, FXEntity.player, id); // might make this function an event invoker
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
        

        // fills each weapon if they are empty - a temporary solution for when i wanted to
        // test the game w/ friends without the limitation of ammo
        foreach(Weapon w in weapons) 
        {
            if (w.bulletLeftTotal + w.bulletLeftInMag == 0)
            {
                w.Fill(id);
            }
        }

        
        
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

           Die(byPlayer);
        }

        ServerSend.PlayerHealth(this, byPlayer);
    }
    /// <summary>
    /// This player dies and server sends this info to every client.
    /// Several methods are invoked by this method both server and client side.
    /// Check PlayerHealth ClientHandle method for related issues.
    /// </summary>
    /// <param name="byPlayer"></param>
    private void Die(int byPlayer)
    {
        bool isSelfKill = false;
        transform.position = new Vector3(0, 100, 0);
        ServerSend.PlayerPosition(this);
        if (byPlayer != id)
        {
            Server.clients[byPlayer].player.IncrementKill();
        }
        else
        {
            ServerSend.SelfKill(id);
            isSelfKill = true;
        }
        IncrementDeath();

        ServerSend.UpdateScoreBoard();
        health = 0;
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative; // fix for weird bug
        moveMultiplier = 1f;
        jumpMultiplier = 1f;
        maxSpeedMultiplier = 1f;
        
        StartCoroutine(Respawn(isSelfKill));
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
        maxSpeedMultiplier = 1.75f;
        StartCoroutine(SpeedBoostEnded());
    }
    IEnumerator SpeedBoostEnded()
    {
        yield return new WaitForSeconds(5f);
        moveMultiplier = 1f;
        jumpMultiplier = 1f;
        maxSpeedMultiplier = 1f;
        hasItem = false;
    }
    private IEnumerator Respawn(bool isSelfKill)
    {
        yield return new WaitForSeconds(5f);
        rb.isKinematic = false;
        transform.position = (NetworkManager.instance.FindSpawnPosition()); ;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        ServerSend.PlayerPosition(this);
        health = maxHealth;
        foreach(Weapon weapon in weapons)
        {
            weapon.Fill(id);
            weapon.canShoot = true;
            
        }
        if(isSelfKill)
        ServerSend.SelfKill(id);
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
