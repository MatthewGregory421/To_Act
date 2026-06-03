using UnityEngine;
using FMODUnity;

public class NarrationManager : MonoBehaviour
{
    public StudioEventEmitter Narration;

   [SerializeField]
   [ParamRef]
   private string narrationSelector = null;

    // these works for both areas since the area selection is run through the music selector global param
    public void Narration1()
    {
        RuntimeManager.StudioSystem.setParameterByName(narrationSelector, 0);
        Narration.Play();
    }

    public void Narration2()
    {
        RuntimeManager.StudioSystem.setParameterByName(narrationSelector, 1);
        Narration.Play();
    }

    public void Narration3()
    {
        RuntimeManager.StudioSystem.setParameterByName(narrationSelector, 2);
        Narration.Play();
    }

    public void Narration4()
    {
        RuntimeManager.StudioSystem.setParameterByName(narrationSelector, 3);
        Narration.Play();
    }

    public void Narration5()
    {
        RuntimeManager.StudioSystem.setParameterByName(narrationSelector, 4);
        Narration.Play();
    }

    public void Narration6()
    {
        RuntimeManager.StudioSystem.setParameterByName(narrationSelector, 5);
        Narration.Play();
    }

    public void Narration7()
    {
        RuntimeManager.StudioSystem.setParameterByName(narrationSelector, 6);
        Narration.Play();
    }

    public void Narration8()
    {
        RuntimeManager.StudioSystem.setParameterByName(narrationSelector, 7);
        Narration.Play();
    }

    public void Narration9()
    {
        RuntimeManager.StudioSystem.setParameterByName(narrationSelector, 8);
        Narration.Play();
    }

    public void Narration10()
    {
        RuntimeManager.StudioSystem.setParameterByName(narrationSelector, 9);
        Narration.Play();
    }

    public void Narration11()
    {
        RuntimeManager.StudioSystem.setParameterByName(narrationSelector, 10);
        Narration.Play();
    }

    public void Narration12()
    {
        RuntimeManager.StudioSystem.setParameterByName(narrationSelector, 11);
        Narration.Play();
    }

    public void Narration13()
    {
        RuntimeManager.StudioSystem.setParameterByName(narrationSelector, 12);
        Narration.Play();
    }

    public void Narration14()
    {
        RuntimeManager.StudioSystem.setParameterByName(narrationSelector,13);
        Narration.Play();
    }

    public void Narration15()
    {
        RuntimeManager.StudioSystem.setParameterByName(narrationSelector, 14);
        Narration.Play();
    }
}
