using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static Dictionary<int, ItemSpawner> spawners = new Dictionary<int, ItemSpawner>();
    private static int nextSpawnerID = 0;

    public int spawnerID;
    public bool hasItem = false;
    public Item itemType;
    
    public enum Item
    {
        health = 0,
        speed

    }
    void Start()
    {
        hasItem = false;
        spawnerID = nextSpawnerID;
        nextSpawnerID++;
        spawners.Add(spawnerID, this);
        StartCoroutine(SpawnItem());
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player.AttemptPickupItem((int)itemType))
            {
                ItemPickedUp(player.id);
            }
        }
    }
    private IEnumerator SpawnItem()
    {
        yield return new WaitForSeconds(10f);
        itemType = (Item)Random.Range(0, 2);
        hasItem = true;
        ServerSend.ItemSpawned(spawnerID, (int)itemType);
    }
    private void ItemPickedUp(int byPlayer) {
        hasItem = false;
        ServerSend.ItemPickedUp(spawnerID,byPlayer);
        StartCoroutine(SpawnItem());
    }
}
