using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class TeslaTower : Tower
{
    [SerializeField]
    private GameObject lineRendererPrefab; // Assign a prefab with a LineRenderer in the inspector

    [SerializeField]
    private float lineDisplayTime = 0.08f;
    private float lineTimer = 0f;

    [Header("Tesla Visuals")]
    [SerializeField]
    private float spriteActiveTime = 0.7f; // Duration the sprite is visually affected

    // Pool for LineRenderers
    private readonly List<LineRenderer> linePool = new List<LineRenderer>();
    private int activeLineCount = 0;

    [Header("Tower Stats")]
    [SerializeField] protected float initialTowerRange = 20f;
    [SerializeField] protected float initalDamage = 1f;
    [SerializeField] protected float initalAttacksPerSecond = 1f;
    [SerializeField] private int MaxArcsPerEnemy = 2;
    [SerializeField] private int MaxChainTargets = 7;
    [SerializeField] private float ChainRange = 7;



    protected void Awake()
    {
        base.Awake(initialTowerRange, initalDamage, initalAttacksPerSecond);
        SetStats(initialTowerRange, initalDamage, initalAttacksPerSecond, ChainRange, MaxChainTargets, MaxArcsPerEnemy);
        // Optionally create a default prefab if not set
        if (lineRendererPrefab == null)
        {
            var go = new GameObject("TeslaArcLineRenderer");
            var lr = go.AddComponent<LineRenderer>();
            lr.startWidth = 0.04f;
            lr.endWidth = 0.04f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = Color.cyan;
            lr.endColor = Color.cyan;
            lr.enabled = false;
            lineRendererPrefab = go;
            go.SetActive(false);
        }
    }

    public void SetStats(float newRange, float newDamage, float newAttacksPerSecond, float newChainRange, int newMaxChainTargets, int newMaxArcsPerEnemy)
    {
        towerRange = newRange;
        ChainRange = newChainRange;
        MaxChainTargets = Mathf.Max(1, newMaxChainTargets);
        damage = newDamage;
        currentAttacksPerSecond = newAttacksPerSecond;
        MaxArcsPerEnemy = Mathf.Max(1, newMaxArcsPerEnemy);

        if (rangeIndicator != null)
            rangeIndicator.SetRange(towerRange);
    }

    protected override void Update()
    {
        base.Update();

        if (activeLineCount > 0)
        {
            lineTimer -= Time.deltaTime;
            if (lineTimer <= 0f)
            {
                DisableAllLines();
            }
        }
    }

    protected override void TryAttack()
    {
        if (enemiesInRange.Count == 0)
        {
            DisableAllLines();
            return;
        }

        // 1. Find the closest enemy to the tower
        Enemy firstTarget = SelectTarget();
        if (firstTarget == null)
        {
            DisableAllLines();
            return;
        }

        // 2. Chain logic: BFS from firstTarget, up to maxChainTargets, maxArcsPerEnemy per node
        var hitEnemies = new HashSet<Enemy>();
        var arcs = new List<(Vector3 from, Vector3 to)>();
        var queue = new Queue<(Enemy enemy, Vector3 fromPos)>();

        hitEnemies.Add(firstTarget);
        queue.Enqueue((firstTarget, transform.position));
        int totalTargets = 1;

        while (queue.Count > 0 && totalTargets < MaxChainTargets)
        {
            var (current, fromPos) = queue.Dequeue();

            // Find up to maxArcsPerEnemy closest unhit enemies within chainRange
            List<Enemy> nextTargets = FindClosestChainTargets(current, hitEnemies, ChainRange, MaxArcsPerEnemy);

            int arcsCreated = 0;
            foreach (var next in nextTargets)
            {
                if (totalTargets >= MaxChainTargets)
                    break;
                if (arcsCreated >= MaxArcsPerEnemy)
                    break;
                if (hitEnemies.Add(next))
                {
                    arcs.Add((current.transform.position, next.transform.position));
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

                // If the color is close enough to yellow, mark as fully changed
                if (ApproximatelyYellow(sr.color))
                {
                    target.MarkColorAsFullyChanged();
                }

                if (target is Zomboss zomboss)
                    zomboss.UpdateSpeedBasedOnColor();

                ColorChangeUtility.ShowSpriteTemporarily(this, target, sr, spriteActiveTime);
            }
        }

        // 4. Draw all arcs as individual segments using pooled LineRenderers
        EnsureLinePoolSize(arcs.Count);
        for (int i = 0; i < arcs.Count; i++)
        {
            var lr = linePool[i];
            lr.positionCount = 2;
            lr.SetPosition(0, arcs[i].from);
            lr.SetPosition(1, arcs[i].to);
            lr.enabled = true;
        }
        // Disable any unused lines from previous frame
        for (int i = arcs.Count; i < activeLineCount; i++)
            linePool[i].enabled = false;
        activeLineCount = arcs.Count;
        lineTimer = lineDisplayTime;
    }

    private void EnsureLinePoolSize(int needed)
    {
        while (linePool.Count < needed)
        {
            var go = Instantiate(lineRendererPrefab, transform);
            go.SetActive(true);
            var lr = go.GetComponent<LineRenderer>();
            lr.startWidth = 0.08f;
            lr.endWidth = 0.05f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = Color.cyan;
            lr.endColor = Color.white;
            lr.enabled = false;
            linePool.Add(lr);
        }
    }

    private void DisableAllLines()
    {
        for (int i = 0; i < activeLineCount; i++)
            linePool[i].enabled = false;
        activeLineCount = 0;
    }

    // Returns up to count closest enemies to 'from', not in 'exclude', within 'maxRange'
    private List<Enemy> FindClosestChainTargets(Enemy from, HashSet<Enemy> exclude, float maxRange, int count)
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

    // Helper method to check if a color is close to yellow
    private bool ApproximatelyYellow(Color color)
    {
        Color yellow = Color.yellow;
        float threshold = 0.1f;
        return Mathf.Abs(color.r - yellow.r) < threshold &&
               Mathf.Abs(color.g - yellow.g) < threshold &&
               Mathf.Abs(color.b - yellow.b) < threshold;
    }
}