using UnityEngine;
using UnityEngine.SceneManagement; 

public class RH_SceneLoader : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene("TestScene");
    }
    // From Here is set up for later use once I fully flesh out the level design
    public void GoToHubProto()
    {
        SceneManager.LoadScene("Hub Proto");
    }

    public void GoToAngerProto()
    {
        SceneManager.LoadScene("Anger Proto");
    }

    public void GoToSadnessProto()
    {
        SceneManager.LoadScene("Sadness Proto");
    }
}
