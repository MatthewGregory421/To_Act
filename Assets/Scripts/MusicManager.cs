using UnityEngine;
using FMODUnity;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public StudioEventEmitter Music;

   [SerializeField]
   [ParamRef]
   private string musicSelector = null;

   [SerializeField]
   [ParamRef]
   private string Dead = null;

    private int currentMusicState = -1;

    public enum MusicState
    {
        Menu,
        Hub,
        Anger,
        Sadness,
        Joy
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
        if (Music != null && !Music.IsPlaying())
        {
            Music.Play();
        }
    }

    // =========================
    // CORE MUSIC SWITCH
    // =========================
    private void SetMusicState(int state)
    {
        Debug.Log($"[MusicManager] Requested state: {state}, Current state: {currentMusicState}");

        if (currentMusicState == state)
        {
            Debug.Log($"[MusicManager] Staying on same music state: {state}");
            return;
        }

        Debug.Log($"[MusicManager] Switching music state: {currentMusicState} - {state}");

        currentMusicState = state;

        RuntimeManager.StudioSystem.setParameterByName(musicSelector, state);

        Debug.Log($"[MusicManager] Now playing state: {currentMusicState}");
    }

    // =========================
    // PUBLIC API
    // =========================
    public void SetMusic(MusicState state)
    {
        Debug.Log($"[MusicManager] SetMusic called with enum: {state}");

        SetMusicState((int)state);
    }

    public void MenuMusicSelect() => SetMusicState(0);
    public void HubMusicSelect() => SetMusicState(1);
    public void AngerMusicSelect() => SetMusicState(2);
    public void SadnessMusicSelect() => SetMusicState(3);
    public void JoyMusicSelect() => SetMusicState(4);

    public void DeathStingerSelect()
    {
        RuntimeManager.StudioSystem.setParameterByName(Dead, 1);
    }
}