using UnityEngine;
using UnityEngine.Serialization;
using FMODUnity;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public StudioEventEmitter Music;

    [SerializeField, ParamRef] private string musicSelector = null;

    [FormerlySerializedAs("musicState")]
    [SerializeField, ParamRef]
    private string musicIntensity = "Music Intensity";

    [SerializeField, ParamRef] private string Dead = null;

    private int currentMusicSelector = -1;
    private int currentMusicIntensity = -1;

    private int enemiesInDetectionRange = 0;
    private bool playerIsAtOneHealth = false;

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
        RefreshMusicIntensity();
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

    private void RefreshMusicIntensity()
    {
        // Highest priority: player is at 1 HP.
        if (playerIsAtOneHealth)
        {
            SetMusicIntensity(MusicIntensity.Tense);
            return;
        }

        // Second priority: player is near at least one enemy.
        if (enemiesInDetectionRange > 0)
        {
            SetMusicIntensity(MusicIntensity.Energetic);
            return;
        }

        // Default state.
        SetMusicIntensity(MusicIntensity.Neutral);
    }

    public void SetPlayerAtOneHealth(bool isAtOneHealth)
    {
        if (playerIsAtOneHealth == isAtOneHealth)
            return;

        playerIsAtOneHealth = isAtOneHealth;
        RefreshMusicIntensity();
    }

    public void EnterEnemyDetectionRange()
    {
        enemiesInDetectionRange++;
        RefreshMusicIntensity();
    }

    public void ExitEnemyDetectionRange()
    {
        enemiesInDetectionRange = Mathf.Max(
            0,
            enemiesInDetectionRange - 1
        );

        RefreshMusicIntensity();
    }

    public void SetMusic(MusicArea area)
    {
        SetMusicArea((int)area);

        // Re-check health and combat instead of always forcing Neutral.
        RefreshMusicIntensity();
    }

    public void MenuMusicSelect() => SetMusic(MusicArea.Menu);
    public void HubMusicSelect() => SetMusic(MusicArea.Hub);
    public void AngerMusicSelect() => SetMusic(MusicArea.Anger);
    public void SadnessMusicSelect() => SetMusic(MusicArea.Sadness);
    public void JoyMusicSelect() => SetMusic(MusicArea.Joy);

    public void DeathStingerSelect()
    {
        RuntimeManager.StudioSystem.setParameterByName(Dead, 1);
    }
}