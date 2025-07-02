using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }

    [Header("Tower Build Data")]
    public TowerData[] availableTowers;

    private TowerData selectedTower;
    private bool isBuildMode = false;
    private GameObject previewInstance;
    [Header("Preview Settings")]
    [SerializeField, Range(0f, 1f)] private float previewAlpha = 0.2f;
    [SerializeField] private Color unaffordableColor = new Color(0.6886792f, 0.2111517f, 0.2111517f, 0.4627451f); // semi-transparent red
    [SerializeField] private Color previewColor = new Color(.5f, 1f, 1f, 0.4627451f); // semi-transparent preview color

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
            DestroyPreview();
        }
    }

    public void SelectTowerToBuild(TowerData tower)
    {
        selectedTower = tower;
        CreatePreview();
    }

    private void Update()
    {
        if (!isBuildMode)
        {
            DestroyPreview();
            return;
        }

        // Tower selection input
        if (Input.GetKeyDown(KeyCode.Alpha1) && availableTowers.Length > 0)
        {
            SelectTowerToBuild(availableTowers[0]);
        }

        // Update preview position and color
        if (previewInstance != null)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            previewInstance.transform.position = mouseWorldPos;

            bool canAfford = PlayerResources.Instance != null && selectedTower != null && PlayerResources.Instance.Resources >= selectedTower.cost;
            UpdatePreviewColor(canAfford);
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
            // Optionally: Play build sound, animation, etc.
            // Keep preview if still affordable, else tint red
            if (PlayerResources.Instance.Resources < selectedTower.cost)
            {
                UpdatePreviewColor(false);
            }
        }
        else
        {
            // Optionally: Show not enough resources UI
            UpdatePreviewColor(false);
        }
    }

    private void CreatePreview()
    {
        DestroyPreview();
        if (selectedTower != null && selectedTower.towerPrefab != null)
        {
            previewInstance = Instantiate(selectedTower.towerPrefab);
            previewInstance.name = "Preview_" + selectedTower.towerPrefab.name;
            // Make all SpriteRenderers semi-transparent (original color, just lower alpha)
            foreach (var sr in previewInstance.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = previewColor;
            }
            // Disable any scripts/colliders on the preview
            foreach (var comp in previewInstance.GetComponents<MonoBehaviour>())
                comp.enabled = false;
            foreach (var col in previewInstance.GetComponentsInChildren<Collider2D>())
                col.enabled = false;
        }
    }

    private void UpdatePreviewColor(bool canAfford)
    {
        if (previewInstance == null) return;
        foreach (var sr in previewInstance.GetComponentsInChildren<SpriteRenderer>())
        {
            if (canAfford)
            {
                sr.color = previewColor;
            }
            else
            {
                sr.color = unaffordableColor;
            }
        }
    }

    private void DestroyPreview()
    {
        if (previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null;
        }
    }
}
