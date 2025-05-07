using UnityEngine;
using System.Collections;

public class PlayerLightConeCollision : MonoBehaviour
{
    private Coroutine colorChangeCoroutine;
    private float elapsedTime;

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
            Zombie zombie = collision.GetComponent<Zombie>();
            if (zombie != null)
            {
                SetEnemySpeedToZero(zombie);
                if (!zombie.GetComponent<SpriteRenderer>().enabled)
                {
                    zombie.ToggleSpriteRenderer();
                }
                if (colorChangeCoroutine == null && zombie.GetComponent<SpriteRenderer>().color != Color.yellow)
                {
                    colorChangeCoroutine = StartCoroutine(ChangeColorOverTime(zombie.GetComponent<SpriteRenderer>(), zombie.GetComponent<SpriteRenderer>().color, Color.yellow, 1f - elapsedTime));
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Zombie zombie = collision.GetComponent<Zombie>();
            if (zombie != null)
            {
                zombie.CurrentSpeed = zombie.MaxSpeed; // Reset speed when exiting the light cone
                if (zombie.GetComponent<SpriteRenderer>().enabled)
                {
                    zombie.ToggleSpriteRenderer(1f);
                }
                if (colorChangeCoroutine != null)
                {
                    StopCoroutine(colorChangeCoroutine);
                    colorChangeCoroutine = null;
                }
                elapsedTime = 1f - (1f - elapsedTime); // Store the remaining time
            }
        }
    }

    private void SetEnemySpeedToZero(Enemy enemy)
    {
        if (enemy.CurrentSpeed == 0f)
        {
            return; // Speed is already zero, no need to set it again
        }
        if (enemy != null)
        {
            enemy.CurrentSpeed = 0f; // Set speed to zero
        }
    }

    private IEnumerator ChangeColorOverTime(SpriteRenderer spriteRenderer, Color startColor, Color endColor, float duration)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
        spriteRenderer.color = endColor; // Ensure the final color is set
        colorChangeCoroutine = null; // Reset the coroutine reference
        ScoreController.Instance.AddScore(300); // Add score when the color change is complete
    }
}
