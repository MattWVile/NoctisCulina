using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public float BaseDamage { get; protected set; }
    public float ProjectileSpeed { get; protected set; }
    public float PierceCount { get; protected set; }
    public GameObject ParentGun { get; set; }
    public Rigidbody2D rb { get; protected set; }
    void OnBecameInvisible()
    {
        // Destroy the bullet when it goes out of camera bounds
        Destroy(gameObject);
    }

    public void HandleCollisionWithEnemy(Collider2D other)
    {
        if(other == this) return;
        if(PierceCount > 0)
        {
            PierceCount--;
            return;
        }
        Destroy(gameObject);
    }
}
