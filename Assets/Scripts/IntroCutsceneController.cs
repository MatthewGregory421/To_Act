using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using FMODUnity;
using FMOD.Studio;

public class IntroCutsceneController : MonoBehaviour
{
    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private AudioSource videoAudioSource;
    [SerializeField] private string videoFileName = "Opener.mp4";

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

    private void Awake()
    {
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

            // Disable input.
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

            // Completely stop the player's physics.
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
        else
        {
            Debug.LogWarning(
                "IntroCutsceneController could not find PauseMenu."
            );
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

                Debug.Log(
                    "Intro cutscene paused FMOD Music bus."
                );
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
                "Could not find FMOD Music bus: " +
                MusicBusPath
            );
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

        // =========================
        // CUTSCENE SAFETY LOCK
        // =========================

        // If another script tries to unpause the game,
        // immediately force it back to paused.
        if (Time.timeScale != 0f)
        {
            Time.timeScale = 0f;
        }

        // If another script tries to re-enable input,
        // disable it again.
        if (playerInput != null &&
            playerInput.enabled)
        {
            playerInput.enabled = false;
        }

        // Keep physics completely disabled.
        if (playerRigidbody != null &&
            playerRigidbody.simulated)
        {
            playerRigidbody.simulated = false;
        }
    }

    private void PlayIntro()
    {
        // Video continues while game time is frozen.
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
        // AUDIO TRACK
        // =========================

        // Our MP4 contains one AAC audio track.
        //
        // This is especially important for URL-based
        // VideoPlayers because Unity does not know
        // the track count until preparation.
        videoPlayer.controlledAudioTrackCount = 1;

        videoPlayer.EnableAudioTrack(
            0,
            true
        );

#if UNITY_WEBGL && !UNITY_EDITOR

        // Web builds can send the video's audio
        // directly through the browser/platform.
        videoPlayer.audioOutputMode =
            VideoAudioOutputMode.Direct;

        videoPlayer.SetDirectAudioMute(
            0,
            false
        );

        videoPlayer.SetDirectAudioVolume(
            0,
            1f
        );

#else

        // Windows Editor/native backend complained
        // about Direct output, so use an AudioSource.
        videoPlayer.audioOutputMode =
            VideoAudioOutputMode.AudioSource;

        if (videoAudioSource != null)
        {
            videoAudioSource.playOnAwake = false;
            videoAudioSource.loop = false;
            videoAudioSource.mute = false;
            videoAudioSource.volume = 1f;
            videoAudioSource.spatialBlend = 0f;
            videoAudioSource.ignoreListenerPause = true;

            videoPlayer.SetTargetAudioSource(
                0,
                videoAudioSource
            );
        }
        else
        {
            Debug.LogWarning(
                "No Video Audio Source assigned."
            );
        }

#endif

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

    private void OnVideoPrepared(VideoPlayer source)
    {
        Debug.Log("Intro video prepared.");

        Debug.Log(
            "Video audio tracks found: " +
            source.audioTrackCount
        );

        if (source.audioTrackCount > 0)
        {
            Debug.Log(
                "Audio channels: " +
                source.GetAudioChannelCount(0)
            );

            Debug.Log(
                "Audio sample rate: " +
                source.GetAudioSampleRate(0)
            );
        }

        if (videoAudioSource != null)
        {
            // Make absolutely sure the AudioSource
            // can still make sound while gameplay is paused.
            videoAudioSource.enabled = true;
            videoAudioSource.mute = false;
            videoAudioSource.volume = 1f;
            videoAudioSource.spatialBlend = 0f;

            // Important if another system uses AudioListener.pause.
            videoAudioSource.ignoreListenerPause = true;

            Debug.Log(
                "AudioSource enabled: " +
                videoAudioSource.enabled
            );

            Debug.Log(
                "AudioSource muted: " +
                videoAudioSource.mute
            );

            Debug.Log(
                "AudioSource volume: " +
                videoAudioSource.volume
            );

            Debug.Log(
                "AudioListener paused: " +
                AudioListener.pause
            );

            Debug.Log(
                "AudioListener volume: " +
                AudioListener.volume
            );
        }

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

        // Destroy entire intro canvas.
        Destroy(gameObject);
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

        // Emergency cleanup if the canvas gets
        // destroyed without FinishIntro().
        if (!hasFinished)
        {
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