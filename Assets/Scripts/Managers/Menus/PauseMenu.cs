using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject pauseMenuUI;
    public GameObject optionsPanel;

    private bool isPaused = false;

    [Header("Buttons")]
    public Button respawnButton;

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

        UpdateButtonStates();

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
        string sceneName = WorldStateManager.Instance.GetCurrentScene();
        string benchID = WorldStateManager.Instance.GetCurrentBench();

        if (string.IsNullOrEmpty(sceneName) || string.IsNullOrEmpty(benchID))
        {
            Debug.LogWarning("Cannot respawn: no bench has been activated yet.");
            UISFXManager.Instance?.PlayUIBack();
            return;
        }

        UISFXManager.Instance?.PlayUIConfirm();

        // Reset enemies like a bench rest
        WorldStateManager.Instance.RespawnEnemiesFromBench();

        Time.timeScale = 1f;

        isPaused = false;
        pauseMenuUI.SetActive(false);

        SceneTransitionManager.Instance.RespawnAtBench(sceneName, benchID);
    }

    public void OpenOptions()
    {
        UISFXManager.Instance?.PlayUIConfirm();

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (optionsPanel != null)
            optionsPanel.SetActive(true);
    }

    private void UpdateButtonStates()
    {
        if (respawnButton == null)
            return;

        string sceneName = WorldStateManager.Instance.GetCurrentScene();
        string benchID = WorldStateManager.Instance.GetCurrentBench();

        bool hasBench =
            !string.IsNullOrEmpty(sceneName) &&
            !string.IsNullOrEmpty(benchID);

        respawnButton.interactable = hasBench;
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

        SceneTransitionManager.Instance.StartCoroutine(
            SceneTransitionManager.Instance.LoadMainMenuWithFade()
        );
    }
}