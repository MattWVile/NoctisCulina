using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }

    [Header("Tower Build Data")]
    public TowerData[] availableTowers;

    private TowerData selectedTower;
    private bool isBuildMode = false;
    private GameObject towerPreviewInstance;
    private SpriteRenderer previewRangeRenderer;
    [Header("Preview Settings")]
    [SerializeField] private Color unaffordableColor = new Color(0.6886792f, 0.2111517f, 0.2111517f, 0.4627451f); // semi-transparent red

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
            DestroyTowerPreview();
        }
    }

    public void SelectTowerToBuild(TowerData tower)
    {
        selectedTower = tower;
        CreateSimpleTowerPreviewFromPrefab();
    }

    private void Update()
    {
        if (!isBuildMode) return;

        HandleTowerSelectionInput();
        UpdateTowerPreviewPositionAndColor();
        HandleTowerPlacementInput();
    }

    private void HandleTowerSelectionInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && availableTowers.Length > 0)
        {
            SelectTowerToBuild(availableTowers[0]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && availableTowers.Length > 0)
        {
            SelectTowerToBuild(availableTowers[1]);
        }
    }

    private void UpdateTowerPreviewPositionAndColor()
    {
        if (towerPreviewInstance == null) return;
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        towerPreviewInstance.transform.position = mouseWorldPos;
        bool canAfford = PlayerResources.Instance != null && selectedTower != null && PlayerResources.Instance.Resources >= selectedTower.cost;
        UpdateTowerPreviewColor(canAfford);
    }

    private void HandleTowerPlacementInput()
    {
        if (selectedTower != null && Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            TryBuildTower(mouseWorldPos);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        return mouseWorldPos;
    }

    private void TryBuildTower(Vector3 position)
    {
        if (PlayerResources.Instance != null && PlayerResources.Instance.SpendResources(selectedTower.cost))
        {
            Instantiate(selectedTower.towerPrefab, position, Quaternion.identity);
            // Optionally: Play build sound, animation, etc.
            if (PlayerResources.Instance.Resources < selectedTower.cost)
                UpdateTowerPreviewColor(false);
        }
        else
        {
            UpdateTowerPreviewColor(false);
        }
    }

    // Only copy the main tower sprite and the range indicator sprite
    private void CreateSimpleTowerPreviewFromPrefab()
    {
        DestroyTowerPreview();
        previewRangeRenderer = null;
        if (selectedTower == null || selectedTower.towerPrefab == null) return;
        towerPreviewInstance = new GameObject("Preview_" + selectedTower.towerPrefab.name);

        SpriteRenderer towerSR = null;
        SpriteRenderer rangeSR = null;
        foreach (var sr in selectedTower.towerPrefab.GetComponentsInChildren<SpriteRenderer>(true))
        {
            if (sr.GetComponent<RangeController>() != null || sr.gameObject.name.ToLower().Contains("range"))
                rangeSR = sr;
            else
                towerSR = sr;
        }

        // Copy tower sprite
        if (towerSR != null)
        {
            var towerObj = new GameObject("TowerSprite");
            towerObj.transform.SetParent(towerPreviewInstance.transform);
            towerObj.transform.localPosition = towerSR.transform.localPosition;
            towerObj.transform.localRotation = towerSR.transform.localRotation;
            towerObj.transform.localScale = towerSR.transform.localScale;
            var previewTowerSR = towerObj.AddComponent<SpriteRenderer>();
            CopySpriteRendererProperties(towerSR, previewTowerSR);
        }

        // Copy range sprite
        if (rangeSR != null)
        {
            var rangeObj = new GameObject("RangeSprite");
            rangeObj.transform.SetParent(towerPreviewInstance.transform);
            rangeObj.transform.localPosition = rangeSR.transform.localPosition;
            rangeObj.transform.localRotation = rangeSR.transform.localRotation;
            rangeObj.transform.localScale = rangeSR.transform.localScale;
            previewRangeRenderer = rangeObj.AddComponent<SpriteRenderer>();
            CopySpriteRendererProperties(rangeSR, previewRangeRenderer);
        }
    }

    private void CopySpriteRendererProperties(SpriteRenderer src, SpriteRenderer dest)
    {
        dest.sprite = src.sprite;
        dest.sortingLayerID = src.sortingLayerID;
        dest.sortingOrder = src.sortingOrder;
        dest.flipX = src.flipX;
        dest.flipY = src.flipY;
        dest.color = src.color;
        dest.material = src.sharedMaterial;
    }

    private void UpdateTowerPreviewColor(bool canAfford)
    {
        if (towerPreviewInstance == null) return;
        if (previewRangeRenderer != null)
        {
            if (!canAfford)
            {
                previewRangeRenderer.color = unaffordableColor;
            }
        }
    }

    private void DestroyTowerPreview()
    {
        if (towerPreviewInstance != null)
        {
            Destroy(towerPreviewInstance);
            towerPreviewInstance = null;
        }
    }
}
