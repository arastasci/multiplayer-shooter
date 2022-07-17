using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{

    public static Dictionary<int, Projectile> Projectiles = new Dictionary<int, Projectile>();
    public static ProjectileManager instance;
    public int nextProjectileID;
    public float projectileForceMultiplier = 10f;
    [SerializeField]
    GameObject projectilePrefab;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        nextProjectileID = 0;
    }
    public int InitializeProjectile(Player player, Vector3 direction)
    {
        int projectileID = nextProjectileID;
        nextProjectileID++;
        GameObject projectile = Instantiate(projectilePrefab, player.shootOrigin.position + player.shootOrigin.forward * 0.7f, Quaternion.identity); // dunno
        Projectile projectileP = projectile.GetComponent<Projectile>();
        projectileP.SetID(projectileID);
        Projectiles.Add(projectileID, projectileP);
        projectile.GetComponent<Rigidbody>().AddForce(direction * projectileForceMultiplier);
        return projectileID;
    }

    private void FixedUpdate()
    {
        ProjectileTransformInfo();
    }

    void ProjectileTransformInfo()
    {
        foreach(Projectile projectile in Projectiles.Values)
        {
            ServerSend.ProjectilePosition(projectile);
        }
    }
}
