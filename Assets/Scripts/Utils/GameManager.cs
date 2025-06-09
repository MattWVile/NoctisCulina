using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsGamePaused { get; private set; } = true;

    [SerializeField] private GameObject startScreenUI;
    [SerializeField] private GameObject scoreUI;      // Add this line
    [SerializeField] private GameObject waveTimerUI;  // Add this line

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

        PauseGame();
    }

    private void Update()
    {
        if (IsGamePaused && Input.GetKeyDown(KeyCode.Space))
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        IsGamePaused = true;

        if (startScreenUI != null)
        {
            startScreenUI.SetActive(true);
        }
        if (scoreUI != null)
        {
            scoreUI.SetActive(false); // Hide score when paused
        }
        if (waveTimerUI != null)
        {
            waveTimerUI.SetActive(false); // Hide timer when paused
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
        EnableGameUI();
    }
    public void EnableGameUI()
    {
        if (scoreUI != null)
        {
            scoreUI.SetActive(true); // Show score when resumed
        }
        if (waveTimerUI != null)
        {
            waveTimerUI.SetActive(true); // Show timer when resumed
        }
    }
}
