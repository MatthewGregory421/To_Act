using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using FMODUnity;
using FMOD.Studio;

public class IntroCutsceneController : MonoBehaviour
{
    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string videoFileName = "Opener.mp4";

    [Header("Cutscene Audio")]
    [SerializeField] private CutscenesAudioManager cutscenesAudioManager;

    private const string MusicBusPath =
        "bus:/Mix Buss/Music";

    private PlayerInput playerInput;
    private Rigidbody2D playerRigidbody;
    private PauseMenu pauseMenu;

    private Bus musicBus;

    private float previousTimeScale;

    private bool previousRigidbodySimulated;
    private bool pauseMenuWasEnabled;

    private bool musicWasPaused;
    private bool hasFinished;
    private bool cutsceneAudioStarted;

    private void Awake()
    {
        // =========================
        // FIND CUTSCENE AUDIO MANAGER
        // =========================

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
                "IntroCutsceneController could not find CutscenesAudioManager."
            );
        }

        // =========================
        // FIND PLAYER
        // =========================

        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerInput =
                player.GetComponent<PlayerInput>();

            playerRigidbody =
                player.GetComponent<Rigidbody2D>();

            if (playerInput != null)
            {
                playerInput.enabled = false;
            }
            else
            {
                Debug.LogWarning(
                    "Player was found but has no PlayerInput."
                );
            }

            if (playerRigidbody != null)
            {
                previousRigidbodySimulated =
                    playerRigidbody.simulated;

                playerRigidbody.simulated = false;
            }
            else
            {
                Debug.LogWarning(
                    "Player was found but has no Rigidbody2D."
                );
            }
        }
        else
        {
            Debug.LogWarning(
                "IntroCutsceneController could not find the Player."
            );
        }

        // =========================
        // DISABLE PAUSE MENU
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

        previousTimeScale = Time.timeScale;
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
    }

    private void Start()
    {
        PlayIntro();
    }

    private void Update()
    {
        if (hasFinished)
            return;

        // Keep gameplay completely locked
        // for the entire cutscene.

        if (Time.timeScale != 0f)
        {
            Time.timeScale = 0f;
        }

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

    private void PlayIntro()
    {
        // Video continues despite Time.timeScale = 0.
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
            "Intro video URL: " +
            videoPlayer.url
        );

        // =========================
        // VIDEO AUDIO
        // =========================

        // Unity Audio is disabled.
        // FMOD handles all cutscene audio.
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
            "Intro video prepared."
        );

        // =========================
        // START FMOD CUTSCENE AUDIO
        // =========================

        if (cutscenesAudioManager != null)
        {
            cutscenesAudioManager
                .PlayOpenerCutscene();

            cutsceneAudioStarted = true;
        }

        // Start the video immediately after
        // starting the FMOD audio.
        source.Play();
    }

    private void OnVideoFinished(
        VideoPlayer source
    )
    {
        FinishIntro();
    }

    private void OnVideoError(
        VideoPlayer source,
        string message
    )
    {
        Debug.LogError(
            "Intro video error: " +
            message
        );

        FinishIntro();
    }

    private void FinishIntro()
    {
        if (hasFinished)
            return;

        hasFinished = true;

        // =========================
        // STOP VIDEO
        // =========================

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }

        // =========================
        // STOP CUTSCENE AUDIO
        // =========================

        StopCutsceneAudio();

        // =========================
        // RESTORE MUSIC
        // =========================

        RestoreMusic();

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

        // =========================
        // RESTORE PAUSE MENU
        // =========================

        if (pauseMenu != null)
        {
            pauseMenu.enabled =
                pauseMenuWasEnabled;
        }

        // =========================
        // RESTORE GAME TIME
        // =========================

        Time.timeScale =
            previousTimeScale;

        // Remove entire cutscene canvas.
        Destroy(gameObject);
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

        // Emergency cleanup if something destroys
        // the cutscene before it ends normally.
        if (!hasFinished)
        {
            StopCutsceneAudio();
            RestoreMusic();

            if (playerRigidbody != null)
            {
                playerRigidbody.simulated =
                    previousRigidbodySimulated;
            }

            if (playerInput != null)
            {
                playerInput.enabled = true;
            }

            if (pauseMenu != null)
            {
                pauseMenu.enabled =
                    pauseMenuWasEnabled;
            }

            Time.timeScale =
                previousTimeScale;
        }
    }
}