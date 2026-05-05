using UnityEngine;
using FMODUnity;

public class UISFXManager : MonoBehaviour
{
    public StudioEventEmitter Open;
    public StudioEventEmitter Close;
    public StudioEventEmitter Confirm;
    public StudioEventEmitter Forward;
    public StudioEventEmitter Back;

    public void PlayOpen()
    {
        Open.Play();
    }

    public void PlayClose()
    {
        Close.Play();
    }

    public void PlayConfirm()
    {
        Confirm.Play();
    }

    public void PlayForward()
    {
        Forward.Play();
    }

    public void PlayBack()
    {
        Back.Play();
    }
}
