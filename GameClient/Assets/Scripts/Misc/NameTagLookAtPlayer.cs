using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class is used to only display nametags when the player carrying it is visible to the local player.
/// </summary>
public class NameTagLookAtPlayer : MonoBehaviour
{
    public static Transform localPlayer;
    private static CapsuleCollider collider;
    private MeshRenderer renderer;
    private void Start()
    {
        localPlayer = GameManager.players[Client.instance.myId].transform;
        if (collider == null)
        {
            collider = localPlayer.GetChild(0).GetComponent<CapsuleCollider>();
        }
        renderer = GetComponent<MeshRenderer>();
    }

    private void FixedUpdate()
    {
        Vector3 dir = localPlayer.position - transform.position;
        CastRay(dir); // very expensive call, might ditch it or have to make a more affordable solution.
       // Debug.Log(collider.name);
        transform.forward = new Vector3(-dir.x, 0, -dir.z);
    }

    private void CastRay(Vector3 direction)
    {
        Ray ray = new Ray(transform.position, direction);
        Physics.Raycast(ray, out RaycastHit hitInfo, 100f);
        if (!hitInfo.collider || hitInfo.collider != collider)
        {
            renderer.enabled = false;
            Debug.Log("didnt hit");
        }
        else renderer.enabled = true;
    }
}
