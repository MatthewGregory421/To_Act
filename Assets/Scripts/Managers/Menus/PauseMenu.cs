using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject pauseMenuUI;

    private bool isPaused = false;

    private void Update()
    {
        // ESC key toggles pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // =========================
    // PAUSE
    // =========================
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;

        isPaused = true;
    }

    // =========================
    // RESUME
    // =========================
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;

        isPaused = false;
    }

    // =========================
    // RELOAD LAST SAVE
    // =========================
    public void ReloadLastSave()
    {
        Time.timeScale = 1f;

        // Replace this with your future save system
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // =========================
    // QUIT TO MAIN MENU
    // =========================
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}