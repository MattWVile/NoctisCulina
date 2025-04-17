using UnityEngine;
using System.Collections.Generic;

public class PlayerLightCircleCollisionController : MonoBehaviour
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
        if (collision.CompareTag("Enemy"))
        {
            enemiesInCircle.Remove(collision.gameObject);
        }
    }
    private void TriggerEnterAndStayLogic(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.GetComponent<Zombie>().GetComponent<SpriteRenderer>().enabled == false)
            {
                collision.GetComponent<Zombie>().ToggleSpriteRenderer();
            }
        }
    }
}
