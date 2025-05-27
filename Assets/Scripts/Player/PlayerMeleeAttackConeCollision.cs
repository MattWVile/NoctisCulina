using UnityEngine;

public class PlayerMeleeAttackConeCollision : MonoBehaviour
{
    public int damageAmount = 11; // Amount of damage to deal

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie") || collision.CompareTag("Zomboss"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageAmount);
            }
        }
    }
}
