using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private void Update()
    {
        if (GameManager.Instance.IsGamePaused)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                Resume();

            if (Input.GetKeyDown(KeyCode.T))
                Restart();

            if (Input.GetKeyDown(KeyCode.Q))
                ExitGame();
        }
    }

    public void Resume()
    {
        GameManager.Instance.ResumeGame();
    }

    public void Pause()
    {
        GameManager.Instance.PauseGame();
    }

    public void Restart()
    {
        GameManager.Instance.FullRestart();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Time.timeScale = 1f;
    }
}