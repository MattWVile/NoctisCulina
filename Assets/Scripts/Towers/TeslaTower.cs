using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class TeslaTower : Tower
{
    [SerializeField]
    private LineRenderer lineRenderer;
    private float lineDisplayTime = .05f;
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
        SetStats(20f, 3.3f, 7, 0f, 1.3f, 2);
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.startWidth = 0.08f; // Reduced from 0.1f to 0.05f
        lineRenderer.endWidth = 0.05f;   // Reduced from 0.1f to 0.05f
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

        // 1. Find the closest enemy to the tower
        Enemy firstTarget = null;
        float minDist = float.MaxValue;
        foreach (var enemy in enemiesInRange)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                firstTarget = enemy;
            }
        }

        if (firstTarget == null)
        {
            lineRenderer.enabled = false;
            return;
        }

        // 2. Chain logic: BFS from firstTarget, up to maxChainTargets, maxArcsPerEnemy per node
        var hitEnemies = new HashSet<Enemy>();
        var arcs = new List<(Vector3 from, Vector3 to)>();
        var queue = new Queue<(Enemy enemy, Vector3 fromPos)>();

        hitEnemies.Add(firstTarget);
        queue.Enqueue((firstTarget, transform.position));
        int totalTargets = 1;

        while (queue.Count > 0 && totalTargets < maxChainTargets)
        {
            var (current, fromPos) = queue.Dequeue();

            // Find up to maxArcsPerEnemy closest unhit enemies within chainRange
            List<Enemy> nextTargets = FindClosestChainTargets(current, new List<Enemy>(hitEnemies), chainRange, maxArcsPerEnemy);

            int arcsCreated = 0;
            foreach (var next in nextTargets)
            {
                if (totalTargets >= maxChainTargets)
                    break;
                if (arcsCreated >= maxArcsPerEnemy)
                    break;
                if (!hitEnemies.Contains(next))
                {
                    arcs.Add((current.transform.position, next.transform.position));
                    hitEnemies.Add(next);
                    queue.Enqueue((next, current.transform.position));
                    totalTargets++;
                    arcsCreated++;
                }
            }
        }

        // Add the initial arc from the tower to the first target
        arcs.Insert(0, (transform.position, firstTarget.transform.position));

        // 3. Apply effects to all hit enemies
        foreach (Enemy target in hitEnemies)
        {
            if (target == null) continue;

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

        // 4. Draw all arcs as individual segments
        if (arcs.Count > 0)
        {
            lineRenderer.positionCount = arcs.Count * 2;
            int i = 0;
            foreach (var arc in arcs)
            {
                lineRenderer.SetPosition(i++, arc.from);
                lineRenderer.SetPosition(i++, arc.to);
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