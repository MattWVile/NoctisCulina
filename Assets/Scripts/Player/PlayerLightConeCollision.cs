using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerLightConeCollision : MonoBehaviour
{
    [SerializeField]

    private Dictionary<Zombie, Coroutine> zombieColorChangeCoroutines = new Dictionary<Zombie, Coroutine>();
    private Dictionary<Zombie, float> zombieElapsedTimes = new Dictionary<Zombie, float>();

    private Dictionary<Zomboss, Coroutine> zombossColorChangeCoroutines = new Dictionary<Zomboss, Coroutine>();
    private Dictionary<Zomboss, float> zombossElapsedTimes = new Dictionary<Zomboss, float>();

    // New: Track exit coroutines for delayed sprite disabling
    private Dictionary<Zombie, Coroutine> zombieExitCoroutines = new Dictionary<Zombie, Coroutine>();
    private Dictionary<Zomboss, Coroutine> zombossExitCoroutines = new Dictionary<Zomboss, Coroutine>();

    private float spriteDisableDelay = 0.4f; // Delay before disabling sprite

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleEnemyLogic(collision);
        if (collision.CompareTag("PhotonCannon"))
        {
            collision.GetComponent<PhotonCannonTower>().currentAttacksPerSecond = 10;
        }
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
                zombie.IsInLightCone = false;
                HandleZombieExit(zombie);

                // Start delayed sprite disable
                if (zombieExitCoroutines.ContainsKey(zombie))
                {
                    StopCoroutine(zombieExitCoroutines[zombie]);
                    zombieExitCoroutines.Remove(zombie);
                }
                Coroutine coroutine = StartCoroutine(DelayedSpriteDisable(zombie));
                zombieExitCoroutines[zombie] = coroutine;
            }
        }
        else if (collision.CompareTag("Zomboss"))
        {
            Zomboss zomboss = collision.GetComponent<Zomboss>();
            if (zomboss != null)
            {
                zomboss.IsInLightCone = false;
                HandleZombossExit(zomboss);

                // Start delayed sprite disable
                if (zombossExitCoroutines.ContainsKey(zomboss))
                {
                    StopCoroutine(zombossExitCoroutines[zomboss]);
                    zombossExitCoroutines.Remove(zomboss);
                }
                Coroutine coroutine = StartCoroutine(DelayedSpriteDisable(zomboss));
                zombossExitCoroutines[zomboss] = coroutine;
            }
        }
        else if (collision.CompareTag("PhotonCannon"))
        {
            PhotonCannonTower photonCannon = collision.GetComponent<PhotonCannonTower>();
            photonCannon.currentAttacksPerSecond = photonCannon.initialAttacksPerSecond;
        }
    }

    private void HandleEnemyLogic(Collider2D collision)
    {
        if (collision.CompareTag("Zombie"))
        {
            Zombie zombie = collision.GetComponent<Zombie>();
            if (zombie != null)
            {
                zombie.IsInLightCone = true;

                // Cancel delayed sprite disable if re-entered
                if (zombieExitCoroutines.ContainsKey(zombie))
                {
                    StopCoroutine(zombieExitCoroutines[zombie]);
                    zombieExitCoroutines.Remove(zombie);
                }

                zombie.UpdateSpriteRendererState();
                HandleZombieEnterOrStay(zombie);
            }
        }
        else if (collision.CompareTag("Zomboss"))
        {
            Zomboss zomboss = collision.GetComponent<Zomboss>();
            if (zomboss != null)
            {
                zomboss.IsInLightCone = true;

                // Cancel delayed sprite disable if re-entered
                if (zombossExitCoroutines.ContainsKey(zomboss))
                {
                    StopCoroutine(zombossExitCoroutines[zomboss]);
                    zombossExitCoroutines.Remove(zomboss);
                }

                zomboss.UpdateSpriteRendererState();
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
                zombie.colourChangeDuration - zombieElapsedTimes[zombie],
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
        zombie.ResetToMaxSpeed();

        // Get the SpriteRenderer component
        SpriteRenderer spriteRenderer = zombie.GetComponent<SpriteRenderer>();

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
                zombieElapsedTimes[zombie] = Mathf.Clamp01(zombieElapsedTimes[zombie]);
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

        // Start the color change coroutine if not already running
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
                zomboss.colourChangeDuration - zombossElapsedTimes[zomboss],
                () =>
                {
                    zomboss.MarkColorAsFullyChanged();
                }
            );
            zombossColorChangeCoroutines[zomboss] = coroutine;
        }
    }

    private void HandleZombossExit(Zomboss zomboss)
    {
        SpriteRenderer spriteRenderer = zomboss.GetComponent<SpriteRenderer>();

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

    // Coroutine to delay sprite renderer disabling
    private IEnumerator DelayedSpriteDisable(Enemy enemy)
    {
        yield return new WaitForSeconds(spriteDisableDelay);
        enemy.UpdateSpriteRendererState();
    }
}