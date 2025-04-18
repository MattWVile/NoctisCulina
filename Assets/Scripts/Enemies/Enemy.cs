using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour
{
    public float TotalHealth { get; protected set; }
    public float CurrentHealth { get; protected set; }
    public float Damage { get; protected set; }
    public float MaxSpeed { get; protected set; }
    public float CurrentSpeed { get; set; }

    protected virtual void Start()
    {
        CurrentHealth = TotalHealth;
    }

    public virtual void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        Debug.Log("Current Health: " + CurrentHealth);
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile"))
        {
            // Get the base damage from the projectile script attached to the projectile
            Projectile projectile = other.GetComponent<Projectile>();

            if (projectile != null)
            {
                float projectileBaseDamage = projectile.BaseDamage;
                float parentGunBaseDamage = projectile.ParentGun.GetComponent<Pistol>().BaseDamage;
                TakeDamage(parentGunBaseDamage + projectileBaseDamage);
                projectile.HandleCollisionWithEnemy(transform.gameObject.GetComponent<Collider2D>());
                if (ScoreController.Instance != null)
                {
                    ScoreController.Instance.AddScore(100);
                }
                else
                {
                    Debug.LogError("ScoreController instance not found.");
                }
            }
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }

    public virtual void ToggleSpriteRenderer(float timeDelay = 0f)
    {
        StartCoroutine(ToggleSpriteRendererCoroutine(timeDelay));
    }

    private IEnumerator ToggleSpriteRendererCoroutine(float timeDelay)
    {
        // Wait for the specified time delay
        yield return new WaitForSeconds(timeDelay);

        // Toggle the sprite renderer
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
        }
    }
}
