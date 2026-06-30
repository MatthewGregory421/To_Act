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

        if (Music != null && !Music.IsPlaying())
        {
            Music.Play();
        }

        RuntimeManager.StudioSystem.setParameterByName(musicSelector, state);

        if (currentMusicState == state)
        {
            return;
        }


        currentMusicState = state;

    }

    // =========================
    // PUBLIC API
    // =========================
    public void SetMusic(MusicState state)
    {

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