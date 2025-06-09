using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour
{
    public float TotalHealth { get; protected set; }
    public float CurrentHealth { get; protected set; }
    public float Damage { get; protected set; }
    public float MaxSpeed { get; protected set; }
    public float CurrentSpeed { get; set; }
    public int  ScoreWhenKilled { get; set; }
    public int ScoreWhenColurChanged { get; set; }

    // New boolean to track if the color has fully changed
    public bool IsColorFullyChanged { get; private set; } = false;
    public bool HasScoreBeenAwarded { get; set; } = false;

    protected virtual void Start()
    {
        CurrentHealth = TotalHealth;
    }

    public virtual void TakeDamage(float damageAmount)
    {
        CurrentHealth -= damageAmount;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        AddScore(ScoreWhenKilled);
        Destroy(gameObject);
    }

    public virtual void ToggleSpriteRenderer(float timeDelay = 0f)
    {
        // Only toggle the SpriteRenderer if the color has not fully changed
        if (!IsColorFullyChanged)
        {
            StartCoroutine(ToggleSpriteRendererCoroutine(timeDelay));
        }
    }

    private IEnumerator ToggleSpriteRendererCoroutine(float timeDelay)
    {
        // Wait for the specified time delay
        yield return new WaitForSeconds(timeDelay);

        // Toggle the sprite renderer
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && !IsColorFullyChanged)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
        }
    }

    // New method to mark the color as fully changed
    public void MarkColorAsFullyChanged()
    {
        IsColorFullyChanged = true;

        // Ensure the SpriteRenderer is enabled when the color is fully changed
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
        AddScore(ScoreWhenColurChanged);
    }
    protected void AddScore(int score)
    {
        if (ScoreController.Instance != null)
        {
            ScoreController.Instance.AddScore(score);
        }
        else
        {
            Debug.LogError("ScoreController instance not found.");
        }
    }
}
