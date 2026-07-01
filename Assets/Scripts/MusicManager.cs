using UnityEngine;
using FMODUnity;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public StudioEventEmitter Music;

    [SerializeField, ParamRef] private string musicSelector = null;
    [SerializeField, ParamRef] private string musicState = null;
    [SerializeField, ParamRef] private string Dead = null;

    private int currentMusicSelector = -1;
    private int currentEnergyState = -1;

    public enum MusicArea
    {
        Menu,
        Hub,
        Anger,
        Sadness,
        Joy
    }

    public enum EnergyState
    {
        Neutral = 0,
        MostTense = 1,
        MostEnergetic = 2
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

        // Default for WebGL/main menu
        SetEnergyState(EnergyState.MostEnergetic);
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

        RuntimeManager.StudioSystem.setParameterByName(musicSelector, area);

        currentMusicSelector = area;
    }

    private void SetEnergyState(EnergyState state)
    {
        EnsureMusicPlaying();

        int value = (int)state;

        if (currentEnergyState == value)
            return;

        RuntimeManager.StudioSystem.setParameterByName(musicState, value);

        currentEnergyState = value;
    }

    public void SetMusic(MusicArea area)
    {
        SetMusicArea((int)area);
        SetEnergyState(EnergyState.MostEnergetic);
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