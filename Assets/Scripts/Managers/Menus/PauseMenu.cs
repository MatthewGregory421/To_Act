using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject pauseMenuUI;

    private bool isPaused = false;

    private void Update()
    {
        if (pauseMenuUI == null)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    // =========================
    // PAUSE
    // =========================
    public void PauseGame()
    {
        if (pauseMenuUI == null)
            return;

        Debug.Log("PauseMenu opened");

        UISFXManager.Instance?.PlayUIOpenMenu();

        pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;

        isPaused = true;
    }

    // =========================
    // RESUME
    // =========================
    public void ResumeGame()
    {
        Debug.Log("PauseMneu closed");

        UISFXManager.Instance?.PlayUICloseMenu();

        pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;

        isPaused = false;
    }

    // =========================
    // RELOAD LAST SAVE
    // =========================
    public void ReloadLastSave()
    {
        UISFXManager.Instance?.PlayUIConfirm();

        Time.timeScale = 1f;

        isPaused = false;
        pauseMenuUI.SetActive(false);

        SaveManager.Instance.RequestLoadGame(
            SaveManager.Instance.currentSlot
        );

        SceneManager.LoadScene("Bootstrap");
    }

    // =========================
    // QUIT TO MAIN MENU
    // =========================
    public void QuitToMainMenu()
    {
        UISFXManager.Instance?.PlayUIConfirm();

        Time.timeScale = 1f;

        isPaused = false;
        pauseMenuUI.SetActive(false);

        StartCoroutine(
            SceneTransitionManager.Instance.LoadSceneDirect("MainMenu")
        );
    }
}