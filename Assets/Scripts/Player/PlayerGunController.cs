using UnityEngine;

public class PlayerGunController : MonoBehaviour
{

    public Gun currentGun;
    void Update()
    {
        if (GameManager.Instance != null && (GameManager.Instance.IsGamePaused || GameManager.Instance.IsBuildMode))
            return;
        if (Input.GetButtonDown("Fire1"))
        {
            currentGun.Shoot();
        }

        if (Input.GetButtonDown("Reload") || Input.GetButtonDown("Fire2"))
        {
            currentGun.Reload();
        }
    }
}