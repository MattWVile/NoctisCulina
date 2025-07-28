using UnityEngine;
using System.Collections.Generic;

public abstract class Tower : MonoBehaviour
{
    public enum TargetingMode
    {
        Closest,
        MostHealth,
        LeastHealth,
        AllEnemies
    }

    [Header("Tower Stats")]
    [SerializeField] protected float towerRange = 20f;
    [SerializeField] protected float damage = 1f;
    [SerializeField] public float currentAttacksPerSecond = 1f;
    [SerializeField] protected TargetingMode targetingMode = TargetingMode.Closest;

    [SerializeField] protected RangeController rangeIndicator;

    public float Range => towerRange;
    public float Damage => damage;
    public float AttacksPerSecond => currentAttacksPerSecond;
    public TargetingMode CurrentTargetingMode => targetingMode;

    protected float attackCooldown = 0f;

    // Managed by collider triggers for most towers, or ignored for global towers
    protected readonly List<Enemy> enemiesInRange = new List<Enemy>();

    protected virtual void Awake(float towerRange, float damage, float attacksPerSecond)
    {
        if (rangeIndicator == null)
            rangeIndicator = GetComponentInChildren<RangeController>();
        if (rangeIndicator != null)
            rangeIndicator.SetRange(towerRange);
        SetStats(towerRange, damage, attacksPerSecond);
    }

    public virtual void SetStats(float newRange, float newDamage, float newAttacksPerSecond)
    {
        towerRange = newRange;
        damage = newDamage;
        currentAttacksPerSecond = newAttacksPerSecond;

        if (rangeIndicator != null)
            rangeIndicator.SetRange(towerRange);
    }

    public void SetTargetingMode(TargetingMode mode)
    {
        targetingMode = mode;
    }

    protected virtual void Update()
    {
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0f)
        {
            TryAttack();
            attackCooldown = 1f / currentAttacksPerSecond;
        }

        if (rangeIndicator != null)
            rangeIndicator.SetRange(towerRange);
    }

    // Derived classes should implement their own attack logic
    protected abstract void TryAttack();

    // For collider-based towers, these can be used as-is or overridden
    public virtual void OnRangeTriggerEnter(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);
        }
    }

    public virtual void OnRangeTriggerExit(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemiesInRange.Remove(enemy);
        }
    }

    // General targeting logic for collider-based towers
    protected virtual Enemy SelectTarget()
    {
        if (enemiesInRange.Count == 0)
            return null;

        switch (targetingMode)
        {
            case TargetingMode.Closest:
                float minDist = float.MaxValue;
                Enemy closest = null;
                foreach (var enemy in enemiesInRange)
                {
                    if (enemy == null) continue;
                    float dist = Vector3.Distance(transform.position, enemy.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closest = enemy;
                    }
                }
                return closest;

            case TargetingMode.MostHealth:
                float maxHealth = float.MinValue;
                Enemy mostHealth = null;
                foreach (var enemy in enemiesInRange)
                {
                    if (enemy == null) continue;
                    if (enemy.CurrentHealth > maxHealth)
                    {
                        maxHealth = enemy.CurrentHealth;
                        mostHealth = enemy;
                    }
                }
                return mostHealth;

            case TargetingMode.LeastHealth:
                float minHealth = float.MaxValue;
                Enemy leastHealth = null;
                foreach (var enemy in enemiesInRange)
                {
                    if (enemy == null) continue;
                    if (enemy.CurrentHealth < minHealth)
                    {
                        minHealth = enemy.CurrentHealth;
                        leastHealth = enemy;
                    }
                }
                return leastHealth;

            case TargetingMode.AllEnemies:
                // For AllEnemies, you may want to return null and let the derived class handle multi-target logic
                return null;
        }
        return null;
    }
}