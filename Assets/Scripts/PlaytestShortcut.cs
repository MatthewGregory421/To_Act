using UnityEngine;
using UnityEngine.InputSystem;

public class PlaytestSadnessShortcut : MonoBehaviour
{
    [SerializeField] private Key shortcutKey = Key.Backquote;

    [Header("Scene Transition")]
    [SerializeField] private string targetScene = "Sadness_1";
    [SerializeField] private string spawnID = "Sadness_Spawn";

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current[shortcutKey].wasPressedThisFrame)
        {
            if (SceneTransitionManager.Instance != null)
            {
                Debug.Log($"Playtest shortcut: loading {targetScene} at {spawnID}");
                SceneTransitionManager.Instance.TransitionToScene(targetScene, spawnID);
            }
            else
            {
                Debug.LogError("Playtest shortcut failed: SceneTransitionManager.Instance is missing.");
            }
        }
    }
}