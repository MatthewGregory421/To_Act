using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    public Transform player;

    private string currentScene;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Called by Bootstrap
    public void StartGame(string firstScene, string spawnPointID)
    {
        StartCoroutine(LoadFirstScene(firstScene, spawnPointID));
    }

    private IEnumerator LoadFirstScene(string sceneName, string spawnID)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!op.isDone)
            yield return null;

        Scene newScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(newScene);

        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();

        foreach (var sp in spawnPoints)
        {
            if (sp.spawnID == spawnID)
            {
                player.position = sp.transform.position;
                break;
            }
        }

        currentScene = sceneName;
    }

    public void TransitionToScene(string targetScene, string spawnPointID)
    {
        StartCoroutine(TransitionRoutine(targetScene, spawnPointID));
    }

    private IEnumerator TransitionRoutine(string targetScene, string spawnPointID)
    {
        string oldScene = currentScene;

        AsyncOperation op = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);

        while (!op.isDone)
            yield return null;

        Scene newScene = SceneManager.GetSceneByName(targetScene);
        SceneManager.SetActiveScene(newScene);

        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();

        foreach (var sp in spawnPoints)
        {
            if (sp.spawnID == spawnPointID)
            {
                player.position = sp.transform.position;
                break;
            }
        }

        if (!string.IsNullOrEmpty(oldScene))
        {
            SceneManager.UnloadSceneAsync(oldScene);
        }

        currentScene = targetScene;
    }
}