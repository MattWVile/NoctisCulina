using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }

    [Header("Tower Build Data")]
    public TowerData[] availableTowers;

    private TowerData selectedTower;
    private bool isBuildMode = false;
    private GameObject previewInstance;
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
            DestroyPreview();
        }
    }

    public void SelectTowerToBuild(TowerData tower)
    {
        selectedTower = tower;
        CreateSimplePreviewFromPrefab();
    }

    private void Update()
    {
        if (!isBuildMode)
        {
            DestroyPreview();
            return;
        }

        HandleTowerSelectionInput();
        UpdatePreviewPositionAndColor();
        HandleTowerPlacementInput();
    }

    private void HandleTowerSelectionInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && availableTowers.Length > 0)
        {
            SelectTowerToBuild(availableTowers[0]);
        }
    }

    private void UpdatePreviewPositionAndColor()
    {
        if (previewInstance == null) return;
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        previewInstance.transform.position = mouseWorldPos;
        bool canAfford = PlayerResources.Instance != null && selectedTower != null && PlayerResources.Instance.Resources >= selectedTower.cost;
        UpdatePreviewColor(canAfford);
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
                UpdatePreviewColor(false);
        }
        else
        {
            UpdatePreviewColor(false);
        }
    }

    // Only copy the main tower sprite and the range indicator sprite
    private void CreateSimplePreviewFromPrefab()
    {
        DestroyPreview();
        previewRangeRenderer = null;
        if (selectedTower == null || selectedTower.towerPrefab == null) return;
        previewInstance = new GameObject("Preview_" + selectedTower.towerPrefab.name);

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
            towerObj.transform.SetParent(previewInstance.transform);
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
            rangeObj.transform.SetParent(previewInstance.transform);
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

    private void UpdatePreviewColor(bool canAfford)
    {
        if (previewInstance == null) return;
        if (previewRangeRenderer != null)
        {
            if (!canAfford)
            {
                previewRangeRenderer.color = unaffordableColor;
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
