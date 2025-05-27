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
        if (collision.CompareTag("Zombie"))
        {
            enemiesInCircle.Remove(collision.gameObject);
        }
    }

    private void TriggerEnterAndStayLogic(Collider2D collision)
    {
        if (collision.CompareTag("Zombie"))
        {
            Zombie zombie = collision.GetComponent<Zombie>();
            if (zombie != null && !zombie.IsColorFullyChanged)
            {
                if (zombie.GetComponent<SpriteRenderer>().enabled == false)
                {
                    zombie.ToggleSpriteRenderer();
                }
            }
        }
    }

    // New method to destroy all nearby enemies
    public void DestroyAllNearbyEnemies()
    {
        List<GameObject> enemiesToDestroy = new List<GameObject>(enemiesInCircle);

        foreach (GameObject enemy in enemiesToDestroy)
        {
            if (enemy != null)
            {
                Destroy(enemy);
                enemiesInCircle.Remove(enemy);
            }
        }
    }
}
