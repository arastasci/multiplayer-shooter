using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, ProjectileManager> projectiles = new Dictionary<int, ProjectileManager>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;
    public GameObject projectilePrefab;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists.");
            Destroy(this);
        }
    }
    private void Start()
    {
        Debug.Log(projectiles.ToString());
    }
    public void SpawnPlayer(int _id, string username, Vector3 position, Quaternion rotation)
    {
        GameObject player;
        if(_id == Client.instance.myId)
        {
            player = Instantiate(localPlayerPrefab,position,rotation);
        }
        else
        {
            player = Instantiate(playerPrefab,position,rotation);
        }
        player.GetComponent<PlayerManager>().Initialize(_id,username);
        players.Add(_id, player.GetComponent<PlayerManager>());
    }

    public void SpawnProjectile(int projectileID, Vector3 position)
    {
        ProjectileManager projectile = Instantiate(projectilePrefab, position, Quaternion.identity).GetComponent<ProjectileManager>();
        projectiles.Add(projectileID, projectile);
    }
    public static void ProjectileExplode(int id)
    {
        Debug.Log("exploded");
        GameObject projectile = projectiles[id].gameObject;
        projectiles.Remove(id);
        Destroy(projectile);
        // play animation

    }
    public void UpdateProjectilePosition(int id, Vector3 position)
    {
        projectiles[id].transform.position = position;
    }
}
