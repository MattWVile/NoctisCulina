using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }

    [Header("Tower Build Data")]
    public TowerData[] availableTowers;

    private TowerData selectedTower;
    private bool isBuildMode = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetBuildMode(bool enabled)
    {
        isBuildMode = enabled;
        if (!isBuildMode)
        {
            selectedTower = null;
        }
    }

    public void SelectTowerToBuild(TowerData tower)
    {
        selectedTower = tower;
    }

    private void Update()
    {
        if (!isBuildMode)
            return;

        // Tower selection input
        if (Input.GetKeyDown(KeyCode.Alpha1) && availableTowers.Length > 0)
        {
            SelectTowerToBuild(availableTowers[0]);
        }

        // Place tower on left mouse click
        if (selectedTower != null && Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            TryBuildTower(mouseWorldPos);
        }
    }

    private void TryBuildTower(Vector3 position)
    {
        if (PlayerResources.Instance != null && PlayerResources.Instance.SpendResources(selectedTower.cost)) 
        {
            Instantiate(selectedTower.towerPrefab, position, Quaternion.identity);
            // TODO: Play build sound, animation, etc.
        }
        else
        {
            // TODO: Show not enough resources UI
        }
    }
}
