using UnityEngine;
using System.Collections.Generic;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController Instance { get; private set; }
    public float playerMaxHealth = 3f;
    public float playerCurrentHealth;
    private List<GameObject> hearts;
    public Color colourToFlash = Color.white; // Default color for the light circle

    private SpriteRenderer lightCircleRenderer; // Reference to the light circle's SpriteRenderer
    private PlayerLightCircleCollision lightCircleCollision; // Reference to the PlayerLightCircleCollision component
    private Coroutine flashCoroutine; // To manage the flashing coroutine

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
            Instance = this;
        }
    }

    public bool IsDead => playerCurrentHealth <= 0;

    void Start()
    {
        playerCurrentHealth = playerMaxHealth;

        // Initialize the list of hearts
        hearts = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (child.CompareTag("Heart"))
            {
                hearts.Add(child.gameObject);
            }
        }

        GameObject lightCircleObject = GameObject.FindWithTag("PlayerLightCircle");
        if (lightCircleObject != null)
        {
            lightCircleCollision = lightCircleObject.GetComponent<PlayerLightCircleCollision>();
            lightCircleRenderer = lightCircleObject.GetComponent<SpriteRenderer>();
        }
        else
        {
            Debug.LogError("No GameObject with the tag 'PlayerLightCircle' found!");
        }
    }

    public void RemoveHeart()
    {
        if (hearts.Count > 0)
        {
            GameObject heartToRemove = hearts[hearts.Count - 1];
            hearts.RemoveAt(hearts.Count - 1);
            Destroy(heartToRemove);
        }
    }

    public void Heal(float amount)
    {
        playerCurrentHealth += amount;
        if (playerCurrentHealth > playerMaxHealth)
        {
            playerCurrentHealth = playerMaxHealth;
        }
        Debug.Log("Player healed! Current health: " + playerCurrentHealth);
    }

    // Updated method to handle taking damage
    public void TakeDamage(float amount)
    {
        playerCurrentHealth -= amount;

        if (playerCurrentHealth <= 0)
        {
            playerCurrentHealth = 0;
            Debug.Log("Player is dead!");
            // Handle player death logic here
        }

        // Remove hearts based on the amount of health lost
        for (int i = 0; i < amount && hearts.Count > 0; i++)
        {
            RemoveHeart();
        }
        Debug.Log("Player took damage! Current health: " + playerCurrentHealth);

        // Destroy all nearby enemies when the player takes damage
        if (lightCircleRenderer != null && lightCircleCollision != null)
        {
            // Flash the player's light circle to red for 1 second
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine); // Stop any ongoing flash
            }

            flashCoroutine = ColorChangeUtility.FlashColorForSeconds(
                this,
                lightCircleRenderer,
                colourToFlash,
                .15f,
                lightCircleRenderer.color // Reset to the original color
            );

            lightCircleCollision.DamageAllNearbyEnemies();
        }
    }
}
