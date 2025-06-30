using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    [Header("Tower Stats")]
    [SerializeField] protected float towerRange = 5f;
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected float attacksPerSecond = 1f; // attacks per second

    public float Range => towerRange;
    public float Damage => damage;
    public float AttacksPerSecond => attacksPerSecond;

    protected float attackCooldown = 0f;

    protected virtual void Update()
    {
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0f)
        {
            TryAttack();
            attackCooldown = 1f / attacksPerSecond;
        }
    }

    // Derived classes should implement their own attack logic
    protected abstract void TryAttack();

    protected Transform FindNearestEnemyInRange()
    {
        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (var enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist <= towerRange && dist < minDist)
            {
                minDist = dist;
                nearest = enemy.transform;
            }
        }
        return nearest;
    }
}