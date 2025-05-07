using UnityEngine;

public class PlayerAttackConeCollision : MonoBehaviour
{
    public int damageAmount = 10; // Amount of damage to deal

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TriggerEnterAndStayLogic(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TriggerEnterAndStayLogic(collision);
    }

    private void TriggerEnterAndStayLogic(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageAmount); // Apply damage
            }
        }
    }
}
