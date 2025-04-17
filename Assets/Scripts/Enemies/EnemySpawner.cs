using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject zombiePrefab; // Reference to the zombie prefab
    public float spawnInterval = 5.0f; // Time interval between spawns
    private float timer;

    void Start()
    {
        timer = spawnInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SpawnZombie();
            timer = spawnInterval;
        }
    }

    void SpawnZombie()
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

        // Instantiate the zombie prefab at the spawn position
        Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
    }
}
