using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class TeslaTower : Tower
{
    private LineRenderer lineRenderer;
    private float lineDisplayTime = 0.1f;
    private float lineTimer = 0f;

    [SerializeField] private RangeController rangeIndicator;

    private readonly List<Enemy> enemiesInRange = new List<Enemy>();

    protected void Awake()
    {
        range = 20f;
        damage = 1f;
        fireRate = 5f;

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

        List<Vector3> linePoints = new List<Vector3> { transform.position };
        foreach (var enemy in enemiesInRange)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                linePoints.Add(enemy.transform.position);
                linePoints.Add(transform.position);
            }
        }

        if (linePoints.Count > 1)
        {
            lineRenderer.positionCount = linePoints.Count;
            lineRenderer.SetPositions(linePoints.ToArray());
            lineRenderer.enabled = true;
            lineTimer = lineDisplayTime;
        }
        else
        {
            lineRenderer.enabled = false;
        }
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