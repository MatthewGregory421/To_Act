using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;

    // Loads the main game
    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    // Loads the weekly testing scene
    public void LoadTestingScene()
    {
        SceneManager.LoadScene("PlayerTestingScene");
    }

    // Opens the options menu
    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    // Quits the application
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}