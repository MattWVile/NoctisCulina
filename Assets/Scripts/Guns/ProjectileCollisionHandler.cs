using UnityEngine;

public class ProjectileCollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Zombie"))
        {
            HandleEnemyCollision(other);
        }        
        if (other.CompareTag("Zomboss"))
        {
            HandleEnemyCollision(other);
        }
    }

    private void HandleEnemyCollision(Collider2D enemyCollider)
    {
        Enemy enemy = enemyCollider.GetComponent<Enemy>();
        Projectile projectile = GetComponent<Projectile>();

        if (enemy != null && projectile != null)
        {
            // Calculate total damage
            float projectileBaseDamage = projectile.BaseDamage;
            float parentGunBaseDamage = projectile.ParentGun.GetComponent<Pistol>().BaseDamage;

            // Apply damage to the enemy
            enemy.TakeDamage(parentGunBaseDamage + projectileBaseDamage);

            // Handle projectile-specific collision behavior
            projectile.HandleCollisionWithEnemy(enemyCollider);
        }
    }
}
