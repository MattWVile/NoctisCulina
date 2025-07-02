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
    [SerializeField, Range(0f, 1f)] private float previewAlpha = 0.2f;
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
        previewRangeRenderer = null;
        if (selectedTower != null && selectedTower.towerPrefab != null)
        {
            // Create an empty GameObject for the preview
            previewInstance = new GameObject("Preview_" + selectedTower.towerPrefab.name);
            // Copy all SpriteRenderers from the prefab (including children)
            var prefabRenderers = selectedTower.towerPrefab.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var prefabRenderer in prefabRenderers)
            {
                // Create a child GameObject for each sprite
                GameObject child = new GameObject(prefabRenderer.gameObject.name);
                child.transform.SetParent(previewInstance.transform);
                child.transform.localPosition = prefabRenderer.transform.localPosition;
                child.transform.localRotation = prefabRenderer.transform.localRotation;
                child.transform.localScale = prefabRenderer.transform.localScale;
                var sr = child.AddComponent<SpriteRenderer>();
                sr.sprite = prefabRenderer.sprite;
                sr.sortingLayerID = prefabRenderer.sortingLayerID;
                sr.sortingOrder = prefabRenderer.sortingOrder;
                sr.flipX = prefabRenderer.flipX;
                sr.flipY = prefabRenderer.flipY;
                sr.color = prefabRenderer.color;
                sr.material = prefabRenderer.sharedMaterial;
                // If this is the range indicator, store a reference
                if (prefabRenderer.GetComponent<RangeController>() != null || prefabRenderer.gameObject.name.ToLower().Contains("range"))
                {
                    previewRangeRenderer = sr;
                }
            }
        }
    }

    private void UpdatePreviewColor(bool canAfford)
    {
        if (previewInstance == null) return;
        if (previewRangeRenderer != null)
        {
            if (canAfford)
            {
                // Restore original color (no tint)
                // (Assume the color is already correct from CreatePreview)
            }
            else
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
