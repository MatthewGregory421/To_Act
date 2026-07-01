using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class SplashScreen : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string mainMenuScene = "MainMenu";

    private bool loading = false;

    void Update()
    {
        if (loading)
            return;

        if (Input.anyKeyDown ||
            Input.GetMouseButtonDown(0) ||
            Input.GetMouseButtonDown(1) ||
            Input.GetMouseButtonDown(2))
        {
            LoadMainMenu();
        }
    }

    private void LoadMainMenu()
    {
        loading = true;

#if UNITY_WEBGL && !UNITY_EDITOR
        RuntimeManager.CoreSystem.mixerSuspend();
        RuntimeManager.CoreSystem.mixerResume();
#endif

        SceneManager.LoadScene(mainMenuScene);
    }
}