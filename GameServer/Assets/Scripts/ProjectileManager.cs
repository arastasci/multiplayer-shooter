using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ProjectileInfo
{
    public ProjectileInfo(int id, Vector3 position)
    {
        ID = id;
        projectileShotFrom = position;
    }

    public int ID { get; }
    public Vector3 projectileShotFrom { get; }

}
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
    public ProjectileInfo InitializeProjectile(Player player, Vector3 direction)
    {
        int projectileID = nextProjectileID;
        nextProjectileID++;
        Vector3 position = player.shootOrigin.position;
        GameObject projectile = Instantiate(projectilePrefab, position, Quaternion.identity); // dunno
        Debug.DrawRay(player.shootOrigin.position,direction, Color.yellow, 4f);
        Projectile projectileP = projectile.GetComponent<Projectile>();
        projectileP.SetID(projectileID,player.id);
        Projectiles.Add(projectileID, projectileP);
        projectile.GetComponent<Rigidbody>().AddForce(direction * projectileForceMultiplier);
        return new ProjectileInfo(projectileID, position);
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
