using UnityEngine;

public class BootstrapController : MonoBehaviour
{
    private void Start()
    {
        // Start first real gameplay scene
        SceneTransitionManager.Instance.StartGame("TutorialScene", "Tutorial_Spawn");
    }
}