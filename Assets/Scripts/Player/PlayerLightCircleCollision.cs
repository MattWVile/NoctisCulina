using UnityEngine;
using System.Collections.Generic;

public class PlayerLightCircleCollision : MonoBehaviour
{
    public List<GameObject> enemiesInCircle;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TriggerEnterAndStayLogic(collision);
        enemiesInCircle.Add(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TriggerEnterAndStayLogic(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie") || collision.CompareTag("Zomboss"))
        {
            enemiesInCircle.Remove(collision.gameObject);
        }
    }

    private void TriggerEnterAndStayLogic(Collider2D collision)
    {
        if (collision.CompareTag("Zombie") || collision.CompareTag("Zomboss"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsColorFullyChanged)
            {
                SpriteRenderer spriteRenderer = enemy.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && !spriteRenderer.enabled)
                {
                    enemy.ToggleSpriteRenderer();
                }
            }
        }
    }

    // Method to destroy all nearby enemies
    public void DestroyAllNearbyEnemies()
    {
        List<GameObject> enemiesToDestroy = new List<GameObject>(enemiesInCircle);

        foreach (GameObject enemy in enemiesToDestroy)
        {
            if (enemy != null)
            {
                Enemy enemyComponent = enemy.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    enemyComponent.Die(); // Use the Enemy class's Die method to handle destruction and scoring
                }
                enemiesInCircle.Remove(enemy);
            }
        }
    }
}
