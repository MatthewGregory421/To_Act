using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("Player References")]
    public Transform player;
    public MonoBehaviour playerMovement;

    [SerializeField] private bool isTransitioning;
    public bool IsTransitioning => isTransitioning;

    [Header("Spawn Offset")]
    [SerializeField] private float spawnYOffset = 0.5f;

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

    // =========================
    // PUBLIC ENTRY POINT (USE THIS ALWAYS)
    // =========================
    public void TransitionToScene(string targetScene, string spawnID)
    {
        if (isTransitioning)
            return;

        StartCoroutine(TransitionRoutine(targetScene, spawnID));
    }

    // =========================
    // MAIN TRANSITION FLOW
    // =========================
    private IEnumerator TransitionRoutine(string targetScene, string spawnID)
    {
        isTransitioning = true;

        // LOCK PLAYER
        if (playerMovement != null)
            playerMovement.enabled = false;

        Rigidbody2D rb = player ? player.GetComponent<Rigidbody2D>() : null;
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // FADE OUT
        yield return FadeManager.Instance.FadeOut();

        // LOAD SCENE
        AsyncOperation op = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);

        while (!op.isDone)
            yield return null;

        Scene newScene = SceneManager.GetSceneByName(targetScene);
        SceneManager.SetActiveScene(newScene);

        HandleMusicForScene(targetScene);

        // WAIT ONE FRAME (important for spawn objects to exist)
        yield return null;

        // SPAWN PLAYER
        SpawnPoint[] spawns = FindObjectsByType<SpawnPoint>(
            FindObjectsSortMode.None
        );

        bool foundSpawn = false;

        foreach (var sp in spawns)
        {
            if (sp.spawnID == spawnID)
            {
                player.position = sp.transform.position + Vector3.up * spawnYOffset;
                foundSpawn = true;
                break;
            }
        }

        if (!foundSpawn)
        {
            Debug.LogWarning($"Spawn ID not found: {spawnID}");
        }

        // STOP PHYSICS AFTER TELEPORT
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // FADE IN
        yield return FadeManager.Instance.FadeIn();

        // UNLOCK PLAYER
        if (playerMovement != null)
            playerMovement.enabled = true;

        isTransitioning = false;
    }

    // =========================
    // DIRECT LOAD (ONLY USE FOR SPECIAL CASES LIKE BOOTSTRAP)
    // =========================
    public IEnumerator LoadSceneDirect(string sceneName)
    {
        if (isTransitioning)
            yield break;

        isTransitioning = true;

        if (playerMovement != null)
            playerMovement.enabled = false;

        yield return FadeManager.Instance.FadeOut();

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        while (!op.isDone)
            yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        yield return null;

        yield return FadeManager.Instance.FadeIn();

        if (playerMovement != null)
            playerMovement.enabled = true;

        isTransitioning = false;
    }

    public void RespawnAtBench(string targetScene, string benchID)
    {
        if (isTransitioning)
            return;

        StartCoroutine(RespawnAtBenchRoutine(targetScene, benchID));
    }

    private IEnumerator RespawnAtBenchRoutine(string targetScene, string benchID)
    {
        isTransitioning = true;

        if (playerMovement != null)
            playerMovement.enabled = false;

        Rigidbody2D rb = player ? player.GetComponent<Rigidbody2D>() : null;
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        yield return FadeManager.Instance.FadeOut();

        AsyncOperation op = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);

        while (!op.isDone)
            yield return null;

        Scene newScene = SceneManager.GetSceneByName(targetScene);
        SceneManager.SetActiveScene(newScene);

        HandleMusicForScene(targetScene);

        yield return null;

        Bench bench = BenchUtility.FindBench(benchID);

        if (bench != null)
        {
            player.position = bench.transform.position + Vector3.up * spawnYOffset;
            Debug.Log($"Respawned player at bench: {benchID}");
        }
        else
        {
            Debug.LogWarning($"Bench not found for respawn: {benchID}");
        }

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        yield return FadeManager.Instance.FadeIn();

        if (playerMovement != null)
            playerMovement.enabled = true;

        isTransitioning = false;
    }

    private void HandleMusicForScene(string sceneName)
    {
        if (MusicManager.Instance == null)
        {
            Debug.LogWarning("[Music] MusicManager instance missing!");
            return;
        }

        Debug.Log($"[Music] Scene triggered music check: {sceneName}");

        MusicManager.MusicState state = sceneName switch
        {
            "MainMenu" => MusicManager.MusicState.Menu,
            "Hub" => MusicManager.MusicState.Hub,

            "Anger_1" => MusicManager.MusicState.Anger,

            "Sadness_1" => MusicManager.MusicState.Sadness,
            "Sadness_2" => MusicManager.MusicState.Sadness, // SAME STATE = NO RESTART

            "Joy_1" => MusicManager.MusicState.Joy,

            _ => MusicManager.MusicState.Hub
        };

        Debug.Log($"[Music] Resolved state: {state} for scene: {sceneName}");

        MusicManager.Instance.SetMusic(state);
    }
}