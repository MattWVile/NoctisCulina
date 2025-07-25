using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsGamePaused { get; private set; } = true;
    public bool IsBuildMode { get; private set; } = false; // Build mode flag

    [Header("Player")]
    public GameObject playerPrefab; // Assign your player prefab in the Inspector
    public Vector3 playerSpawnPosition = Vector3.zero; // Set your default spawn position

    [SerializeField] private SpriteRenderer playerLightCone;

    [SerializeField] private GameObject startScreenUI;
    [SerializeField] private GameObject scoreUI;
    [SerializeField] private GameObject waveTimerUI;
    [SerializeField] private GameObject PauseScreenUI;

    private void Start()
    {
        if (playerLightCone == null)
        {
            playerLightCone = GameObject.FindGameObjectWithTag("PlayerLightCone").GetComponent<SpriteRenderer>();
        }
    }
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadStartScreen();
    }

    private void Update()
    {
        HandlePauseResumeInput();
        HandleBuildModeToggleInput();
    }

    private void HandlePauseResumeInput()
    {
        if (IsGamePaused && Input.GetKeyDown(KeyCode.Space))
        {
            ResumeGame();
        }
        else if (!IsGamePaused && Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    private void HandleBuildModeToggleInput()
    {
        if (!IsGamePaused && Input.GetKeyDown(KeyCode.B))
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
        Debug.Log("Entered Build Mode");
        if (BuildManager.Instance != null)
            BuildManager.Instance.SetBuildMode(true);
        TogglePlayerLightCone(false);
        EnableGameUI(false); // Hide score and wave timer UI during build mode
    }

    public void ExitBuildMode()
    {
        IsBuildMode = false;
        if (BuildManager.Instance != null)
        {
            BuildManager.Instance.SetBuildMode(false);
            TogglePlayerLightCone(true);
        }
        EnableGameUI(true); // Restore score and wave timer UI after build mode
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        IsGamePaused = true;

        if (PauseScreenUI != null)
        {
            PauseScreenUI.SetActive(true);
        }
        EnableGameUI(false);
    }

    public void LoadStartScreen()
    {
        Time.timeScale = 0f;
        IsGamePaused = true;

        if (startScreenUI != null)
        {
            startScreenUI.SetActive(true);
        }
        EnableGameUI(false);

        if (PauseScreenUI != null)
        {
            PauseScreenUI.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        IsGamePaused = false;

        if (startScreenUI != null)
        {
            startScreenUI.SetActive(false);
        }
        if (PauseScreenUI != null)
        {
            PauseScreenUI.SetActive(false);
        }
        EnableGameUI(true);
    }

    public void EnableGameUI(bool enabled)
    {
        if (scoreUI != null)
        {
            scoreUI.SetActive(enabled);
        }
        if (waveTimerUI != null)
        {
            waveTimerUI.SetActive(enabled);
        }
    }

    public void FullRestart()
    {
        GameObject oldPlayer = GameObject.FindGameObjectWithTag("Player");
        if (oldPlayer != null)
        {
            Destroy(oldPlayer);
        }

        GameObject newPlayer = null;
        if (playerPrefab != null)
        {
            newPlayer = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
        }

        // Set the new player in the camera follow script
        CameraFollow cameraFollow = FindObjectOfType<CameraFollow>();
        if (cameraFollow != null && newPlayer != null)
        {
            cameraFollow.SetPlayer(newPlayer.transform);
        }        

        // Reset score
        if (ScoreController.Instance != null)
            ScoreController.Instance.ResetScore();


        EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
        if (enemySpawner != null)
            enemySpawner.ResetSpawner();

        // Hide all UI except start screen
        EnableGameUI(false);

        if (startScreenUI != null)
            startScreenUI.SetActive(true);

        if (PauseScreenUI != null)
            PauseScreenUI.SetActive(false);

        // Pause the game
        Time.timeScale = 0f;
    }
}
