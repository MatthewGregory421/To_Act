using UnityEngine;
using UnityEngine.SceneManagement;

public class HudSceneVisibility : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateVisibility(SceneManager.GetActiveScene());
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateVisibility(scene);
    }

    private void UpdateVisibility(Scene scene)
    {
        bool shouldShow = scene.name != mainMenuSceneName;

        if (canvas != null)
            canvas.enabled = shouldShow;
    }
}
