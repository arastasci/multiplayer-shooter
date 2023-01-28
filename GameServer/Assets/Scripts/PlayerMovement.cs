using System.Collections;
using System;
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
    public bool isGrounded = true;
    bool isAffectedBool;
    bool isMoving;
    bool isJumping;
    public bool isWallWalking;
    public bool slidingOff = false;
    Vector3 moveDirection;
    [SerializeField] float counterMovement = 0.3f;
    [SerializeField] float slideCounterMovement = 0.1f;
    [SerializeField] float minSpeed = 0.3f;
    public float moveSpeed = 5f;
    public float jumpspeed = 5f;

    public float wallJumpMultiplier = 1f;
    public float wallJumpSpeed = 5f;
    float moveMultiplier = 1f;
    float jumpMultiplier = 1f;
    public float maxSpeedConstant = 7f;
    [SerializeField] float threshold = 1f;
    public float planarSpeed;

    public Vector3 wallNormal;
    float maxSpeed { get => maxSpeedConstant * player.maxSpeedMultiplier; set => maxSpeed = player.maxSpeedMultiplier * maxSpeedConstant; }

    PlayerInput playerInput;

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
 
    IEnumerator SlideOff()
    {
        yield return new WaitForSeconds(1f);
        slidingOff = true;
    }

    
    float GetMagnitude(float x, float z)
    {
        return Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(z, 2));
    }
    public bool debug;
    void Crouch()
    {
        transform.localScale = new Vector3(1, 0.5f, 1f);
    }
    void StopCrouch()
    {
        transform.localScale = new Vector3(1, 1, 1);

    }

    public void Move(PlayerInput input)
    {
        

        //---------------------------------------
        playerInput = input;
        if (playerInput.isCrouching)
        {
            Crouch();
        }
        else
        {
            StopCrouch();
        }
        Vector2 inputDirection = new Vector2(playerInput.x, playerInput.z);


        rb.AddForce(Vector3.down * Time.fixedDeltaTime * 15);

        Vector2 mag = FindVelRelativeToLook();

        float xMag = mag.x, yMag = mag.y;
        planarSpeed = GetMagnitude(xMag, yMag);
        CounterMovement(inputDirection.x, inputDirection.y, mag);

        if (playerInput.isJumping) 
        {
            if (isGrounded){

                Jump();
            }
            else if (isWallWalking)
            {
                WallJump();
            }
        } 
        


        if (playerInput.isCrouching && isGrounded)
        {
            rb.AddForce(Vector3.down * Time.fixedDeltaTime * 3000);
            return;
        }

        if (playerInput.x > 0 && xMag > maxSpeed) playerInput.x = 0;
        if (playerInput.x < 0 && xMag < -maxSpeed) playerInput.x = 0;
        if (playerInput.z > 0 && yMag > maxSpeed) playerInput.z = 0;
        if (playerInput.z < 0 && yMag < -maxSpeed) playerInput.z = 0;
        

        float multiplier = 1f, multiplierV = 1f;

        //if (!isGrounded && !isWallWalking)
        //{
        //    multiplier = 0.5f;
        //    multiplierV = 0.5f;
        //}

        if (isGrounded && playerInput.isCrouching) multiplierV = 0f;
        rb.AddForce(transform.forward * playerInput.z * moveSpeed * Time.fixedDeltaTime * multiplier * multiplierV, ForceMode.VelocityChange);
        rb.AddForce(transform.right * playerInput.x * moveSpeed * Time.fixedDeltaTime * multiplier, ForceMode.VelocityChange);
    }
    void Jump()
    {
        if(Physics.Raycast(player.shootOrigin.position,transform.forward,out RaycastHit hit, 5f))
        {
            if (IsWallJumpable(hit.normal))
            {
                rb.AddForce((transform.forward + Vector3.up * 4).normalized * wallJumpMultiplier * wallJumpSpeed, ForceMode.VelocityChange);
                Debug.Log("walljump");
            }
        }
        else
        {
            rb.AddForce(Vector3.up * jumpspeed * jumpMultiplier, ForceMode.VelocityChange);
            Debug.Log("normal jump" + jumpMultiplier + " " + jumpspeed);

        }
        
        isGrounded = false;
    }
    void WallJump()
    {
        rb.AddForce(( transform.forward + wallNormal + Vector3.up ).normalized * jumpMultiplier * jumpspeed, ForceMode.VelocityChange);
        
        AudioManager.instance.PlayAudio(FXID.wallJump, FXEntity.player, player.id);
        
    }

    Vector2 FindVelRelativeToLook()
    {
        float lookAngle = transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;
        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitude = rb.velocity.magnitude;
        float yMag = magnitude * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitude * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }
    void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!isGrounded || isJumping) return;
        if (planarSpeed < minSpeed)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            return;
        }
        // slow down sliding
        if (playerInput.isCrouching)
        {

            rb.AddForce(  moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement, ForceMode.VelocityChange);
          
            return;
        }
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * transform.right * Time.deltaTime * -mag.x * counterMovement, ForceMode.VelocityChange);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * transform.forward * Time.deltaTime * -mag.y * counterMovement, ForceMode.VelocityChange);
        }
        Vector3 velocity = rb.velocity;
        if (GetMagnitude(velocity.x,velocity.z) > maxSpeed)
        {
            float fallSpeed = velocity.y;
            Vector3 n = velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallSpeed, n.z);
        }
    }

    private void OnCollisionStay(Collision col)
    {
        if (isGrounded)
        {
            isWallWalking = false;
            rb.useGravity = true;
            return;
        }
        contacts = new ContactPoint[col.contactCount];
        int points = col.GetContacts(contacts);

        
        if (((1 << col.gameObject.layer) & whatIsGround) == 0)
        {
            for (int i = 0; i < points; i++)
            {
                ContactPoint cp = contacts[i];

                if (IsWallJumpable(cp.normal) && lastNormalWJ != cp.normal)
                {
                    lastNormalWJ = cp.normal;
                    rb.useGravity = false;
                    player.affectedByExplosion = false;
                    lastCollider = col.collider;
                    isWallWalking = true;
                    wallNormal = cp.normal;
                    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                    StartCoroutine(SlideOff());
                    break;
                }
            }
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
                player.affectedByExplosion = false;

                isGrounded = true;
                slidingOff = false;
                break;
            }
        }

    }
    private void OnCollisionExit(Collision collision)
    {
        if (lastCollider == collision.collider)
        {
            rb.useGravity = true;
            isWallWalking = false;
        }

    }

}
