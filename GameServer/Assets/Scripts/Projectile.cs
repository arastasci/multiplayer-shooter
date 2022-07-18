using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float explosionForce = 20f;
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
    }

    private void OnCollisionEnter(Collision collision)
    {

        ServerSend.ProjectileExploded(id);
        
        rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider collider in colliders)
        {
            if (collider.TryGetComponent<Player>(out Player player))
            {
                Rigidbody rigidbodyCollider = collider.GetComponent<Rigidbody>();
                Vector3 vector = rigidbodyCollider.worldCenterOfMass - transform.position;
                float distance = Vector3.Distance(Vector3.zero, vector);
                float damageTaken = damageMultiplier * (1 - (distance / explosionRadius));
                rigidbodyCollider.AddForce(Vector3.Normalize(vector) * damageTaken * forceMultiplier, ForceMode.Acceleration);
                if(playerID != player.id) player.TakeDamage(damageTaken);

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

        ProjectileManager.Projectiles.Remove(id);
        Destroy(gameObject);

    }
}
