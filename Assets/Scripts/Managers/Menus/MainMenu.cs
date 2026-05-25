using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public UISFXManager uiSFXManager;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;

    // Loads the main game
    public void PlayGame()
    {
        uiSFXManager.PlayUIConfirm();
        SceneManager.LoadScene("Bootstrap");
    }

    // Loads the weekly testing scene
    public void LoadTestingScene()
    {
        uiSFXManager.PlayUIConfirm();
        SceneManager.LoadScene("PlayerTestingScene");
    }

    // Opens the options menu
    public void OpenOptions()
    {
        uiSFXManager.PlayUIOpenMenu();
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    // Quits the application
    public void QuitGame()
    {
        uiSFXManager.PlayUIConfirm();
        Debug.Log("Quit Game");
        Application.Quit();
    }
}