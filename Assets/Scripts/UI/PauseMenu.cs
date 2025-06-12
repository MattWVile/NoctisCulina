using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool isPaused = true;

    private void Update()
    {

        if (isPaused)
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
        isPaused = false;
        Time.timeScale = 1f;
        GameManager.Instance.ResumeGame();
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        isPaused = false;
        GameManager.Instance.FullRestart();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
