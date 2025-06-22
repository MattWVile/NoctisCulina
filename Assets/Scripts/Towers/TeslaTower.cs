using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public class TeslaTower : Tower
{
    private LineRenderer lineRenderer;
    private CircleCollider2D rangeCollider;
    private float lineDisplayTime = 0.1f;
    private float lineTimer = 0f;

    [SerializeField] private Transform rangeIndicator;
    [SerializeField] public float rangeIndicatorScaleFactor = 2f;

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

        rangeCollider = GetComponent<CircleCollider2D>();
        rangeCollider.isTrigger = true;
        rangeCollider.radius = range;

        if (rangeIndicator == null)
            rangeIndicator = transform.Find("Range");

        if (rangeIndicator != null)
        {
            float diameter = range * rangeIndicatorScaleFactor;
            rangeIndicator.localScale = new Vector3(diameter, diameter, 1f);
        }
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
        {
            float diameter = range * rangeIndicatorScaleFactor;
            rangeIndicator.localScale = new Vector3(diameter, diameter, 1f);
        }
        if (rangeCollider.radius != range)
        {
            rangeCollider.radius = range;
        }
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemiesInRange.Remove(enemy);
        }
    }
}