using UnityEngine;

public class Zomboss : Enemy
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        TotalHealth = 330f;
        Damage = 2f;
        MaxSpeed = .4f;
        CurrentSpeed = MaxSpeed;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        ScoreWhenColurChanged = 1000;
        ScoreWhenKilled = 5000;
    }

    // Update speed based on the current color
    public void UpdateSpeedBasedOnColor()
    {
        if (spriteRenderer == null) return;

        // Calculate how "yellow" the Zomboss is (yellow = max red + max green, no blue)
        float yellowFactor = (spriteRenderer.color.r + spriteRenderer.color.g) / 2f;

        // Interpolate speed: 1.0 (original color) -> 0.5 (fully yellow)
        CurrentSpeed = Mathf.Lerp(MaxSpeed * 0.5f, MaxSpeed, 1f - yellowFactor);
    }
}
