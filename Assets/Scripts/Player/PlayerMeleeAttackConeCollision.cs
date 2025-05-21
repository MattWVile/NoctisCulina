using UnityEngine;

public class PlayerMeleeAttackConeCollision : MonoBehaviour
{
    public int damageAmount = 11; // Amount of damage to deal

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageAmount); // Apply damage
                AddScore(250);
            }
        }
    }

    private void AddScore(int score)
    {
        if (ScoreController.Instance != null)
        {
            ScoreController.Instance.AddScore(score);
        }
        else
        {
            Debug.LogError("ScoreController instance not found.");
        }
    }
}
