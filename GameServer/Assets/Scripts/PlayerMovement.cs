using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 lastNormalWJ;
    Collider lastCollider;

    [SerializeField]Player player;
    public LayerMask whatIsGround;
    public float maxSlopeAngle = 45f;
    ContactPoint[] contacts;
    [SerializeField] Rigidbody rb;
    [SerializeField] float trivialDifference = 3f;

    bool isAffectedBool;
    public static bool IsWallJumpable(Vector3 normal)
    {
        float angle = Vector3.Angle(normal, Vector3.up);
        
        return (angle <= 90f && angle > 0);
    }
    private bool IsFloor(Vector3 normal)
    {
        float angle = Vector3.Angle(normal, Vector3.up);
        return angle < maxSlopeAngle;
    }
    private bool IsDifferenceTrivial(Vector3 normal)
    {
        Debug.Log(normal);
        float angle = Vector3.Angle(normal, lastNormalWJ);
        return angle < trivialDifference;
    }
    IEnumerator SlideOff()
    {
        yield return new WaitForSeconds(1f);
        player.slidingOff = true;
    }
    

    private void OnCollisionStay(Collision col)
    {
        if (player.isGrounded)
        {
            player.isWallWalking = false;
            rb.useGravity = true;
            return;
        }
        contacts = new ContactPoint[col.contactCount];
        int points = col.GetContacts(contacts);

        for (int i = 0; i < points; i++)
        {
            ContactPoint cp = contacts[i];

            if (IsWallJumpable(cp.normal) && lastNormalWJ != cp.normal )
            {
                lastNormalWJ = cp.normal;
                rb.useGravity = false;
                lastCollider = col.collider;
                player.isWallWalking = true;
                player.wallNormal = cp.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                StartCoroutine(SlideOff());
                break;
            }
        }
        if (((1 << col.gameObject.layer) & whatIsGround) == 0)
        {
            return;
        }
        for (int i = 0; i < points; i++)
        {
            Vector3 normal = contacts[i].normal;
            if(player.lastExplodedPosition != null)
            {
                isAffectedBool =  1 > Vector3.Distance(player.lastExplodedPosition, contacts[i].point);
            }
            else
            {
                isAffectedBool = false;
            }
            if (IsFloor(normal) && !isAffectedBool)
            {
                Debug.Log(player.affectedByExplosion);
                player.affectedByExplosion = false;

                player.isGrounded = true;
                player.slidingOff = false;
                break;
            }
        }

    }
    private void OnCollisionExit(Collision collision)
    {
        if(lastCollider == collision.collider)
        {
            rb.useGravity = true;
            player.isWallWalking = false;
        }

    }

}
