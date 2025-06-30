using UnityEngine;
using System.Collections;

public static class ColorChangeUtility
{
    // Flash a SpriteRenderer's color for a given duration
    public static Coroutine FlashColorForSeconds(MonoBehaviour caller, SpriteRenderer spriteRenderer, Color colorToFlash, float duration, Color originalColor, bool preventOverride = false)
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer is null. Cannot flash color.");
            return null;
        }

        // Prevent overriding the yellow color during reload (if specified)
        if (preventOverride && spriteRenderer.color == Color.yellow && colorToFlash != Color.yellow)
        {
            return null;
        }

        // Start the coroutine to handle the flashing
        return caller.StartCoroutine(FlashColorCoroutine(spriteRenderer, colorToFlash, duration, originalColor));
    }

    private static IEnumerator FlashColorCoroutine(SpriteRenderer spriteRenderer, Color colorToFlash, float duration, Color originalColor)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = colorToFlash;
            yield return new WaitForSeconds(duration);

            // Reset to the original color unless the current color is yellow (reloading)
            if (spriteRenderer.color != Color.yellow)
            {
                spriteRenderer.color = originalColor;
            }
        }
    }

    // Pulse a SpriteRenderer's alpha channel for a given duration
    public static Coroutine PulseAlpha(MonoBehaviour caller, SpriteRenderer spriteRenderer, float duration, float pulseSpeed)
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer is null. Cannot pulse alpha.");
            return null;
        }

        return caller.StartCoroutine(PulseAlphaCoroutine(spriteRenderer, duration, pulseSpeed));
    }

    private static IEnumerator PulseAlphaCoroutine(SpriteRenderer spriteRenderer, float duration, float pulseSpeed)
    {
        float elapsedTime = 0f;
        Color originalColor = spriteRenderer.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.PingPong(elapsedTime * pulseSpeed, 1f); // Create a pulsing effect
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Reset to the original color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    // Gradually change a SpriteRenderer's color over time
    public static Coroutine ChangeColorOverTime(MonoBehaviour caller, SpriteRenderer spriteRenderer, Color startColor, Color endColor, float duration, System.Action onComplete = null)
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer is null. Cannot change color over time.");
            return null;
        }

        return caller.StartCoroutine(ChangeColorOverTimeCoroutine(spriteRenderer, startColor, endColor, duration, onComplete));
    }

    private static IEnumerator ChangeColorOverTimeCoroutine(SpriteRenderer spriteRenderer, Color startColor, Color endColor, float duration, System.Action onComplete)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (spriteRenderer == null || spriteRenderer.gameObject == null)
            {
                // Exit the coroutine if the SpriteRenderer or its GameObject is destroyed
                yield break;
            }

            float t = Mathf.Clamp01(elapsedTime / duration); // Ensure t is always between 0 and 1
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = endColor; // Ensure the final color is set
        }

        onComplete?.Invoke(); // Call the completion callback if provided
    }
    public static void ShowSpriteTemporarily(MonoBehaviour caller, Enemy enemy, SpriteRenderer sr, float duration)
    {
        if (caller != null && enemy != null && sr != null)
        {
            caller.StartCoroutine(ShowSpriteTemporarilyCoroutine(enemy, sr, duration));
        }
    }

    private static IEnumerator ShowSpriteTemporarilyCoroutine(Enemy enemy, SpriteRenderer sr, float duration)
    {
        sr.enabled = true;
        yield return new WaitForSeconds(duration);
        if (enemy != null)
            enemy.UpdateSpriteRendererState();
    }
}
