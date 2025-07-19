using UnityEngine;
using UnityEngine.PlayerLoop;

public class PhotonCannonTower : Tower
{
    [Header("Photon Cannon Stats")]
    [SerializeField]
    private float initialRange = 20f;
    [SerializeField]
    private float initialDamage = 5f; // Damage per attack
    [SerializeField]
    private float initialAttacksPerSecond = .2f; // Attacks per second
    [SerializeField]
    private float maxBeamDuration = .2f; // Time the beam is active


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
        Debug.Log($"Photon Cannon Tower attacking with {damage} damage at {towerRange} range");
        InitializeBeam();
    }
    private void InitializeBeam()
    {
        GameObject photonBeam = Instantiate(Resources.Load<GameObject>("Prefabs/Beams/PhotonCannonBeam"), transform);
        StartCoroutine(BeamCoroutine(photonBeam));
    }
    private System.Collections.IEnumerator BeamCoroutine(GameObject photonBeamToDestroy)
    {
        yield return new WaitForSeconds(maxBeamDuration);
         Destroy(photonBeamToDestroy);
    }
}
