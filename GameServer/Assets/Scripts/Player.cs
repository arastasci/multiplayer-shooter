using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;

    public Rigidbody rb;
    public Transform shootOrigin;

    

    public bool isGrounded = true;
    public float gravity = -9.81f;
    public float moveSpeed = 5f;
    public float jumpspeed = 5f;
    public float health;
    public float maxHealth;

    public float projectileForceMultiplier = 10f;
   
    public int itemAmount;
    public int maxItemAmount;
    private bool[] inputs;

    public float maxVelocity = 4f;


    private void Start()
    {
        
    }

    public void Initialize(int id, string username)
    {
        this.id = id;
        this.username = username;
        health = maxHealth;
        inputs = new bool[5];
    }

    public void FixedUpdate()
    {
        if (health <= 0f) return;

        Vector2 inputDir = Vector2.zero;
        if (inputs[0])
        {
            inputDir.y += 1;
        }
        if (inputs[1])
        {
            inputDir.y -= 1;
        }
        if (inputs[2])
        {
            inputDir.x -= 1;
        }
        if (inputs[3])
        {
            inputDir.x += 1;
        }

        Move(inputDir);
    }
    
    
    private void Move(Vector2 inputDirection)
    {
        Vector3 moveDirection = Vector3.zero;

        Vector3 velocity = rb.velocity;
        float planarVelocity = Mathf.Sqrt(Mathf.Pow(velocity.x, 2) + Mathf.Pow(velocity.z, 2));
        if ( planarVelocity < maxVelocity)
        {
            moveDirection = transform.right * inputDirection.x + transform.forward * inputDirection.y;
            moveDirection *= moveSpeed;
        }

        if (isGrounded && inputs[4])
        {
            moveDirection += transform.up * moveSpeed;
            isGrounded = false;
        }
        rb.AddForce(moveDirection, ForceMode.VelocityChange);
        Debug.Log(moveDirection);
        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
        
    }

    public void SetInput(bool[] inputs, Quaternion rotation)
    {
        this.inputs = inputs;
        transform.rotation = rotation;
    }
    public void Shoot(Vector3 direction)
    {
        if(Physics.Raycast(shootOrigin.position,direction, out RaycastHit hitInfo,25f))
        {
            if (hitInfo.collider.CompareTag("Player"))
            {
                hitInfo.collider.GetComponent<Player>().TakeDamage(35f);
            }
        }
    }

    public void LaunchProjectile(Vector3 direction)
    {
        int projectileID = ProjectileManager.instance.InitializeProjectile(this,direction);
        ServerSend.ProjectileLaunched(this, projectileID);
    }
    public void TakeDamage(float damage)
    {
        if(health <= 0)
        {
            return;
        }
        health -= damage;
        if(health <= 0f)
        {
            health = 0f;
            rb.isKinematic = true;
            transform.position = new Vector3(0f, 25f, 0f);
            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }

        ServerSend.PlayerHealth(this);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);
        health = maxHealth;
        rb.isKinematic = false;
        ServerSend.PlayerRespawned(this);
    }
    
    public bool AttemptPickupItem()
    {
        if (itemAmount >= maxHealth) return false;

        itemAmount++;
        return true;
    }
    
    
}
