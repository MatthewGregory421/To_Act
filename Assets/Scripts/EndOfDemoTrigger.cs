using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using FMODUnity;
using FMOD.Studio;

public class EndOfDemoTrigger : MonoBehaviour
{
    [Header("End Screen")]
    [SerializeField] private GameObject endScreen;

    [Header("Outro Video")]
    [SerializeField] private GameObject blackBackground;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string videoFileName = "Closer.mp4";

    [Header("Cutscene Audio")]
    [SerializeField] private CutscenesAudioManager cutscenesAudioManager;

    private const string MusicBusPath =
        "bus:/Mix Buss/Music";

    private bool triggered;
    private bool returningToMenu;
    private bool videoFinished;
    private bool cutsceneAudioStarted;

    private PlayerInput playerInput;
    private Rigidbody2D playerRigidbody;

    private PauseMenu pauseMenu;

    private bool previousRigidbodySimulated;
    private bool pauseMenuWasEnabled;

    private Bus musicBus;
    private bool musicWasPaused;

    private void Start()
    {
        // =========================
        // INITIAL UI STATE
        // =========================

        // Congratulations screen should not
        // appear until the video has finished.
        if (endScreen != null)
        {
            endScreen.SetActive(false);
        }

        // Video display should not appear
        // until the player reaches the portal.
        if (blackBackground != null)
        {
            blackBackground.SetActive(false);
        }
    }

    private void Update()
    {
        if (!triggered || videoFinished)
            return;

        // =========================
        // CUTSCENE SAFETY LOCK
        // =========================

        // Keep the game frozen.
        if (Time.timeScale != 0f)
        {
            Time.timeScale = 0f;
        }

        // Keep player controls disabled.
        if (playerInput != null &&
            playerInput.enabled)
        {
            playerInput.enabled = false;
        }

        // Keep player physics disabled.
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
        {
            playerInput.enabled = false;
        }

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
        // PAUSE GAME MUSIC
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
            {
                musicWasPaused = true;
            }
            else
            {
                Debug.LogWarning(
                    "Could not pause Music bus: " +
                    result
                );
            }
        }
        else
        {
            Debug.LogWarning(
                "Could not find FMOD Music bus."
            );
        }

        if (blackBackground != null)
        {
            blackBackground.SetActive(true);
        }

        // =========================
        // GET CUTSCENE AUDIO MANAGER
        // =========================

        /*
         * Since your CutscenesManager is underneath
         * BlackBackground, we search AFTER turning
         * BlackBackground on.
         */

        if (cutscenesAudioManager == null)
        {
            cutscenesAudioManager =
                FindFirstObjectByType<CutscenesAudioManager>(
                    FindObjectsInactive.Include
                );
        }

        if (cutscenesAudioManager == null)
        {
            Debug.LogWarning(
                "Could not find CutscenesAudioManager."
            );
        }

        // =========================
        // START OUTRO
        // =========================

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

        // Make sure the VideoPlayer itself is enabled.
        if (!videoPlayer.enabled)
        {
            videoPlayer.enabled = true;
        }

        // The VideoPlayer is outside the hidden
        // BlackBackground, so it should always
        // remain active in the hierarchy.
        if (!videoPlayer.gameObject.activeInHierarchy)
        {
            Debug.LogError(
                "Outro VideoPlayer GameObject is inactive."
            );

            FinishOutro();
            return;
        }

        // Video continues while Time.timeScale = 0.
        videoPlayer.timeUpdateMode =
            VideoTimeUpdateMode.UnscaledGameTime;

        // =========================
        // VIDEO FILE
        // =========================

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

        // =========================
        // VIDEO AUDIO
        // =========================

        // Unity Audio is disabled.
        // FMOD handles the cutscene sound.
        videoPlayer.audioOutputMode =
            VideoAudioOutputMode.None;

        // =========================
        // VIDEO SETTINGS
        // =========================

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
        Debug.Log(
            "Outro video prepared."
        );

        // =========================
        // START FMOD CLOSER AUDIO
        // =========================

        if (cutscenesAudioManager != null)
        {
            cutscenesAudioManager
                .PlayCloserCutscene();

            cutsceneAudioStarted = true;
        }
        else
        {
            Debug.LogWarning(
                "Outro prepared, but CutscenesAudioManager is missing."
            );
        }

        // Start video immediately after starting FMOD.
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

        // If the video breaks, don't trap
        // the player forever.
        FinishOutro();
    }

    private void FinishOutro()
    {
        if (videoFinished)
            return;

        videoFinished = true;

        // =========================
        // STOP VIDEO
        // =========================

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }

        // =========================
        // STOP FMOD CUTSCENE AUDIO
        // =========================

        StopCutsceneAudio();

        // =========================
        // HIDE VIDEO
        // =========================

        if (blackBackground != null)
        {
            blackBackground.SetActive(false);
        }

        // =========================
        // SHOW CONGRATULATIONS
        // =========================

        if (endScreen != null)
        {
            endScreen.SetActive(true);
        }

        /*
         * IMPORTANT:
         *
         * We intentionally keep:
         *
         * Time.timeScale = 0
         * PlayerInput disabled
         * Rigidbody2D disabled
         * PauseMenu disabled
         * Game music paused
         *
         * because the demo is finished.
         *
         * The player now only interacts
         * with the EndScreen button.
         */
    }

    public void ReturnToMainMenu()
    {
        if (returningToMenu)
            return;

        returningToMenu = true;

        UISFXManager.Instance?.PlayUIConfirm();

        // =========================
        // STOP CUTSCENE AUDIO
        // =========================

        StopCutsceneAudio();

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
        // RESTORE PLAYER PHYSICS
        // =========================

        if (playerRigidbody != null)
        {
            playerRigidbody.simulated =
                previousRigidbodySimulated;
        }

        // =========================
        // RESTORE PLAYER INPUT
        // =========================

        if (playerInput != null)
        {
            playerInput.enabled = true;
        }

        // Scene transition needs normal time.
        Time.timeScale = 1f;

        // Hide congratulations screen.
        if (endScreen != null)
        {
            endScreen.SetActive(false);
        }

        // =========================
        // LOAD MAIN MENU
        // =========================

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

    private void StopCutsceneAudio()
    {
        if (!cutsceneAudioStarted)
            return;

        if (cutscenesAudioManager != null)
        {
            cutscenesAudioManager
                .StopCutscene();
        }

        cutsceneAudioStarted = false;
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

        StopCutsceneAudio();
    }
}