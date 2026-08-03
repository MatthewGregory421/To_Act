using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using FMODUnity;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public StudioEventEmitter Music;

    [SerializeField, ParamRef]
    private string musicSelector = null;

    [FormerlySerializedAs("musicState")]
    [SerializeField, ParamRef]
    private string musicIntensity = "Music Intensity";

    [SerializeField, ParamRef]
    private string Dead = null;

    [Header("Intensity Timing")]
    [Tooltip(
        "How long the enemy range condition must remain unchanged " +
        "before switching between Neutral and Energetic."
    )]
    [SerializeField, Min(0f)]
    private float intensityConfirmationDelay = 1f;

    private int currentMusicSelector = -1;
    private int currentMusicIntensity = -1;

    private int enemiesInDetectionRange;
    private bool playerIsAtOneHealth;

    private Coroutine intensityConfirmationCoroutine;
    private MusicIntensity? pendingMusicIntensity;

    public enum MusicArea
    {
        Menu,
        Hub,
        Anger,
        Sadness,
        Joy
    }

    public enum MusicIntensity
    {
        Tense = 1,
        Neutral = 2,
        Energetic = 3
    }

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

    private void Start()
    {
        EnsureMusicPlaying();

        // Initial state is applied immediately.
        RefreshMusicIntensity(true);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void EnsureMusicPlaying()
    {
        if (Music != null && !Music.IsPlaying())
        {
            Music.Play();
        }
    }

    private void SetMusicArea(int area)
    {
        EnsureMusicPlaying();

        if (currentMusicSelector == area)
            return;

        RuntimeManager.StudioSystem.setParameterByName(
            musicSelector,
            area
        );

        currentMusicSelector = area;
    }

    private void SetMusicIntensity(MusicIntensity intensity)
    {
        EnsureMusicPlaying();

        int value = (int)intensity;

        if (currentMusicIntensity == value)
            return;

        RuntimeManager.StudioSystem.setParameterByName(
            musicIntensity,
            value
        );

        currentMusicIntensity = value;
    }

    private void RefreshMusicIntensity(
        bool skipNeutralEnergeticConfirmation = false
    )
    {
        // Tense always has immediate priority.
        if (playerIsAtOneHealth)
        {
            CancelPendingIntensityChange();
            SetMusicIntensity(MusicIntensity.Tense);
            return;
        }

        MusicIntensity desiredIntensity =
            GetDesiredNonTenseIntensity();

        bool hasNoCurrentIntensity =
            currentMusicIntensity == -1;

        bool isLeavingTense =
            currentMusicIntensity == (int)MusicIntensity.Tense;

        // Initial setup and transitions involving Tense happen immediately.
        if (skipNeutralEnergeticConfirmation ||
            hasNoCurrentIntensity ||
            isLeavingTense)
        {
            CancelPendingIntensityChange();
            SetMusicIntensity(desiredIntensity);
            return;
        }

        RequestConfirmedIntensityChange(desiredIntensity);
    }

    private MusicIntensity GetDesiredNonTenseIntensity()
    {
        if (enemiesInDetectionRange > 0)
        {
            return MusicIntensity.Energetic;
        }

        return MusicIntensity.Neutral;
    }

    private void RequestConfirmedIntensityChange(
        MusicIntensity desiredIntensity
    )
    {
        // We have returned to the current state before the pending
        // transition was confirmed.
        if (currentMusicIntensity == (int)desiredIntensity)
        {
            CancelPendingIntensityChange();
            return;
        }

        // This exact transition is already being checked.
        if (intensityConfirmationCoroutine != null &&
            pendingMusicIntensity == desiredIntensity)
        {
            return;
        }

        CancelPendingIntensityChange();

        pendingMusicIntensity = desiredIntensity;

        intensityConfirmationCoroutine = StartCoroutine(
            ConfirmIntensityChange(desiredIntensity)
        );
    }

    private IEnumerator ConfirmIntensityChange(
        MusicIntensity requestedIntensity
    )
    {
        yield return new WaitForSeconds(
            intensityConfirmationDelay
        );

        // Tense may have activated while we were waiting.
        if (playerIsAtOneHealth)
        {
            ClearPendingIntensityChange();
            yield break;
        }

        // Check the enemy range condition for a second time.
        MusicIntensity currentDesiredIntensity =
            GetDesiredNonTenseIntensity();

        if (currentDesiredIntensity == requestedIntensity)
        {
            SetMusicIntensity(requestedIntensity);
        }

        ClearPendingIntensityChange();
    }

    private void CancelPendingIntensityChange()
    {
        if (intensityConfirmationCoroutine != null)
        {
            StopCoroutine(intensityConfirmationCoroutine);
        }

        ClearPendingIntensityChange();
    }

    private void ClearPendingIntensityChange()
    {
        intensityConfirmationCoroutine = null;
        pendingMusicIntensity = null;
    }

    public void SetPlayerAtOneHealth(bool isAtOneHealth)
    {
        if (playerIsAtOneHealth == isAtOneHealth)
            return;

        playerIsAtOneHealth = isAtOneHealth;

        // Entering or leaving Tense should happen immediately.
        RefreshMusicIntensity(true);
    }

    public void EnterEnemyDetectionRange()
    {
        enemiesInDetectionRange++;

        // Neutral to Energetic requires confirmation.
        RefreshMusicIntensity();
    }

    public void ExitEnemyDetectionRange()
    {
        enemiesInDetectionRange = Mathf.Max(
            0,
            enemiesInDetectionRange - 1
        );

        // Energetic to Neutral requires confirmation.
        RefreshMusicIntensity();
    }

    public void SetMusic(MusicArea area)
    {
        SetMusicArea((int)area);
        RefreshMusicIntensity();
    }

    public void MenuMusicSelect() =>
        SetMusic(MusicArea.Menu);

    public void HubMusicSelect() =>
        SetMusic(MusicArea.Hub);

    public void AngerMusicSelect() =>
        SetMusic(MusicArea.Anger);

    public void SadnessMusicSelect() =>
        SetMusic(MusicArea.Sadness);

    public void JoyMusicSelect() =>
        SetMusic(MusicArea.Joy);

    public void DeathStingerSelect()
    {
        RuntimeManager.StudioSystem.setParameterByName(
            Dead,
            1
        );
    }
}