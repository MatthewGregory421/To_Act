using UnityEngine;
using FMODUnity;

public class UISFXManager : MonoBehaviour
{
    public StudioEventEmitter UISFX;

    [SerializeField]
    [ParamRef]
    private string UISelector = null;

    public static UISFXManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayUIBack() 
    {
        RuntimeManager.StudioSystem.setParameterByName(UISelector, 0);
        UISFX.Play();
    }

    public void PlayUICloseMenu() 
    {
        RuntimeManager.StudioSystem.setParameterByName(UISelector, 1);
        UISFX.Play();
    }

    public void PlayUIConfirm() 
    {
        RuntimeManager.StudioSystem.setParameterByName(UISelector, 2);
        UISFX.Play();
    }

    public void PlayUIForward() 
    {
        RuntimeManager.StudioSystem.setParameterByName(UISelector, 3);
        UISFX.Play();
    }

    public void PlayUIOpenMenu() 
    {
        RuntimeManager.StudioSystem.setParameterByName(UISelector, 4);
        UISFX.Play();
    }
}
