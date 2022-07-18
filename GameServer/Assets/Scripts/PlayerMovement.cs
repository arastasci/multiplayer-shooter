using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public LayerMask whatIsGround;
    public float maxSlopeAngle = 45f;
    public Player player;

    
    private bool IsFloor(Vector3 normal)
    {
        float angle = Vector3.Angle(normal, Vector3.up);
        return angle < maxSlopeAngle;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & whatIsGround) == 0)
        {
            return;
        }
        for (int i = 0; i < collision.contactCount; i++)
        {

            Vector3 normal = collision.contacts[i].normal;
            if (IsFloor(normal))
            {
                player.isGrounded = true;
            }


        }
    }

}
