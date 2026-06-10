using UnityEngine;

public class SceneTransitionTrigger : MonoBehaviour
{
    public string targetScene;
    public string targetSpawnID;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        SceneTransitionManager.Instance.TransitionToScene(targetScene, targetSpawnID);
    }
} 