using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("References")]
    public Transform player;
    public MonoBehaviour playerMovement;

    private string currentScene;

    private bool isTransitioning;

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
        isTransitioning = true;

        if (playerMovement != null)
            playerMovement.enabled = false;

        yield return FadeManager.Instance.FadeOut();

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

        yield return FadeManager.Instance.FadeIn();

        if (playerMovement != null)
            playerMovement.enabled = true;

        isTransitioning = false;
    }

    public void TransitionToScene(string targetScene, string spawnPointID)
    {
        if (isTransitioning)
            return;

        StartCoroutine(TransitionRoutine(targetScene, spawnPointID));
    }

    private IEnumerator TransitionRoutine(string targetScene, string spawnPointID)
    {
        isTransitioning = true;

        if (playerMovement != null)
            playerMovement.enabled = false;

        yield return FadeManager.Instance.FadeOut();

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

        yield return FadeManager.Instance.FadeIn();

        if (playerMovement != null)
            playerMovement.enabled = true;

        isTransitioning = false;
    }

    public IEnumerator RespawnToBench(string targetScene, Vector3 targetPosition)
    {
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (currentScene != targetScene)
        {
            string oldScene = currentScene;

            AsyncOperation op =
                SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);

            while (!op.isDone)
                yield return null;

            Scene newScene = SceneManager.GetSceneByName(targetScene);
            SceneManager.SetActiveScene(newScene);

            if (!string.IsNullOrEmpty(oldScene))
            {
                yield return SceneManager.UnloadSceneAsync(oldScene);
            }

            currentScene = targetScene;
        }

        player.position = targetPosition;

        if (playerMovement != null)
            playerMovement.enabled = true;
    }

    public string GetCurrentScene()
    {
        return currentScene;
    }
}