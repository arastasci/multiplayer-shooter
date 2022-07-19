using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    bool isQuitting = false;
    [SerializeField] GameObject explodeParticlePrefab;
    private void OnApplicationQuit()
    {
        isQuitting = true;
    }
    private void OnDestroy()
    {
        if (!isQuitting)
        {
            Instantiate(explodeParticlePrefab, transform.position, Quaternion.identity);
        }
    }
}
