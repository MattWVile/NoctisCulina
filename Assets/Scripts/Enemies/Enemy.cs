using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour
{
    public float TotalHealth { get; protected set; }
    public float CurrentHealth { get; protected set; }
    public float Damage { get; protected set; }
    public float MaxSpeed { get; protected set; }
    public float CurrentSpeed { get; set; }
    public int ScoreWhenKilled { get; set; }
    public int ResourceWhenKilled { get; set; } // NEW: Resource reward for killing
    public int ScoreWhenColurChanged { get; set; }

    // Tracks if the color has fully changed
    public bool IsColorFullyChanged { get; private set; } = false;
    public bool HasScoreBeenAwarded { get; set; } = false;
    public bool IsInLightCone { get; set; }
    public bool IsInLightCircle { get; set; }

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
        AddResources(ResourceWhenKilled); // NEW: Give player resources
        Destroy(gameObject);
    }

    // Use this to update sprite visibility based on light/circle and color state
    public void UpdateSpriteRendererState()
    {
        // Prevent running if this object is destroyed
        if (this == null || gameObject == null)
            return;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;

        // Always enable if color is fully changed
        if (IsColorFullyChanged)
        {
            spriteRenderer.enabled = true;
        }
        else
        {
            spriteRenderer.enabled = IsInLightCone || IsInLightCircle;
        }
    }

    // Deprecated: Prefer UpdateSpriteRendererState for coordinated logic
    public virtual void ToggleSpriteRenderer(float timeDelay = 0f)
    {
        if (!IsColorFullyChanged)
        {
            StartCoroutine(ToggleSpriteRendererCoroutine(timeDelay));
        }
    }

    private IEnumerator ToggleSpriteRendererCoroutine(float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && !IsColorFullyChanged)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
        }
    }

    // Mark the color as fully changed and update sprite state
    public void MarkColorAsFullyChanged()
    {
        IsColorFullyChanged = true;
        UpdateSpriteRendererState();
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

    protected void AddResources(int amount)
    {
        if (PlayerResources.Instance != null)
        {
            PlayerResources.Instance.AddResources(amount);
        }
    }
}
