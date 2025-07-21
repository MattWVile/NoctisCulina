using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameManager : MonoBehaviour
{

    public bool IsBuildMode { get; private set; } = false; // Build mode flag
    // Start is called before the first frame update
    void Start()
    {
        
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

    public void EnterBuildMode()
    {
        Debug.Log("Entered Build Mode");
        if (BuildManager.Instance != null)
            BuildManager.Instance.SetBuildMode(true);
    }

    public void ExitBuildMode()
    {
        if (BuildManager.Instance != null)
        {
            BuildManager.Instance.SetBuildMode(false);
            BuildManager.Instance.SelectTowerToBuild(null);
        }
    }


}
