using UnityEngine;
using System.Collections;

public abstract class Gun : MonoBehaviour
{
    public GameObject ProjectilePrefab { get; protected set; }
    public float BaseDamage { get; protected set; }
    public float FireRate { get; protected set; }
    public float ReloadTime { get; protected set; }
    public int MagSize { get; protected set; }
    protected int currentAmmo;

    public Color originalColor;

    private PlayerMovement playerMovement;
    private SpriteRenderer gunSpriteRenderer;
    private Coroutine flashCoroutine;

    private int emptyShootCount = 0; // Tracks consecutive attempts to shoot with no ammo
    private bool isReloading = false; // Tracks whether the gun is currently reloading

    protected virtual void Start()
    {
        currentAmmo = MagSize;
        playerMovement = GetComponentInParent<PlayerMovement>();
        gunSpriteRenderer = GetComponent<SpriteRenderer>();
        if (gunSpriteRenderer != null)
        {
            originalColor = gunSpriteRenderer.color;
        }
        else
        {
            Debug.LogError("SpriteRenderer component not found on the gun.");
        }
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement component not found on the player or its parent.");
        }
    }

    public virtual void Shoot()
    {
        if (isReloading)
        {
            Debug.Log("Cannot shoot while reloading.");
            return; // Prevent shooting while reloading
        }

        if (currentAmmo > 0)
        {
            // Implement firing logic
            currentAmmo--;
            emptyShootCount = 0; // Reset the empty shoot counter since ammo was used

            if (playerMovement != null)
            {
                // Instantiate the projectile with the player's current rotation
                GameObject newProjectile = Instantiate(ProjectilePrefab, transform.position, playerMovement.currentRotation);

                DefaultBulletBehaviour defaultBulletBehaviour = newProjectile.GetComponent<DefaultBulletBehaviour>();
                defaultBulletBehaviour.ParentGun = transform.gameObject;

                if (currentAmmo == 0)
                {
                    FlashColourForSeconds(Color.red, .1f); // Flash red if out of ammo
                }
            }
            else
            {
                Debug.LogError("PlayerMovement component not found, cannot determine shooting direction.");
            }
        }
        else
        {
            emptyShootCount++; // Increment the empty shoot counter

            FlashColourForSeconds(Color.red, .1f); // Flash red if out of ammo

            if (emptyShootCount >= 2)
            {
                Debug.Log("Automatically starting reload after second empty shot.");
                Reload(); // Start reloading after the second empty shot
            }
        }
    }

    public virtual void Reload()
    {
        if (isReloading)
        {
            Debug.Log("Already reloading.");
            return; // Prevent starting another reload while already reloading
        }

        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        if (currentAmmo == MagSize)
        {
            Debug.Log("Already full ammo!");
            yield break; // Exit if already full
        }

        isReloading = true; // Set reloading flag
        Debug.Log("Reloading...");
        FlashColourForSeconds(Color.yellow, ReloadTime); // Flash yellow for the duration of the reload

        // Wait for the reload time
        yield return new WaitForSeconds(ReloadTime);

        // Reload the gun
        currentAmmo = MagSize;
        Debug.Log("Reloaded!");
        emptyShootCount = 0; // Reset the empty shoot counter after reloading
        isReloading = false; // Reset reloading flag

        // Reset the gun's color to its original color after reloading
        if (gunSpriteRenderer != null)
        {
            gunSpriteRenderer.color = originalColor;
        }
    }

    public virtual void FlashColourForSeconds(Color colourToFlash, float secondsToBeColor)
    {
        // Prevent overriding the yellow color during reload
        if (gunSpriteRenderer.color == Color.yellow && colourToFlash != Color.yellow)
        {
            return;
        }

        // Stop any currently running flash coroutine to allow the new flash to take priority
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        // Start a new flash coroutine
        flashCoroutine = StartCoroutine(FlashColourForSecondsCoroutine(colourToFlash, secondsToBeColor));
    }

    private IEnumerator FlashColourForSecondsCoroutine(Color colourToFlash, float secondsToBeColor)
    {
        if (gunSpriteRenderer != null)
        {
            gunSpriteRenderer.color = colourToFlash;
            yield return new WaitForSeconds(secondsToBeColor); // Flash duration

            // Reset to original color unless the gun is still reloading (yellow)
            if (gunSpriteRenderer.color != Color.yellow)
            {
                gunSpriteRenderer.color = originalColor;
            }
        }
        flashCoroutine = null; // Reset the coroutine reference
    }
}
