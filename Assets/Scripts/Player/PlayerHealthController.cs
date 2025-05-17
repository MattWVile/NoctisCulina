using UnityEngine;
using System.Collections.Generic;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController Instance { get; private set; }
    public float playerMaxHealth = 3f;
    public float playerCurrentHealth;
    private List<GameObject> hearts;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsDead
    {
        get { return playerCurrentHealth <= 0; }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerCurrentHealth = playerMaxHealth;

        // Initialize the list of hearts
        hearts = new List<GameObject>();

        // Populate the list with the heart GameObjects
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Heart"))
            {
                hearts.Add(child.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            playerCurrentHealth -= collision.gameObject.GetComponent<Zombie>().Damage;

            var playerLightCircleCollisionController = FindObjectOfType<PlayerLightCircleCollision>();
            if (playerLightCircleCollisionController != null)
            {
                if (playerCurrentHealth <= 0)
                {
                    playerCurrentHealth = 0; // Ensure health doesn't go below zero
                    RemoveHeart(); // Remove the last heart
                                   // You can add logic here to restart the game or show a game over screen
                }
                else
                {
                    RemoveHeart();
                }

                // Create a copy of the enemies list to iterate over
                var enemiesCopy = new List<GameObject>(playerLightCircleCollisionController.enemiesInCircle);
                foreach (var enemy in enemiesCopy)
                {
                    if (enemy != null && enemy.GetComponent<Zombie>() != null)
                    {
                        enemy.GetComponent<Zombie>().Die();
                    }
                }

                // Remove destroyed enemies from the original list
                playerLightCircleCollisionController.enemiesInCircle.RemoveAll(enemy => enemy == null);
            }
        }
    }

    private void RemoveHeart()
    {
        if (hearts.Count > 0)
        {
            // Get the last heart in the list
            GameObject heartToRemove = hearts[hearts.Count - 1];

            // Remove the heart from the list
            hearts.RemoveAt(hearts.Count - 1);

            // Destroy the heart GameObject
            Destroy(heartToRemove);
        }
    }

    public void Heal(float amount)
    {
        playerCurrentHealth += amount;
        if (playerCurrentHealth > playerMaxHealth)
        {
            playerCurrentHealth = playerMaxHealth; // Ensure health doesn't exceed max health
        }
        Debug.Log("Player healed! Current health: " + playerCurrentHealth);
    }
}
