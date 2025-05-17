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
                // Reduce player health
                healthController.playerCurrentHealth -= zombie.Damage;

                if (healthController.playerCurrentHealth <= 0)
                {
                    healthController.playerCurrentHealth = 0;
                    healthController.RemoveHeart();
                    // Handle game over logic here
                }
                else
                {
                    healthController.RemoveHeart();
                }

                // Destroy the enemy or handle its behavior
                zombie.Die();
            }
        }
    }
}
