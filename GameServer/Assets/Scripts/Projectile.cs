using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float explosionForce = 20f;
    [SerializeField] float explosionRadius = 3f;
    [SerializeField] float damageMultiplier = 50f;
    public int id;

    public void SetID(int id)
    {
        this.id = id;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collidedObject = collision.gameObject;
        ServerSend.ProjectileExploded(transform.position);
        collidedObject.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius);
        if(collidedObject.TryGetComponent<Player>(out Player player))
        {
            float distance = Vector3.Distance(transform.position, collidedObject.transform.position);
            float damageTaken = damageMultiplier * 1 - (distance / explosionRadius);
            player.TakeDamage(damageTaken);
        }

        Invoke(nameof(DestroyProjectile), 2f);
    }
    void DestroyProjectile()
    {

        ProjectileManager.Projectiles.Remove(id);
        Destroy(gameObject);

    }
}
