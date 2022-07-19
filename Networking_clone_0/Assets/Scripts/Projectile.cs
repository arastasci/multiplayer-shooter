using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject particleSystemPrefab;
    private void OnDestroy()
    {
        Instantiate(particleSystemPrefab, transform.position, Quaternion.identity);
    }
}
