using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public UISFXManager uiSFXManager;

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
        uiSFXManager.PlayUIOpenMenu();

        pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;

        isPaused = true;
    }

    // =========================
    // RESUME
    // =========================
    public void ResumeGame()
    {
        uiSFXManager.PlayUICloseMenu();

        pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;

        isPaused = false;
    }

    // =========================
    // RELOAD LAST SAVE
    // =========================
    public void ReloadLastSave()
    {
        uiSFXManager.PlayUIConfirm();

        Time.timeScale = 1f;

        // Replace this with your future save system
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // =========================
    // QUIT TO MAIN MENU
    // =========================
    public void QuitToMainMenu()
    {
        uiSFXManager.PlayUIConfirm();

        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}