using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerLightConeCollision : MonoBehaviour
{
    [SerializeField]
    private float timeToDeactivateSprite = 0.7f; // Time to disable sprite after exiting light cone

    private float zombieColorChangeDuration = 1.5f; // Time for zombies to fully change color
    private float zombossColorChangeDuration = 10f; // Time for Zomboss to fully change color (much slower)

    private Dictionary<Zombie, Coroutine> zombieColorChangeCoroutines = new Dictionary<Zombie, Coroutine>();
    private Dictionary<Zombie, float> zombieElapsedTimes = new Dictionary<Zombie, float>();

    private Dictionary<Zomboss, Coroutine> zombossColorChangeCoroutines = new Dictionary<Zomboss, Coroutine>();
    private Dictionary<Zomboss, float> zombossElapsedTimes = new Dictionary<Zomboss, float>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleEnemyLogic(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        HandleEnemyLogic(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie"))
        {
            Zombie zombie = collision.GetComponent<Zombie>();
            if (zombie != null)
            {
                HandleZombieExit(zombie);
            }
        }
        else if (collision.CompareTag("Zomboss"))
        {
            Zomboss zomboss = collision.GetComponent<Zomboss>();
            if (zomboss != null)
            {
                HandleZombossExit(zomboss);
            }
        }
    }

    private void HandleEnemyLogic(Collider2D collision)
    {
        if (collision.CompareTag("Zombie"))
        {
            Zombie zombie = collision.GetComponent<Zombie>();
            if (zombie != null)
            {
                HandleZombieEnterOrStay(zombie);
            }
        }
        else if (collision.CompareTag("Zomboss"))
        {
            Zomboss zomboss = collision.GetComponent<Zomboss>();
            if (zomboss != null)
            {
                HandleZombossEnterOrStay(zomboss);
            }
        }
    }

    private void HandleZombieEnterOrStay(Zombie zombie)
    {
        // Freeze the zombie
        zombie.SetSpeedToZero();

        // Get the SpriteRenderer component
        SpriteRenderer spriteRenderer = zombie.GetComponent<SpriteRenderer>();

        // Only toggle the sprite renderer if it exists and is not already enabled
        if (spriteRenderer != null && !spriteRenderer.enabled)
        {
            zombie.ToggleSpriteRenderer();
        }

        // Start the color change coroutine if not already running
        if (spriteRenderer != null && !zombieColorChangeCoroutines.ContainsKey(zombie))
        {
            // Reset elapsed time if the enemy is not already transitioning
            if (!zombieElapsedTimes.ContainsKey(zombie))
            {
                zombieElapsedTimes[zombie] = 0f;
            }

            // Start the color change coroutine
            Coroutine coroutine = ColorChangeUtility.ChangeColorOverTime(
                this,
                spriteRenderer,
                spriteRenderer.color,
                Color.yellow,
                zombieColorChangeDuration - zombieElapsedTimes[zombie],
                () =>
                {
                    zombie.MarkColorAsFullyChanged();
                }
            );
            zombieColorChangeCoroutines[zombie] = coroutine;
        }
    }

    private void HandleZombieExit(Zombie zombie)
    {
        // Restore the zombie's speed
        zombie.CurrentSpeed = zombie.MaxSpeed;

        // Get the SpriteRenderer component
        SpriteRenderer spriteRenderer = zombie.GetComponent<SpriteRenderer>();

        // Only toggle the sprite renderer if it exists, is enabled, and the color is not fully changed
        if (spriteRenderer != null && spriteRenderer.enabled && !zombie.IsColorFullyChanged)
        {
            zombie.ToggleSpriteRenderer(timeToDeactivateSprite);
        }

        // Check if the coroutine exists before stopping it
        if (zombieColorChangeCoroutines.ContainsKey(zombie))
        {
            Coroutine coroutine = zombieColorChangeCoroutines[zombie];
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            zombieColorChangeCoroutines.Remove(zombie);
        }

        // Store the remaining time if the color is not fully changed
        if (!zombie.IsColorFullyChanged)
        {
            if (zombieElapsedTimes.ContainsKey(zombie))
            {
                zombieElapsedTimes[zombie] = Mathf.Clamp01(zombieElapsedTimes[zombie]); // Store the remaining time
            }
            else
            {
                zombieElapsedTimes[zombie] = 0f;
            }
        }
    }

    private void HandleZombossEnterOrStay(Zomboss zomboss)
    {
        SpriteRenderer spriteRenderer = zomboss.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && !spriteRenderer.enabled && !zomboss.IsColorFullyChanged)
        {
            zomboss.ToggleSpriteRenderer();
        }

        if (spriteRenderer != null && !zombossColorChangeCoroutines.ContainsKey(zomboss))
        {
            if (!zombossElapsedTimes.ContainsKey(zomboss))
            {
                zombossElapsedTimes[zomboss] = 0f;
            }

            Coroutine coroutine = ColorChangeUtility.ChangeColorOverTime(
                this,
                spriteRenderer,
                spriteRenderer.color,
                Color.yellow,
                zombossColorChangeDuration - zombossElapsedTimes[zomboss],
                () =>
                {
                    zomboss.MarkColorAsFullyChanged(); // Score is now handled in Enemy.cs
                }
            );
            zombossColorChangeCoroutines[zomboss] = coroutine;
        }
    }

    private void HandleZombossExit(Zomboss zomboss)
    {
        SpriteRenderer spriteRenderer = zomboss.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.enabled && !zomboss.IsColorFullyChanged)
        {
            zomboss.ToggleSpriteRenderer(timeToDeactivateSprite);
        }

        if (zombossColorChangeCoroutines.ContainsKey(zomboss))
        {
            Coroutine coroutine = zombossColorChangeCoroutines[zomboss];
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            zombossColorChangeCoroutines.Remove(zomboss);
        }

        if (!zomboss.IsColorFullyChanged)
        {
            if (zombossElapsedTimes.ContainsKey(zomboss))
            {
                zombossElapsedTimes[zomboss] = Mathf.Clamp01(zombossElapsedTimes[zomboss]);
            }
            else
            {
                zombossElapsedTimes[zomboss] = 0f;
            }
        }

        // Update Zomboss speed based on its current color
        zomboss.UpdateSpeedBasedOnColor();
    }
}

