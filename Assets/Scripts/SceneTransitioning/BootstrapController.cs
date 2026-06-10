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
        yield return FadeManager.Instance.FadeOut();

        SceneTransitionManager.Instance.TransitionToScene(
            "TutorialScene",
            "Tutorial_Spawn"
        );

        while (SceneTransitionManager.Instance.IsTransitioning)
            yield return null;

        yield return FadeManager.Instance.FadeIn();

        SaveManager.Instance.Clear();
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

        yield return FadeManager.Instance.FadeOut();

        yield return SceneTransitionManager.Instance.LoadSceneDirect(data.sceneName);

        Bench bench = BenchUtility.FindBench(data.benchID);
        PlayerHealth player = FindObjectOfType<PlayerHealth>();

        if (bench != null && player != null)
            player.transform.position = bench.transform.position;

        var pm = player.GetComponent<PlayerMovementInputSystem>();
        pm.hasShield = data.hasShield;
        pm.hasGroundSlam = data.hasGroundSlam;

        yield return FadeManager.Instance.FadeIn();

        SaveManager.Instance.Clear();
    }
}