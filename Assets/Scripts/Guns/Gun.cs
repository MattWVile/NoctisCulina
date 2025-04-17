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
        if (currentAmmo > 0)
        {
            // Implement firing logic
            currentAmmo--;
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
            FlashColourForSeconds(Color.red, .1f); // Flash red if out of ammo
        }
    }

    public virtual void Reload()
    {
        // Start the reload coroutine
        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        if (currentAmmo == MagSize)
        {
            Debug.Log("Already full ammo!");
            yield break; // Exit if already full
        }
        Debug.Log("Reloading...");
        // Wait for the reload time
        currentAmmo = 0;
        FlashColourForSeconds(Color.yellow, ReloadTime);
        yield return new WaitForSeconds(ReloadTime);
        // Reload the gun 
        currentAmmo = MagSize;
        Debug.Log("Reloaded!");
    }

    public virtual void FlashColourForSeconds(Color colourToFlash, float secondsToBeColor)
    {
        if (flashCoroutine != null)
        {
            return; // If a flash coroutine is already running, do nothing
        }
        flashCoroutine = StartCoroutine(FlashColourForSecondsCoroutine(colourToFlash, secondsToBeColor));
    }

    private IEnumerator FlashColourForSecondsCoroutine(Color colourToFlash, float secondsToBeColor)
    {
        if (gunSpriteRenderer != null)
        {
            gunSpriteRenderer.color = colourToFlash;
            yield return new WaitForSeconds(secondsToBeColor); // Flash duration
            gunSpriteRenderer.color = originalColor;
        }
        flashCoroutine = null; // Reset the coroutine reference
    }
}
