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
}
