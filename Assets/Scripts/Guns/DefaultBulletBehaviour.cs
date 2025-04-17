using UnityEngine;

public class DefaultBulletBehaviour : Projectile
{
    private void Awake()
    {
        ProjectileSpeed = 5.0f;
        rb = GetComponent<Rigidbody2D>();
        BaseDamage = 1f;
        PierceCount = 0f;
        ParentGun = GameObject.Find("Player");
    }

    void Start()
    {
        rb.velocity = -transform.up * ProjectileSpeed;
    }

}
