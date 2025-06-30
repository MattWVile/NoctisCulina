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

    [SerializeField]
    private float chainRange = 8f; // Arc/chain range between enemies, independent of tower range

    protected void Awake()
    {
        range = 20f;
        damage = 1f;
        attacksPerSecond = 1.3f;

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

        // 1. Find the initial target (within tower range)
        Enemy firstTarget = SelectTarget();
        if (firstTarget == null)
        {
            lineRenderer.enabled = false;
            return;
        }

        // 2. Chain to the next two closest enemies (within chainRange, from the last hit enemy)
        List<Enemy> chainTargets = new List<Enemy> { firstTarget };
        Enemy current = firstTarget;

        for (int i = 0; i < 2; i++)
        {
            Enemy next = FindClosestEnemyGlobal(current, chainTargets, chainRange);
            if (next != null)
            {
                chainTargets.Add(next);
                current = next;
            }
            else
            {
                break;
            }
        }

        // 3. Apply effects and draw lines
        List<Vector3> linePoints = new List<Vector3> { transform.position };
        foreach (Enemy target in chainTargets)
        {
            if (target == null) continue;

            target.TakeDamage(damage);

            // Move 10% closer to yellow
            SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color currentColor = sr.color;
                Color yellow = Color.yellow;
                sr.color = Color.Lerp(currentColor, yellow, 0.1f);

                // If Zomboss, update speed based on color
                if (target is Zomboss zomboss)
                    zomboss.UpdateSpeedBasedOnColor();

                // Sprite flash logic (as before)
                if (spriteCoroutines.TryGetValue(target, out Coroutine running) && running != null)
                    StopCoroutine(running);
                Coroutine newCoroutine = StartCoroutine(ShowSpriteTemporarily(target, sr, 0.5f));
                spriteCoroutines[target] = newCoroutine;
            }

            linePoints.Add(target.transform.position);
        }

        // Draw the chain line
        lineRenderer.positionCount = linePoints.Count;
        lineRenderer.SetPositions(linePoints.ToArray());
        lineRenderer.enabled = true;
        lineTimer = lineDisplayTime;
    }

    // Helper: Find the closest enemy to 'from', not in 'exclude', within 'maxRange', searching all enemies in the scene
    private Enemy FindClosestEnemyGlobal(Enemy from, List<Enemy> exclude, float maxRange)
    {
        Enemy[] allEnemies = GameObject.FindObjectsOfType<Enemy>();
        Enemy closest = null;
        float minDist = float.MaxValue;
        foreach (var enemy in allEnemies)
        {
            if (enemy == null || exclude.Contains(enemy)) continue;
            float dist = Vector3.Distance(from.transform.position, enemy.transform.position);
            if (dist < minDist && dist <= maxRange)
            {
                minDist = dist;
                closest = enemy;
            }
        }
        return closest;
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