using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using FMODUnity;
using FMOD.Studio;

public class EndOfDemoTrigger : MonoBehaviour
{
    [Header("End UI")]
    [SerializeField] private GameObject endScreen;

    [Header("Outro Cutscene")]
    [SerializeField] private GameObject outroCanvas;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string videoFileName = "Closer.mp4";

    private const string MusicBusPath =
        "bus:/Mix Buss/Music";

    private bool triggered;
    private bool returningToMenu;
    private bool videoFinished;

    private PlayerInput playerInput;
    private Rigidbody2D playerRigidbody;
    private PauseMenu pauseMenu;

    private bool previousRigidbodySimulated;
    private bool pauseMenuWasEnabled;

    private Bus musicBus;
    private bool musicWasPaused;

    private void Start()
    {
        if (endScreen != null)
            endScreen.SetActive(false);

        if (outroCanvas != null)
            outroCanvas.SetActive(false);
    }

    private void Update()
    {
        if (!triggered || videoFinished)
            return;

        // Keep the game locked during the outro.
        if (Time.timeScale != 0f)
            Time.timeScale = 0f;

        if (playerInput != null &&
            playerInput.enabled)
        {
            playerInput.enabled = false;
        }

        if (playerRigidbody != null &&
            playerRigidbody.simulated)
        {
            playerRigidbody.simulated = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        triggered = true;

        BeginOutro(other.gameObject);
    }

    private void BeginOutro(GameObject player)
    {
        // =========================
        // PLAYER
        // =========================

        playerInput =
            player.GetComponent<PlayerInput>();

        playerRigidbody =
            player.GetComponent<Rigidbody2D>();

        if (playerInput != null)
            playerInput.enabled = false;

        if (playerRigidbody != null)
        {
            previousRigidbodySimulated =
                playerRigidbody.simulated;

            playerRigidbody.simulated = false;
        }

        // =========================
        // PAUSE MENU
        // =========================

        pauseMenu =
            FindFirstObjectByType<PauseMenu>(
                FindObjectsInactive.Include
            );

        if (pauseMenu != null)
        {
            pauseMenuWasEnabled =
                pauseMenu.enabled;

            pauseMenu.enabled = false;
        }

        // =========================
        // FREEZE GAME
        // =========================

        Time.timeScale = 0f;

        // =========================
        // PAUSE FMOD MUSIC
        // =========================

        musicBus =
            RuntimeManager.GetBus(
                MusicBusPath
            );

        if (musicBus.isValid())
        {
            FMOD.RESULT result =
                musicBus.setPaused(true);

            if (result == FMOD.RESULT.OK)
                musicWasPaused = true;
        }

        // =========================
        // SHOW CUTSCENE
        // =========================

        if (outroCanvas != null)
            outroCanvas.SetActive(true);

        PlayOutro();
    }

    private void PlayOutro()
    {
        if (videoPlayer == null)
        {
            Debug.LogError(
                "EndOfDemoTrigger has no VideoPlayer assigned."
            );

            FinishOutro();
            return;
        }

        videoPlayer.timeUpdateMode =
            VideoTimeUpdateMode.UnscaledGameTime;

        videoPlayer.source =
            VideoSource.Url;

        string videoPath =
            System.IO.Path.Combine(
                Application.streamingAssetsPath,
                "Videos",
                videoFileName
            );

#if UNITY_WEBGL && !UNITY_EDITOR

        videoPlayer.url = videoPath;

#else

        videoPlayer.url =
            new System.Uri(
                videoPath
            ).AbsoluteUri;

#endif

        Debug.Log(
            "Outro video URL: " +
            videoPlayer.url
        );

        // Unity Audio is disabled in this project.
        // Outro audio can be handled through FMOD later.
        videoPlayer.audioOutputMode =
            VideoAudioOutputMode.None;

        videoPlayer.isLooping = false;
        videoPlayer.playOnAwake = false;

        videoPlayer.prepareCompleted +=
            OnVideoPrepared;

        videoPlayer.loopPointReached +=
            OnVideoFinished;

        videoPlayer.errorReceived +=
            OnVideoError;

        videoPlayer.Prepare();
    }

    private void OnVideoPrepared(
        VideoPlayer source
    )
    {
        source.Play();
    }

    private void OnVideoFinished(
        VideoPlayer source
    )
    {
        FinishOutro();
    }

    private void OnVideoError(
        VideoPlayer source,
        string message
    )
    {
        Debug.LogError(
            "Outro video error: " +
            message
        );

        // Still show the end screen if the video fails.
        FinishOutro();
    }

    private void FinishOutro()
    {
        if (videoFinished)
            return;

        videoFinished = true;

        if (videoPlayer != null)
            videoPlayer.Stop();

        // Remove the video canvas.
        if (outroCanvas != null)
            Destroy(outroCanvas);

        // Show your existing completion screen.
        if (endScreen != null)
            endScreen.SetActive(true);

        /*
         * We deliberately DO NOT:
         *
         * - re-enable the player
         * - turn physics back on
         * - unpause the game
         *
         * because the demo is finished.
         *
         * The player should only be able to use
         * the Return To Main Menu button now.
         */
    }

    public void ReturnToMainMenu()
    {
        if (returningToMenu)
            return;

        returningToMenu = true;

        UISFXManager.Instance?.PlayUIConfirm();

        // =========================
        // RESTORE MUSIC
        // =========================

        RestoreMusic();

        // =========================
        // RESTORE PAUSE MENU
        // =========================

        if (pauseMenu != null)
        {
            pauseMenu.enabled =
                pauseMenuWasEnabled;
        }

        // =========================
        // RESTORE PLAYER
        // =========================

        if (playerRigidbody != null)
        {
            playerRigidbody.simulated =
                previousRigidbodySimulated;
        }

        if (playerInput != null)
        {
            playerInput.enabled = true;
        }

        // Scene transitions need normal game time.
        Time.timeScale = 1f;

        if (endScreen != null)
            endScreen.SetActive(false);

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.StartCoroutine(
                SceneTransitionManager.Instance
                    .LoadMainMenuWithFade()
            );
        }
        else
        {
            Debug.LogWarning(
                "SceneTransitionManager missing. " +
                "Cannot return to MainMenu."
            );
        }
    }

    private void RestoreMusic()
    {
        if (!musicWasPaused)
            return;

        if (!musicBus.isValid())
            return;

        musicBus.setPaused(false);

        musicWasPaused = false;
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -=
                OnVideoPrepared;

            videoPlayer.loopPointReached -=
                OnVideoFinished;

            videoPlayer.errorReceived -=
                OnVideoError;
        }
    }
}