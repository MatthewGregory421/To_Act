using System.Collections;
using UnityEngine;

public class BootstrapController : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;

        if (!SaveManager.Instance.HasRequest())
        {
            StartNewGame();
            yield break;
        }

        if (SaveManager.Instance.pendingMode == SaveManager.SaveMode.NewGame)
        {
            StartNewGame();
        }
        else
        {
            StartLoadGame();
        }
    }

    private void StartNewGame()
    {
        StartCoroutine(NewGameRoutine());
    }

    private IEnumerator NewGameRoutine()
    {
        SaveManager.Instance.Clear();

        if (WorldStateManager.Instance != null)
        {
            WorldStateManager.Instance.ResetWorldStateForNewGame();
        }

        yield return FadeManager.Instance.FadeOut();

        SceneTransitionManager.Instance.TransitionToScene(
            "TutorialScene",
            "Tutorial_Spawn"
        );

        while (SceneTransitionManager.Instance.IsTransitioning)
            yield return null;

        yield return FadeManager.Instance.FadeIn();
    }

    private void StartLoadGame()
    {
        StartCoroutine(LoadRoutine());
    }

    private IEnumerator LoadRoutine()
    {
        int slot = SaveManager.Instance.pendingSlot;
        SaveData data = SaveManager.Instance.LoadGame(slot);

        if (data == null)
        {
            StartNewGame();
            yield break;
        }

        WorldStateManager.Instance.ApplySave(data);

        SceneTransitionManager.Instance.RespawnAtBench(data.sceneName, data.benchID);

        while (SceneTransitionManager.Instance.IsTransitioning)
            yield return null;

        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();

        if (player == null)
        {
            Debug.LogWarning("Load failed: player missing.");
            SaveManager.Instance.Clear();
            yield break;
        }

        PlayerMovementInputSystem pm = player.GetComponent<PlayerMovementInputSystem>();

        if (pm != null)
        {
            pm.hasShield = data.hasShield;
            pm.hasGroundSlam = data.hasGroundSlam;
            pm.UpdateAbilityUI();
        }

        Debug.Log("Loaded save at bench: " + data.benchID);

        SaveManager.Instance.Clear();
    }
}