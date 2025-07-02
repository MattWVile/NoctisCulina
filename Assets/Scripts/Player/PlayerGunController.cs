using UnityEngine;

public class PlayerGunController : MonoBehaviour
{
    public Gun currentGun;

    void Update()
    {
        if (ShouldBlockInput())
            return;
        HandleShootingInput();
        HandleReloadInput();
    }

    private bool ShouldBlockInput()
    {
        return GameManager.Instance != null && (GameManager.Instance.IsGamePaused || GameManager.Instance.IsBuildMode);
    }

    private void HandleShootingInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            currentGun.Shoot();
        }
    }

    private void HandleReloadInput()
    {
        if (Input.GetButtonDown("Reload") || Input.GetButtonDown("Fire2"))
        {
            currentGun.Reload();
        }
    }
}