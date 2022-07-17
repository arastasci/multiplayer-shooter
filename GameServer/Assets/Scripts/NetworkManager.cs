using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public static Dictionary<int, Projectile> Projectiles = new Dictionary<int, Projectile>();
    public int nextProjectileID;
    public float projectileForceMultiplier = 10f;
    [SerializeField]
    GameObject projectilePrefab;
    public GameObject playerPrefab;
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

    public Player InstantiatePlayer()
    {
        return Instantiate(playerPrefab, new Vector3(0,2f,0), Quaternion.identity).GetComponent<Player>();
    }
}
