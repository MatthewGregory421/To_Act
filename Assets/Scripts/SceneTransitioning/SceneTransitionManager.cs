using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

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

    public void TransitionToScene(string targetScene, string spawnID)
    {
        if (isTransitioning)
            return;

        StartCoroutine(TransitionRoutine(targetScene, spawnID));
    }

    private void FindPlayerReferences()
    {
        if (player == null)
        {
            PlayerMovementInputSystem movementScript =
                FindFirstObjectByType<PlayerMovementInputSystem>();

            if (movementScript != null)
            {
                player = movementScript.transform;
                playerMovement = movementScript;
            }
        }
    }

    private void LockPlayer()
    {
        FindPlayerReferences();

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

            if (rb != null)
                rb.linearVelocity = Vector2.zero;
        }
    }

    private void UnlockPlayer()
    {
        FindPlayerReferences();

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (player != null)
        {
            PlayerMovementInputSystem movement =
                player.GetComponent<PlayerMovementInputSystem>();

            if (movement != null)
                movement.enabled = true;

            PlayerInput input =
                player.GetComponent<PlayerInput>();

            if (input != null)
                input.enabled = true;

            PlayerCombatInputSystem combat =
                player.GetComponent<PlayerCombatInputSystem>();

            if (combat != null)
                combat.enabled = true;

            Rigidbody2D rb =
                player.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.simulated = true;
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    private IEnumerator TransitionRoutine(string targetScene, string spawnID)
    {
        isTransitioning = true;

        LockPlayer();

        yield return FadeManager.Instance.FadeOut();

        AsyncOperation op = SceneManager.LoadSceneAsync(
            targetScene,
            LoadSceneMode.Single
        );

        while (!op.isDone)
            yield return null;

        Scene newScene = SceneManager.GetSceneByName(targetScene);
        SceneManager.SetActiveScene(newScene);

        HandleMusicForScene(targetScene);

        yield return null;

        FindPlayerReferences();

        SpawnPoint[] spawns = FindObjectsByType<SpawnPoint>(
            FindObjectsSortMode.None
        );

        bool foundSpawn = false;

        foreach (var sp in spawns)
        {
            if (sp.spawnID == spawnID)
            {
                if (player != null)
                {
                    player.position =
                        sp.transform.position + Vector3.up * spawnYOffset;
                }

                foundSpawn = true;
                break;
            }
        }

        if (!foundSpawn)
        {
            Debug.LogWarning($"Spawn ID not found: {spawnID}");
        }

        yield return FadeManager.Instance.FadeIn();

        UnlockPlayer();

        isTransitioning = false;
    }

    public IEnumerator LoadSceneDirect(string sceneName)
    {
        if (isTransitioning)
            yield break;

        isTransitioning = true;

        LockPlayer();

        yield return FadeManager.Instance.FadeOut();

        AsyncOperation op = SceneManager.LoadSceneAsync(
            sceneName,
            LoadSceneMode.Single
        );

        while (!op.isDone)
            yield return null;

        SceneManager.SetActiveScene(
            SceneManager.GetSceneByName(sceneName)
        );

        yield return null;

        yield return FadeManager.Instance.FadeIn();

        if (sceneName != "MainMenu")
            UnlockPlayer();

        isTransitioning = false;
    }

    public IEnumerator LoadMainMenuWithFade()
    {
        if (isTransitioning)
            yield break;

        isTransitioning = true;

        LockPlayer();

        yield return FadeManager.Instance.FadeOut();

        AsyncOperation op = SceneManager.LoadSceneAsync(
            "MainMenu",
            LoadSceneMode.Single
        );

        while (!op.isDone)
            yield return null;

        SceneManager.SetActiveScene(
            SceneManager.GetSceneByName("MainMenu")
        );

        player = null;
        playerMovement = null;

        HandleMusicForScene("MainMenu");

        yield return null;

        yield return FadeManager.Instance.FadeIn();

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

        LockPlayer();

        yield return FadeManager.Instance.FadeOut();

        AsyncOperation op = SceneManager.LoadSceneAsync(
            targetScene,
            LoadSceneMode.Single
        );

        while (!op.isDone)
            yield return null;

        Scene newScene = SceneManager.GetSceneByName(targetScene);
        SceneManager.SetActiveScene(newScene);

        HandleMusicForScene(targetScene);

        yield return null;

        FindPlayerReferences();

        Bench bench = BenchUtility.FindBench(benchID);

        if (bench != null && player != null)
        {
            player.position =
                bench.transform.position + Vector3.up * spawnYOffset;
        }
        else
        {
            Debug.LogWarning($"Bench/player not found for respawn: {benchID}");
        }

        yield return FadeManager.Instance.FadeIn();

        UnlockPlayer();

        isTransitioning = false;
    }

    private void HandleMusicForScene(string sceneName)
    {
        if (MusicManager.Instance == null)
        {
            Debug.LogWarning("[Music] MusicManager instance missing!");
            return;
        }

        MusicManager.MusicState state = sceneName switch
        {
            "MainMenu" => MusicManager.MusicState.Menu,
            "Hub" => MusicManager.MusicState.Hub,
            "Anger_1" => MusicManager.MusicState.Anger,
            "Sadness_1" => MusicManager.MusicState.Sadness,
            "Sadness_2" => MusicManager.MusicState.Sadness,
            "Joy_1" => MusicManager.MusicState.Joy,
            _ => MusicManager.MusicState.Hub
        };

        MusicManager.Instance.SetMusic(state);
    }
}