using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class TeslaTower : Tower
{
    public enum TargetingMode
    {
        Closest,
        MostHealth,
        LeastHealth
    }

    [SerializeField]
    private TargetingMode targetingMode = TargetingMode.Closest;

    private LineRenderer lineRenderer;
    private float lineDisplayTime = 0.1f;
    private float lineTimer = 0f;

    [SerializeField] private RangeController rangeIndicator;

    private readonly List<Enemy> enemiesInRange = new List<Enemy>();

    // Track sprite coroutines per enemy to prevent overlap
    private readonly Dictionary<Enemy, Coroutine> spriteCoroutines = new Dictionary<Enemy, Coroutine>();

    protected void Awake()
    {
        range = 20f;
        damage = 1f;
        attacksPerSecond = 2f;

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.cyan;
        lineRenderer.endColor = Color.white;

        if (rangeIndicator == null)
            rangeIndicator = GetComponentInChildren<RangeController>();
        if (rangeIndicator != null)
            rangeIndicator.SetRange(range);
    }

    protected override void Update()
    {
        base.Update();

        if (lineRenderer.enabled)
        {
            lineTimer -= Time.deltaTime;
            if (lineTimer <= 0f)
            {
                lineRenderer.enabled = false;
            }
        }

        if (rangeIndicator != null)
            rangeIndicator.SetRange(range);
    }

    protected override void TryAttack()
    {
        if (enemiesInRange.Count == 0)
        {
            lineRenderer.enabled = false;
            return;
        }

        Enemy target = SelectTarget();
        if (target == null)
        {
            lineRenderer.enabled = false;
            return;
        }

        target.TakeDamage(damage);

        // 1. Move 10% closer to yellow
        SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color current = sr.color;
            Color yellow = Color.yellow;
            float t = 0.1f; // 10%
            Color newColor = Color.Lerp(current, yellow, t);
            sr.color = newColor;

            // If Zomboss, update speed based on color
            Zomboss zomboss = target as Zomboss;
            if (zomboss != null)
            {
                zomboss.UpdateSpeedBasedOnColor();
            }

            // 2. Enable sprite for 0.5s, then restore state
            // Stop previous coroutine if running
            if (spriteCoroutines.TryGetValue(target, out Coroutine running) && running != null)
            {
                StopCoroutine(running);
            }
            Coroutine newCoroutine = StartCoroutine(ShowSpriteTemporarily(target, sr, 0.5f));
            spriteCoroutines[target] = newCoroutine;
        }

        // Draw line to target
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, target.transform.position);
        lineRenderer.enabled = true;
        lineTimer = lineDisplayTime;
    }

    // Coroutine to enable sprite for a short time, then restore state
    private IEnumerator ShowSpriteTemporarily(Enemy enemy, SpriteRenderer sr, float duration)
    {
        sr.enabled = true;
        yield return new WaitForSeconds(duration);
        if (enemy != null)
            enemy.UpdateSpriteRendererState();
        spriteCoroutines.Remove(enemy);
    }

    private Enemy SelectTarget()
    {
        Enemy selected = null;
        switch (targetingMode)
        {
            case TargetingMode.Closest:
                float minDist = float.MaxValue;
                foreach (var enemy in enemiesInRange)
                {
                    if (enemy == null) continue;
                    float dist = Vector3.Distance(transform.position, enemy.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        selected = enemy;
                    }
                }
                break;
            case TargetingMode.MostHealth:
                float maxHealth = float.MinValue;
                foreach (var enemy in enemiesInRange)
                {
                    if (enemy == null) continue;
                    if (enemy.CurrentHealth > maxHealth)
                    {
                        maxHealth = enemy.CurrentHealth;
                        selected = enemy;
                    }
                }
                break;
            case TargetingMode.LeastHealth:
                float minHealth = float.MaxValue;
                foreach (var enemy in enemiesInRange)
                {
                    if (enemy == null) continue;
                    if (enemy.CurrentHealth < minHealth)
                    {
                        minHealth = enemy.CurrentHealth;
                        selected = enemy;
                    }
                }
                break;
        }
        return selected;
    }

    // Call this from UI or player input to change targeting mode
    public void SetTargetingMode(TargetingMode mode)
    {
        targetingMode = mode;
    }

    // Called by RangeIndicator
    public void OnRangeTriggerEnter(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);
        }
    }

    // Called by RangeIndicator
    public void OnRangeTriggerExit(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemiesInRange.Remove(enemy);
        }
    }
}