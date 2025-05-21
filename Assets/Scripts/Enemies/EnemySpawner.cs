using UnityEngine;
using UnityEngine.UI; // Add this line if using Text

public class EnemySpawner : MonoBehaviour
{
    public GameObject zombiePrefab; // Reference to the zombie prefab
    public float spawnInterval = 5.0f; // Time interval between spawns
    public float toggleTimer = 50.0f; // Time interval to automatically toggle spawning
    public KeyCode toggleKey = KeyCode.T; // Key to manually toggle spawning

    private float spawnTimer;
    private float toggleTimerCountdown;
    public bool spawningEnabled = true; // Flag to control whether spawning is enabled

    public Text waveTimerText; // Use this if using Text

    void Start()
    {
        spawnTimer = spawnInterval;
        toggleTimerCountdown = toggleTimer;
    }

    void Update()
    {
        // Handle manual toggle with a key press
        if (Input.GetKeyDown(toggleKey))
        {
            spawningEnabled = !spawningEnabled;
            Debug.Log("Spawning toggled: " + (spawningEnabled ? "Enabled" : "Disabled"));
        }

        // Handle automatic toggle on a timer
        toggleTimerCountdown -= Time.deltaTime;
        if (waveTimerText != null)
        {
            waveTimerText.text = "Wave Timer: " + Mathf.Ceil(toggleTimerCountdown).ToString();
        }
        if (toggleTimerCountdown <= 0)
        {
            spawningEnabled = !spawningEnabled;
            Debug.Log("Spawning automatically toggled: " + (spawningEnabled ? "Enabled" : "Disabled"));
            toggleTimerCountdown = toggleTimer; // Reset the toggle timer
        }

        // Only spawn enemies if spawning is enabled
        if (spawningEnabled)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                SpawnZombie();
                spawnTimer = spawnInterval; // Reset the spawn timer
            }
        }
    }

    void SpawnZombie()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
    }

    Vector3 GetRandomSpawnPosition()
    {
        // Get the camera bounds
        Camera mainCamera = Camera.main;
        Vector3 cameraPosition = mainCamera.transform.position;
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Determine spawn position outside the camera bounds
        Vector3 spawnPosition = new Vector3();
        int side = Random.Range(0, 4); // 0 = top, 1 = bottom, 2 = left, 3 = right

        switch (side)
        {
            case 0: // Top
                spawnPosition = new Vector3(
                    Random.Range(cameraPosition.x - cameraWidth / 2, cameraPosition.x + cameraWidth / 2),
                    cameraPosition.y + cameraHeight / 2 + 1,
                    0);
                break;
            case 1: // Bottom
                spawnPosition = new Vector3(
                    Random.Range(cameraPosition.x - cameraWidth / 2, cameraPosition.x + cameraWidth / 2),
                    cameraPosition.y - cameraHeight / 2 - 1,
                    0);
                break;
            case 2: // Left
                spawnPosition = new Vector3(
                    cameraPosition.x - cameraWidth / 2 - 1,
                    Random.Range(cameraPosition.y - cameraHeight / 2, cameraPosition.y + cameraHeight / 2),
                    0);
                break;
            case 3: // Right
                spawnPosition = new Vector3(
                    cameraPosition.x + cameraWidth / 2 + 1,
                    Random.Range(cameraPosition.y - cameraHeight / 2, cameraPosition.y + cameraHeight / 2),
                    0);
                break;
        }

        return spawnPosition;
    }
}