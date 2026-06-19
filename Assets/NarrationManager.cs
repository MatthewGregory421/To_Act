using UnityEngine;
using FMODUnity;

public class NarrationManager : MonoBehaviour
{
    [SerializeField] private EventReference narrationEvent;
    private FMOD.Studio.EventInstance instance;

    public void PlayNarration(int index)
    {
        Debug.Log("Playing narration: " + index);

        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        instance.release();

        instance = RuntimeManager.CreateInstance(narrationEvent);

        instance.setParameterByName("narrationSelector", index);
        instance.start();
    }
}