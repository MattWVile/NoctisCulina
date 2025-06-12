using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public GameObject zombossPrefab;
    public float spawnInterval = 5.0f;
    public float toggleTimer = 50.0f;
    public KeyCode toggleKey = KeyCode.T;

    private float spawnTimer;
    private float toggleTimerCountdown;
    public bool spawningEnabled = true;

    public Text waveTimerText;

    // Zomboss spawn tracking
    private int zombossesSpawnedThisRound = 0;
    private int zombossesDiedThisRound = 0;
    private bool waitingForFirstZomboss = false;
    private float firstZombossTimer = 0f;
    private bool waitingForSecondZomboss = false;
    private float secondZombossTimer = 0f;

    private void OnEnable()
    {
        EventBus.Subscribe<ZombossDiedEvent>(OnZombossDiedEvent);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ZombossDiedEvent>(OnZombossDiedEvent);
    }

    void Start()
    {
        ResetSpawner();
    }

    void Update()
    {
        // Handle manual toggle with a key press
        //if (Input.GetKeyDown(toggleKey))
        //{
        //    spawningEnabled = !spawningEnabled;
        //    Debug.Log("Spawning toggled: " + (spawningEnabled ? "Enabled" : "Disabled"));
        //}

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
            toggleTimerCountdown = toggleTimer;

            // New round: reset Zomboss counters and set up first Zomboss spawn
            zombossesSpawnedThisRound = 0;
            zombossesDiedThisRound = 0;
            waitingForSecondZomboss = false;
            firstZombossTimer = Random.Range(5f, 10f);
            waitingForFirstZomboss = true;
        }

        // Only spawn enemies if spawning is enabled
        if (spawningEnabled)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                SpawnZombie();
                spawnTimer = spawnInterval;
            }
        }

        // Handle delayed first Zomboss spawn at round start
        if (waitingForFirstZomboss)
        {
            firstZombossTimer -= Time.deltaTime;
            if (firstZombossTimer <= 0)
            {
                SpawnZomboss();
                waitingForFirstZomboss = false;
            }
        }

        // Handle delayed second Zomboss spawn
        if (waitingForSecondZomboss)
        {
            secondZombossTimer -= Time.deltaTime;
            if (secondZombossTimer <= 0)
            {
                SpawnZomboss();
                waitingForSecondZomboss = false;
            }
        }
    }

    void SpawnZombie()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
    }

    void SpawnZomboss()
    {
        if (zombossesSpawnedThisRound < 2)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            Instantiate(zombossPrefab, spawnPosition, Quaternion.identity);
            zombossesSpawnedThisRound++;
        }
    }

    // Event handler for Zomboss death
    private void OnZombossDiedEvent(ZombossDiedEvent evt)
    {
        zombossesDiedThisRound++;
        if (zombossesDiedThisRound == 1 && zombossesSpawnedThisRound == 1)
        {
            // First Zomboss died, start timer for second
            waitingForSecondZomboss = true;
            secondZombossTimer = Random.Range(15f, 30f);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        Camera mainCamera = Camera.main;
        Vector3 cameraPosition = mainCamera.transform.position;
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        Vector3 spawnPosition = new Vector3();
        int side = Random.Range(0, 4);

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
    public void ResetSpawner()
    {
        // Reset timers and state
        spawnTimer = spawnInterval;
        toggleTimerCountdown = toggleTimer;
        zombossesSpawnedThisRound = 0;
        zombossesDiedThisRound = 0;
        waitingForSecondZomboss = false;
        waitingForFirstZomboss = true;
        firstZombossTimer = Random.Range(5f, 10f);
        secondZombossTimer = 0f;
        spawningEnabled = true;

        // Reset wave timer UI
        if (waveTimerText != null)
            waveTimerText.text = "Wave Timer: " + Mathf.Ceil(toggleTimerCountdown).ToString();

        // Despawn all enemies
        DespawnAllEnemies();
    }

    private void DespawnAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Zombie");
        foreach (var enemy in enemies)
        {
            Destroy(enemy);
        }

        GameObject[] zombosses = GameObject.FindGameObjectsWithTag("Zomboss");
        foreach (var zomboss in zombosses)
        {
            Destroy(zomboss);
        }
    }
}
