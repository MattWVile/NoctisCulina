using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class TeslaTower : Tower
{
    [SerializeField]
    private LineRenderer lineRenderer;
    private float lineDisplayTime = 0.1f;
    private float lineTimer = 0f;

    [SerializeField]
    private float chainRange; // Arc/chain range between enemies, independent of tower range

    [SerializeField]
    private int maxChainTargets; // Total number of enemies the arc can hit (including the first)

    [SerializeField]
    private int maxArcsPerEnemy; // How many arcs branch from each hit enemy

    protected override void Awake()
    {
        base.Awake();
        SetStats(20f, 3.3f, 7, 1f, 1.3f, 2);
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.cyan;
        lineRenderer.endColor = Color.white;
    }

    public void SetStats(float newRange, float newChainRange, int newMaxChainTargets, float newDamage, float newAttacksPerSecond, int newMaxArcsPerEnemy)
    {
        towerRange = newRange;
        chainRange = newChainRange;
        maxChainTargets = Mathf.Max(1, newMaxChainTargets);
        damage = newDamage;
        attacksPerSecond = newAttacksPerSecond;
        maxArcsPerEnemy = Mathf.Max(1, newMaxArcsPerEnemy);

        if (rangeIndicator != null)
            rangeIndicator.SetRange(towerRange);
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
    }

    protected override void TryAttack()
    {
        if (enemiesInRange.Count == 0)
        {
            lineRenderer.enabled = false;
            return;
        }

        Enemy firstTarget = SelectTarget();
        if (firstTarget == null)
        {
            lineRenderer.enabled = false;
            return;
        }

        // Map to track each enemy's parent (for line rendering)
        Dictionary<Enemy, Vector3> parentMap = new Dictionary<Enemy, Vector3>();
        List<Enemy> hitEnemies = new List<Enemy> { firstTarget };
        Queue<Enemy> queue = new Queue<Enemy>();
        queue.Enqueue(firstTarget);

        // The first arc is from the tower to the first target
        parentMap[firstTarget] = transform.position;

        while (queue.Count > 0 && hitEnemies.Count < maxChainTargets)
        {
            Enemy current = queue.Dequeue();
            List<Enemy> nextTargets = FindClosestChainTargets(current, hitEnemies, chainRange, maxArcsPerEnemy);

            foreach (var next in nextTargets)
            {
                if (hitEnemies.Count >= maxChainTargets)
                    break;

                hitEnemies.Add(next);
                queue.Enqueue(next);

                // The parent of this enemy is the current enemy
                parentMap[next] = current.transform.position;
            }
        }

        // Apply effects to all hit enemies
        HashSet<Enemy> alreadyProcessed = new HashSet<Enemy>();
        foreach (Enemy target in hitEnemies)
        {
            if (target == null || alreadyProcessed.Contains(target)) continue;
            alreadyProcessed.Add(target);

            target.TakeDamage(damage);

            SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color currentColor = sr.color;
                Color yellow = Color.yellow;
                sr.color = Color.Lerp(currentColor, yellow, 0.1f);

                if (target is Zomboss zomboss)
                    zomboss.UpdateSpeedBasedOnColor();

                ColorChangeUtility.ShowSpriteTemporarily(this, target, sr, 0.5f);
            }
        }

        // Draw the chain lines (one segment per arc, from parent to child)
        if (parentMap.Count > 0)
        {
            lineRenderer.positionCount = parentMap.Count * 2;
            int i = 0;
            foreach (var kvp in parentMap)
            {
                lineRenderer.SetPosition(i++, kvp.Value); // parent position
                lineRenderer.SetPosition(i++, kvp.Key.transform.position); // child position
            }
            lineRenderer.enabled = true;
            lineTimer = lineDisplayTime;
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    // Returns up to count closest enemies to 'from', not in 'exclude', within 'maxRange'
    private List<Enemy> FindClosestChainTargets(Enemy from, List<Enemy> exclude, float maxRange, int count)
    {
        Enemy[] allEnemies = GameObject.FindObjectsOfType<Enemy>();
        List<Enemy> candidates = new List<Enemy>();
        foreach (var enemy in allEnemies)
        {
            if (enemy == null || exclude.Contains(enemy)) continue;
            float dist = Vector3.Distance(from.transform.position, enemy.transform.position);
            if (dist <= maxRange)
            {
                candidates.Add(enemy);
            }
        }
        // Sort by distance
        candidates.Sort((a, b) =>
            Vector3.Distance(from.transform.position, a.transform.position)
            .CompareTo(Vector3.Distance(from.transform.position, b.transform.position)));
        // Take up to 'count'
        if (candidates.Count > count)
            candidates.RemoveRange(count, candidates.Count - count);
        return candidates;
    }
}