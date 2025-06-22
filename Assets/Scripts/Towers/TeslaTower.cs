using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class TeslaTower : Tower
{
    private LineRenderer lineRenderer;
    private float lineDisplayTime = 0.1f;
    private float lineTimer = 0f;

    [SerializeField] private Transform rangeIndicator; // Assign the "Range" child in the Inspector
    [SerializeField] public float rangeIndicatorScaleFactor = 2f; // Adjust if needed for your sprite

    // For drawing multiple lines, use a list of LineRenderers
    private List<LineRenderer> activeLines = new List<LineRenderer>();

    protected void Awake()
    {
        range = 20f;
        damage = 0f;
        fireRate = 5f; // 1 / 0.2s = 5 shots per second

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.cyan;
        lineRenderer.endColor = Color.white;

        // Find the range indicator if not assigned
        if (rangeIndicator == null)
            rangeIndicator = transform.Find("Range");

        // Set the scale of the range indicator to match the range
        if (rangeIndicator != null)
        {
            float diameter = range * rangeIndicatorScaleFactor;
            rangeIndicator.localScale = new Vector3(diameter, diameter, 1f);
        }
    }

    protected override void Update()
    {
        base.Update();

        // Hide all lines after a short time
        if (activeLines.Count > 0)
        {
            lineTimer -= Time.deltaTime;
            if (lineTimer <= 0f)
            {
                foreach (var lr in activeLines)
                {
                    if (lr != null) lr.enabled = false;
                }
                activeLines.Clear();
            }
        }
        if (rangeIndicator != null)
        {
            float diameter = range * rangeIndicatorScaleFactor;
            rangeIndicator.localScale = new Vector3(diameter, diameter, 1f);
        }
    }

    protected override void TryAttack()
    {
        // Find all enemies in range
        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        List<Enemy> targets = new List<Enemy>();
        foreach (var enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist <= range)
            {
                targets.Add(enemy);
            }
        }

        // Remove old lines
        foreach (var lr in activeLines)
        {
            if (lr != null) lr.enabled = false;
        }
        activeLines.Clear();

        // Attack and draw a line to each enemy
        foreach (var enemy in targets)
        {
            enemy.TakeDamage(damage);

            // For each enemy, create or reuse a LineRenderer
            LineRenderer lr = Instantiate(lineRenderer, transform);
            lr.enabled = true;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, enemy.transform.position);
            activeLines.Add(lr);
        }

        lineTimer = lineDisplayTime;
    }
}