using UnityEngine;

public class Pistol : Gun
{
    private void Awake()
    {
        ProjectilePrefab = Resources.Load<GameObject>("Prefabs/Projectiles/PistolBullet");
        BaseDamage = 10f;
        FireRate = 1f;
        ReloadTime = 1f;
        MagSize = 8;
    }

}
