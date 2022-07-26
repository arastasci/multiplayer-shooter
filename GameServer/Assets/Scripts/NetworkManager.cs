using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public static Dictionary<int, Projectile> Projectiles = new Dictionary<int, Projectile>();
    public int nextProjectileID;
    public float projectileForceMultiplier = 10f;
    public GameObject playerPrefab;

    public Transform[] spawnAreas;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
       else if( instance != this)
        {
            Debug.Log("instance already exists, destroying object.");
            Destroy(this);
        }
        
    }
    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 50;

        Server.Start(50,26950);
    }
    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Vector3 FindSpawnPosition()
    {
        Transform playerTransform = spawnAreas[(int)Random.Range(0, spawnAreas.Length)].transform;
        Vector3 scale = playerTransform.localScale * 10;
        float x = Random.Range(-scale.x / 2, scale.x / 2);
        float z = Random.Range(-scale.z / 2, scale.z / 2);
        Vector3 localSpawnPoint = new Vector3(x, 2f, z);
        Vector3 globalSpawnPoint = localSpawnPoint + playerTransform.position;
        return globalSpawnPoint;
    }
    public Player InstantiatePlayer()
    {
        
        return Instantiate(playerPrefab, FindSpawnPosition() , Quaternion.identity).GetComponent<Player>();
    }
}
