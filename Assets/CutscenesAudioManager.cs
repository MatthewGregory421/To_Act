using UnityEngine;
using FMODUnity;

public class CutscenesAudioManager : MonoBehaviour
{
    public StudioEventEmitter CutscenesManager;

    [SerializeField]
    [ParamRef]
    private string CutsceneSelection = null;

    private void PlaySafe()
    {
        if (CutscenesManager == null)
            return;

        try
        {
            CutscenesManager.Play();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(
                "CutscenesManager failed: " + e.Message
            );
        }
    }

    public void PlayOpenerCutscene()
    {
        RuntimeManager.StudioSystem.setParameterByName(
            CutsceneSelection,
            0
        );

        PlaySafe();
    }

    public void PlayCloserCutscene()
    {
        RuntimeManager.StudioSystem.setParameterByName(
            CutsceneSelection,
            1
        );

        PlaySafe();
    }

    public void StopCutscene()
    {
        if (CutscenesManager == null)
            return;

        try
        {
            CutscenesManager.Stop();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(
                "CutscenesManager stop failed: " + e.Message
            );
        }
    }
}