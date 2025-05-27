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
        if (collision.CompareTag("Zombie"))
        {
            HandleEnemyCollision(collision.GetComponent<Zombie>());
        }
        else if (collision.CompareTag("Zomboss"))
        {
            HandleEnemyCollision(collision.GetComponent<Zomboss>());
        }
    }

    private void HandleEnemyCollision(Enemy enemy)
    {
        if (enemy != null)
        {
            healthController.TakeDamage(enemy.Damage);
            Destroy(enemy.gameObject);
        }
    }
}
