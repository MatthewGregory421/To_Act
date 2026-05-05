using UnityEngine;
using UnityEngine.SceneManagement; 

public class SceneLoader : MonoBehaviour
{
    public MusicManager musicManager;
    public UISFXManager uiSFXManager;
    public void LoadScene()
    {
        musicManager.SilenceMusEmitter();
        SceneManager.LoadScene("TestScene");
    }
    // From Here is set up for later use once I fully flesh out the level design
    public void GoToHubProto()
    {
        musicManager.MusAliveFO();
        musicManager.SilenceMusEmitter();
        SceneManager.LoadScene("Hub Proto");
    }

    public void GoToAngerProto()
    {
        musicManager.MusAliveFO();
        musicManager.SilenceMusEmitter();
        SceneManager.LoadScene("Anger Proto");
    }

    public void GoToSadnessProto()
    {
        musicManager.MusAliveFO();
        musicManager.SilenceMusEmitter();
        SceneManager.LoadScene("Sadness Proto");
    }
}
