using UnityEngine;

public class PhotonCannonTower : Tower
{
    //[Header("Photon Cannon Stats")]
    private float initialRange = 20f;
    private float initialDamage = 0.1f;// Damage per attack
    private float maxBeamDuration = .5f;// Time the beam is active

    public float initialAttacksPerSecond = 2f;// Attacks per second
    public PhotonBeamChargeBar photonBeamChargeBar;
    private bool isBeamActive = false;

    public void Awake()
    {
        base.Awake(initialRange, initialDamage, initialAttacksPerSecond);// Set the attacks per second for the base class
    }
    protected override void Update()
    {
        ChargeShot();
        base.Update();
        // Additional logic specific to PhotonCannonTower can be added here
    }
    protected override void TryAttack()
    {
        if (!photonBeamChargeBar.isCharged)
            return; // Not enough charge time to fire a beam
        InitializeBeam();
        photonBeamChargeBar.EmptyBar(); // Reset after firing
    }
    private void ChargeShot()
    {
        if (isBeamActive)
            return; // Do not charge if the beam is already active
        if (!photonBeamChargeBar.isCharged)
        {
            photonBeamChargeBar.FillBar(currentAttacksPerSecond);
        }
        // Do not empty the bar here!
    }

    private void InitializeBeam()
    {
        GameObject photonBeam = Instantiate(Resources.Load<GameObject>("Prefabs/Beams/PhotonCannonBeam"), transform);
        StartCoroutine(BeamCoroutine(photonBeam));
    }
    private System.Collections.IEnumerator BeamCoroutine(GameObject photonBeamToDestroy)
    {
        isBeamActive = true;
        yield return new WaitForSeconds(maxBeamDuration);
         Destroy(photonBeamToDestroy);
        isBeamActive = false;
    }
    public override void OnRangeTriggerEnter(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemiesInRange.Add(enemy);
            enemy.UpdateSpriteRendererState(false);
        }
    }

    public override void OnRangeTriggerExit(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemiesInRange.Remove(enemy);
            // Do nothing to the sprite on exit
        }
    }

}
