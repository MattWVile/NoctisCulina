using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameManager : MonoBehaviour
{

    public bool IsBuildMode { get; private set; } = false; // Build mode flag
                                                           // Start is called before the first frame update
    [SerializeField] private SpriteRenderer playerLightCone;

    private void Start()
    {
        if (playerLightCone == null)
        {
            playerLightCone = GameObject.FindGameObjectWithTag("PlayerLightCone").GetComponent<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleBuildModeToggleInput();
    }
    private void HandleBuildModeToggleInput()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!IsBuildMode)
            {
                EnterBuildMode();
            }
            else
            {
                ExitBuildMode();
            }
        }
    }

    public void TogglePlayerLightCone(bool isEnabled)
    {
        if (playerLightCone != null)
        {
            playerLightCone.enabled = isEnabled;
        }
        else
        {
            Debug.LogWarning("Player Light Cone SpriteRenderer not found!");
        }
    }

    public void EnterBuildMode()
    {
        IsBuildMode = true;
        if (BuildManager.Instance != null)
            BuildManager.Instance.SetBuildMode(true);
            TogglePlayerLightCone(false);
    }

    public void ExitBuildMode()
    {
        IsBuildMode = false;
        if (BuildManager.Instance != null)
        {
            BuildManager.Instance.SetBuildMode(false);
            TogglePlayerLightCone(true);
        }
    }
}
