using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float ForceThreshold = 20f;
    [SerializeField] float explosionRadius = 3f;
    [SerializeField] float damageMultiplier = 50f;
    [SerializeField] float forceMultiplier = 10f;
    public int id;
    public int playerID;
    Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(DestroyThis());
    }
    public void SetID(int id, int playerID)
    {
       
        this.id = id;
        this.playerID = playerID;
        Physics.IgnoreCollision(Server.clients[playerID].player.GetComponent<Collider>(), GetComponent<Collider>());

    }

    private void OnCollisionEnter(Collision collision)
    {
        // TODO: Implement projectile colliding with player not affecting the player collided
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider collider in colliders)
        {
            if (collider.TryGetComponent<Player>(out Player player))
            {
                Rigidbody rigidbodyCollider = collider.GetComponent<Rigidbody>();
                Vector3 vector = rigidbodyCollider.transform.position - transform.position;
                float distance = vector.magnitude;
                int damageTaken = (int)(damageMultiplier * (1 - (distance / explosionRadius)));
                player.affectedByExplosion = true;
                if (damageTaken > ForceThreshold)
                {
                    rigidbodyCollider.AddForce(Vector3.Normalize(vector) * damageTaken * forceMultiplier, ForceMode.Acceleration);
                }
                if(playerID != player.id)  player.TakeDamage(damageTaken, playerID);

            }
        }    

        DestroyProjectile();
    }

    IEnumerator DestroyThis()
    {
        yield return new WaitForSeconds(10f);
        DestroyProjectile();
    }
    void DestroyProjectile()
    {
        ServerSend.ProjectileExploded(id);
        ProjectileManager.Projectiles.Remove(id);
        Destroy(gameObject);

    }
}
