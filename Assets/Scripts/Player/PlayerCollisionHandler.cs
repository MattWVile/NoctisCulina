using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    private PlayerHealthController healthController;

    private void Start()
    {
        healthController = PlayerHealthController.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Zombie zombie = collision.GetComponent<Zombie>();
            if (zombie != null)
            {
                // Use TakeDamage() to reduce player health and trigger related logic
                healthController.TakeDamage(zombie.Damage);

                // Destroy the enemy or handle its behavior
                zombie.Die();
            }
        }
    }
}
