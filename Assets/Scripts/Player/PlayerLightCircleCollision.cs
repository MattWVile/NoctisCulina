using UnityEngine;
using System.Collections.Generic;

public class PlayerLightCircleCollision : MonoBehaviour
{
    public List<GameObject> enemiesInCircle = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie") || collision.CompareTag("Zomboss"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.IsInLightCircle = true;
                enemy.UpdateSpriteRendererState();
            }
            if (!enemiesInCircle.Contains(collision.gameObject))
            {
                enemiesInCircle.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie") || collision.CompareTag("Zomboss"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.IsInLightCircle = true;
                enemy.UpdateSpriteRendererState();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie") || collision.CompareTag("Zomboss"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.IsInLightCircle = false;
                enemy.UpdateSpriteRendererState();
            }
            enemiesInCircle.Remove(collision.gameObject);
        }
    }

    // Method to deal 20 damage to all nearby enemies
    public void DamageAllNearbyEnemies()
    {
        List<GameObject> enemiesToDamage = new List<GameObject>(enemiesInCircle);

        foreach (GameObject enemyObj in enemiesToDamage)
        {
            if (enemyObj != null)
            {
                Enemy enemyComponent = enemyObj.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    enemyComponent.TakeDamage(20f);
                }
                enemiesInCircle.Remove(enemyObj);
            }
        }
    }
}
