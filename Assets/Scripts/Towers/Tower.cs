using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    [Header("Tower Stats")]
    [SerializeField] protected float range = 5f;
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected float fireRate = 1f; // shots per second

    public float Range => range;
    public float Damage => damage;
    public float FireRate => fireRate;

    protected float fireCooldown = 0f;

    protected virtual void Update()
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            TryAttack();
            fireCooldown = 1f / fireRate;
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
            if (dist <= range && dist < minDist)
            {
                minDist = dist;
                nearest = enemy.transform;
            }
        }
        return nearest;
    }
}