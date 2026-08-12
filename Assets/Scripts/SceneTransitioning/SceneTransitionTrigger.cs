using FMODUnity;
using UnityEngine;

public class SceneTransitionTrigger : MonoBehaviour
{
    public string targetScene;
    public string targetSpawnID;

    private bool hasTriggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;

        SceneTransitionManager.Instance.TransitionToScene(
            targetScene,
            targetSpawnID
        );
    }
}