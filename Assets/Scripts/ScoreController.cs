using UnityEngine;
using UnityEngine.UI; // Add this line if using Text

public class ScoreController : MonoBehaviour
{
    public static ScoreController Instance { get; private set; }

    public int score = 0;
    public int highScore = 0;

    // Reference to the Text or TextMeshProUGUI component
    public Text scoreText; // Use this if using Text
    // public TextMeshProUGUI scoreText; // Use this if using TextMeshProUGUI

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep the ScoreController across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int amount)
    {
        if (PlayerHealthController.Instance != null && PlayerHealthController.Instance.IsDead)
        {
            return; // Prevent score addition if the player is dead
        }
        score += amount;
        if (score > highScore)
        {
            highScore = score;
        }
        UpdateScoreText();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
        else
        {
            Debug.LogError("Score text component is not assigned.");
        }
    }
}
