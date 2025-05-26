using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerLightConeCollision : MonoBehaviour
{
    [SerializeField]
    private float timeToDeactivateSprite = .7f; // Default to 1 second

    private float colorChangeDuration = 1.5f; // Default to 1 second

    private Dictionary<Zombie, Coroutine> colorChangeCoroutines = new Dictionary<Zombie, Coroutine>();
    private Dictionary<Zombie, float> elapsedTimes = new Dictionary<Zombie, float>();

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
                SpriteRenderer spriteRenderer = zombie.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && !spriteRenderer.enabled)
                {
                    zombie.ToggleSpriteRenderer();
                }

                if (spriteRenderer != null && !colorChangeCoroutines.ContainsKey(zombie))
                {
                    // Reset elapsed time if the enemy is not already transitioning
                    if (!elapsedTimes.ContainsKey(zombie))
                    {
                        elapsedTimes[zombie] = 0f;
                    }

                    // Start the color change coroutine
                    Coroutine coroutine = ColorChangeUtility.ChangeColorOverTime(
                        this,
                        spriteRenderer,
                        spriteRenderer.color,
                        Color.yellow,
                        colorChangeDuration - elapsedTimes[zombie],
                        () =>
                        {
                            zombie.MarkColorAsFullyChanged();
                            if (!zombie.HasScoreBeenAwarded)
                            {
                                ScoreController.Instance.AddScore(300);
                                zombie.HasScoreBeenAwarded = true;
                            }
                        }
                    );
                    colorChangeCoroutines[zombie] = coroutine;
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
                SpriteRenderer spriteRenderer = zombie.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.enabled && !zombie.IsColorFullyChanged)
                {
                    zombie.ToggleSpriteRenderer(timeToDeactivateSprite);
                }

                // Check if the coroutine exists before stopping it
                if (colorChangeCoroutines.ContainsKey(zombie))
                {
                    Coroutine coroutine = colorChangeCoroutines[zombie];
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }
                    colorChangeCoroutines.Remove(zombie);
                }

                // Store the remaining time if the color is not fully changed
                if (!zombie.IsColorFullyChanged)
                {
                    if (elapsedTimes.ContainsKey(zombie))
                    {
                        elapsedTimes[zombie] = Mathf.Clamp01(elapsedTimes[zombie]); // Store the remaining time
                    }
                    else
                    {
                        elapsedTimes[zombie] = 0f;
                    }
                }
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
}
