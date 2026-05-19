using UnityEngine;
using FMODUnity;

public class MusicManager : MonoBehaviour
{
   public StudioEventEmitter Music;

   [SerializeField]
   [ParamRef]
   private string musicSelector = null;

   [SerializeField]
   [ParamRef]
   private string Dead = null;


    public void PlayMusic()
    {
        Music.Play();
    }

    public void MenuMusicSelect()
    {
        RuntimeManager.StudioSystem.setParameterByName(musicSelector, 0);
    }

     public void HubMusicSelect()
    {
        RuntimeManager.StudioSystem.setParameterByName(musicSelector, 1);
    }

     public void AngerMusicSelect()
    {
        RuntimeManager.StudioSystem.setParameterByName(musicSelector, 2);
    }

     public void SadnessMusicSelect()
    {
        RuntimeManager.StudioSystem.setParameterByName(musicSelector, 3);
    }

     public void DeathStingerSelect()
    {
        RuntimeManager.StudioSystem.setParameterByName(Dead, 1);
    }
}
