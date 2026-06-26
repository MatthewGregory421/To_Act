using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string mainMenuScene = "MainMenu";

    private bool loading = false;

    void Update()
    {
        if (loading)
            return;

        // Any keyboard key
        if (Input.anyKeyDown)
        {
            LoadMainMenu();
            return;
        }

        // Mouse buttons
        if (Input.GetMouseButtonDown(0) ||
            Input.GetMouseButtonDown(1) ||
            Input.GetMouseButtonDown(2))
        {
            LoadMainMenu();
        }
    }

    private void LoadMainMenu()
    {
        loading = true;
        SceneManager.LoadScene(mainMenuScene);
    }
}