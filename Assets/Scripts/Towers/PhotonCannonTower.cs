using UnityEngine;

public class PhotonCannonTower : Tower
{
    [Header("Photon Cannon Stats")]
    [SerializeField]
    private float initialRange = 20f;
    [SerializeField]
    private float initialDamage = 5f; // Damage per attack
    [SerializeField]
    private float initialAttacksPerSecond = 1f; // Attacks per second
    [SerializeField]
    private float maxBeamDuration = 1f; // Cooldown between attacks
    public void Awake()
    {
        base.Awake(initialRange, initialDamage, initialAttacksPerSecond);
    }
    protected override void Update()
    {
        base.Update();
        // Additional logic specific to PhotonCannonTower can be added here
    }
    protected override void TryAttack()
    {
        Debug.Log($"Photon Cannon Tower attacking with {damage} damage at {towerRange} range.");
    }
}
