using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static Dictionary<int, ItemSpawner> spawners = new Dictionary<int, ItemSpawner>();
    private static int nextSpawnerID = 1;

    public int spawnerID;
    public bool hasItem = false;
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
            if (player.AttemptPickupItem())
            {
                ItemPickedUp(player.id);
            }
        }
    }
    private IEnumerator SpawnItem()
    {
        yield return new WaitForSeconds(10f);
        hasItem = true;
    }
    private void ItemPickedUp(int byPlayer) {
        hasItem = false;
        StartCoroutine(SpawnItem());
    }
}
