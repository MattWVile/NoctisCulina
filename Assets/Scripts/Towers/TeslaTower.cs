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

    protected override void Awake()
    {
        base.Awake();
        SetStats(20f, 3.3f, 3, 1f, 1.3f);
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.cyan;
        lineRenderer.endColor = Color.white;
    }

    public void SetStats(float newRange, float newChainRange, int newMaxChainTargets, float newDamage, float newAttacksPerSecond)
    {
        towerRange = newRange;
        chainRange = newChainRange;
        maxChainTargets = Mathf.Max(1, newMaxChainTargets);
        damage = newDamage;
        attacksPerSecond = newAttacksPerSecond;

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

        // 1. Find the initial target (within tower range)
        Enemy firstTarget = SelectTarget();
        if (firstTarget == null)
        {
            lineRenderer.enabled = false;
            return;
        }

        // 2. Chain to the next closest enemies (within chainRange, from the last hit enemy)
        List<Enemy> chainTargets = new List<Enemy> { firstTarget };
        Enemy current = firstTarget;

        for (int i = 1; i < maxChainTargets; i++)
        {
            Enemy next = FindClosestChainTarget(current, chainTargets, chainRange);
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

                if (target is Zomboss zomboss)
                    zomboss.UpdateSpeedBasedOnColor();

                ColorChangeUtility.ShowSpriteTemporarily(this, target, sr, 0.5f);
            }

            linePoints.Add(target.transform.position);
        }

        // Draw the chain line
        lineRenderer.positionCount = linePoints.Count;
        lineRenderer.SetPositions(linePoints.ToArray());
        lineRenderer.enabled = true;
        lineTimer = lineDisplayTime;
    }

    private Enemy FindClosestChainTarget(Enemy from, List<Enemy> exclude, float maxRange)
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
}