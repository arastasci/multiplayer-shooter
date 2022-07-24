using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public int spawnerID;
    public bool hasItem;
    public GameObject[] itemPrefabs;
    public float itemRotationSpeed = 50f;
    public float itemBobSpeed = 2f;
    private Vector3 basePosition;
    int currentItemType;

    private void Update()
    {
        if (hasItem)
        {
            Transform itemTransform = transform.GetChild(currentItemType);
            itemTransform.Rotate(Vector3.up, itemRotationSpeed * Time.deltaTime, Space.World);
            itemTransform.position = basePosition + new Vector3(0,0.25f * Mathf.Sin(Time.time * itemBobSpeed),0);
        }
    }
    public void Initialize(int spawnerID,int itemType, bool hasItem)
    {
        this.spawnerID = spawnerID;
        this.hasItem = hasItem;
        itemPrefabs[itemType].SetActive(hasItem);
        basePosition = transform.position;
    }

    public void ItemPickedUp()
    {
        hasItem = false;
        itemPrefabs[currentItemType].SetActive(false);
    }
    public void ItemSpawned(int itemType)
    {
        hasItem = true;
        itemPrefabs[itemType].SetActive(true);
        currentItemType = itemType;
    }
}
