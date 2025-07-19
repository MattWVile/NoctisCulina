using System.Collections.Generic;
using UnityEngine;

public class PhotonBeamCollisionHandler : MonoBehaviour
{

    [Header("Beam Settings")]
    private const float slowFactor = 4f; // Factor by which enemies are slowed down 
    private const float DamageInterval = 0.1f;

    // Keeps track of every Enemy currently inside the beam
    private readonly HashSet<Enemy> _enemiesInBeam = new HashSet<Enemy>();

    private void OnEnable()
    {
        // Start calling ApplyDamageToAll immediately and then every DamageInterval
        InvokeRepeating(nameof(ApplyDamageToAll), 0f, DamageInterval);
    }

    private void OnDisable()
    {
        // Stop the repeated invocations when this object is disabled/destroyed
        CancelInvoke(nameof(ApplyDamageToAll));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (TryGetBeamTarget(other, out var enemy))
        {
            _enemiesInBeam.Add(enemy);
            enemy.ApplySlow(slowFactor);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (TryGetBeamTarget(other, out var enemy))
        {
            _enemiesInBeam.Remove(enemy);
            enemy.RemoveSlow();
        }
    }

    // This method is invoked every DamageInterval seconds via InvokeRepeating
    private void ApplyDamageToAll()
    {
        var tower = GetComponentInParent<PhotonCannonTower>();
        if (tower == null)
            return;

        // Take a snapshot of the current set to avoid modification during enumeration
        var snapshot = new List<Enemy>(_enemiesInBeam);

        foreach (var enemy in snapshot)
        {
            // Optional: ensure the enemy is still in the beam before hitting
            if (!_enemiesInBeam.Contains(enemy))
                continue;

            enemy.TakeBeamDamage(tower.Damage);
        }
    }
    // Helper to filter by tag and get the Enemy component in one go
    private bool TryGetBeamTarget(Collider2D col, out Enemy enemy)
    {
        enemy = null;

        if (col.CompareTag("Zombie") || col.CompareTag("Zomboss"))
        {
            enemy = col.GetComponent<Enemy>();
            return enemy != null;
        }
        return false;
    }
}
