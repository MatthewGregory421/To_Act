using UnityEngine;

public class TestingBootstrapController : MonoBehaviour
{
    private void Start()
    {
        // Start first real gameplay scene
        SceneTransitionManager.Instance.StartGame("PlayerTestingScene", "Testing_Spawn");
    }
}
