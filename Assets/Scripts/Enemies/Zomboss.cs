using UnityEngine;

public class Zomboss : Enemy
{
    private SpriteRenderer spriteRenderer;

    // Flashing state
    private bool isFlashing = false;
    private float flashTimer = 0f;
    private bool wasInCameraLastFrame = false;
    private Color originalColor;
    public Color flashColor = Color.white; // Set to desired flash color

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
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        // Camera bounds flash logic
        bool isInCamera = IsWithinCameraBounds();

        if (isInCamera && !wasInCameraLastFrame)
        {
            StartFlash();
        }
        wasInCameraLastFrame = isInCamera;

        if (isFlashing && spriteRenderer != null)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer > 0f)
            {
                if (!spriteRenderer.enabled)
                {
                    spriteRenderer.enabled = true;
                }
                spriteRenderer.color = flashColor;
            }
            else
            {
                spriteRenderer.color = originalColor;
                isFlashing = false;
            }
        }
    }

    private bool IsWithinCameraBounds()
    {
        Camera cam = Camera.main;
        if (cam == null) return false;

        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
        return viewportPos.x >= 0f && viewportPos.x <= 1f &&
               viewportPos.y >= 0f && viewportPos.y <= 1f &&
               viewportPos.z > 0f;
    }

    private void StartFlash()
    {
        isFlashing = true;
        flashTimer = 1f; // Flash for 1 second
    }

        if (isFlashing && spriteRenderer != null)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer > 0f)
            {
                if (!spriteRenderer.enabled)
                {
                    spriteRenderer.enabled = true;
                }
                spriteRenderer.color = flashColor;
            }
            else
            {
                if (spriteRenderer.enabled)
                {
                    spriteRenderer.enabled = false;
                }
                spriteRenderer.color = originalColor;
                isFlashing = false;
            }
        }
    }

    private bool IsWithinCameraBounds()
    {
        Camera cam = Camera.main;
        if (cam == null) return false;

        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
        return viewportPos.x >= 0f && viewportPos.x <= 1f &&
               viewportPos.y >= 0f && viewportPos.y <= 1f &&
               viewportPos.z > 0f;
    }

    private void StartFlash()
    {
        isFlashing = true;
        flashTimer = 1f; // Flash for 1 second
    }

        if (isFlashing && spriteRenderer != null)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer > 0f)
            {
                if (!spriteRenderer.enabled)
                {
                    spriteRenderer.enabled = true;
                }
                spriteRenderer.color = flashColor;
            }
            else
            {
                if (spriteRenderer.enabled)
                {
                    spriteRenderer.enabled = false;
                }
                spriteRenderer.color = originalColor;
                isFlashing = false;
            }
        }
    }

    private bool IsWithinCameraBounds()
    {
        Camera cam = Camera.main;
        if (cam == null) return false;

        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
        return viewportPos.x >= 0f && viewportPos.x <= 1f &&
               viewportPos.y >= 0f && viewportPos.y <= 1f &&
               viewportPos.z > 0f;
    }

    private void StartFlash()
    {
        isFlashing = true;
        flashTimer = 1f; // Flash for 1 second
    }

        if (isFlashing && spriteRenderer != null)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer > 0f)
            {
                if (!spriteRenderer.enabled)
                {
                    spriteRenderer.enabled = true;
                }
                spriteRenderer.color = flashColor;
            }
            else
            {
                if (spriteRenderer.enabled)
                {
                    spriteRenderer.enabled = false;
                }
                spriteRenderer.color = originalColor;
                isFlashing = false;
            }
        }
    }

    private bool IsWithinCameraBounds()
    {
        Camera cam = Camera.main;
        if (cam == null) return false;

        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
        return viewportPos.x >= 0f && viewportPos.x <= 1f &&
               viewportPos.y >= 0f && viewportPos.y <= 1f &&
               viewportPos.z > 0f;
    }

    private void StartFlash()
    {
        isFlashing = true;
        flashTimer = 1f; // Flash for 1 second
    }

        if (isFlashing && spriteRenderer != null)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer > 0f)
            {
                if (!spriteRenderer.enabled)
                {
                    spriteRenderer.enabled = true;
                }
                spriteRenderer.color = flashColor;
            }
            else
            {
                if (spriteRenderer.enabled)
                {
                    spriteRenderer.enabled = false;
                }
                spriteRenderer.color = originalColor;
                isFlashing = false;
            }
        }
    }

    private bool IsWithinCameraBounds()
    {
        Camera cam = Camera.main;
        if (cam == null) return false;

        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
        return viewportPos.x >= 0f && viewportPos.x <= 1f &&
               viewportPos.y >= 0f && viewportPos.y <= 1f &&
               viewportPos.z > 0f;
    }

    private void StartFlash()
    {
        isFlashing = true;
        flashTimer = 1f; // Flash for 1 second
    }

    // Update speed based on the current color
    public void UpdateSpeedBasedOnColor()
    {
        if (spriteRenderer == null) return;

        // Calculate how "yellow" the Zomboss is (yellow = max red + max green, no blue)
        float yellowFactor = (spriteRenderer.color.r + spriteRenderer.color.g) / 2f;

        // Interpolate speed: 1.0 (original color) -> 0.5 (fully yellow)
        CurrentSpeed = Mathf.Lerp(MaxSpeed * 0.25f, MaxSpeed, 1f - yellowFactor);
    }

    public void Die()
    {
        // Publish the ZombossDiedEvent
        EventBus.Publish(new ZombossDiedEvent { Sender = this });
        Destroy(gameObject);
    }
}
