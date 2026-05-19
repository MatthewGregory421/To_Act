using UnityEngine;
using FMODUnity;

public class EnemySFXManager : MonoBehaviour
{
    public StudioEventEmitter EnemySFX;

    [SerializeField]
    [ParamRef]
    private string EnemySelector = null;

    [SerializeField]
    [ParamRef]
    private string EnemyActionSelector = null;

    [SerializeField]
    [ParamRef]
    private string EnemySpecialSelector = null;

    
    //AngerSFX
    public void PlayAngerIdle() 
    {
        RuntimeManager.StudioSystem.setParameterByName(EnemySelector, 0);
        RuntimeManager.StudioSystem.setParameterByName(EnemyActionSelector, 0);
        EnemySFX.Play();
    }
    public void PlayAngerAttack() 
    {
        RuntimeManager.StudioSystem.setParameterByName(EnemySelector, 0);
        RuntimeManager.StudioSystem.setParameterByName(EnemyActionSelector, 1);
        EnemySFX.Play();
    }

    public void PlayAngerDamage() 
    {
        RuntimeManager.StudioSystem.setParameterByName(EnemySelector, 0);
        RuntimeManager.StudioSystem.setParameterByName(EnemyActionSelector, 2);
        EnemySFX.Play();
    }

    public void PlayAngerSpecial() 
    {
        RuntimeManager.StudioSystem.setParameterByName(EnemySelector, 0);
        RuntimeManager.StudioSystem.setParameterByName(EnemyActionSelector, 3);
        EnemySFX.Play();
    }

    //Sadness SFX

    public void PlaySadnessIdle() 
    {
        RuntimeManager.StudioSystem.setParameterByName(EnemySelector, 1);
        RuntimeManager.StudioSystem.setParameterByName(EnemyActionSelector, 0);
        EnemySFX.Play();
    }
    public void PlaySadnessAttack() 
    {
        RuntimeManager.StudioSystem.setParameterByName(EnemySelector, 1);
        RuntimeManager.StudioSystem.setParameterByName(EnemyActionSelector, 1);
        EnemySFX.Play();
    }

    public void PlaySadnessDamage() 
    {
        RuntimeManager.StudioSystem.setParameterByName(EnemySelector, 1);
        RuntimeManager.StudioSystem.setParameterByName(EnemyActionSelector, 2);
        EnemySFX.Play();
    }

    public void PlaySadnessSpecialActive() 
    {
        RuntimeManager.StudioSystem.setParameterByName(EnemySelector, 1);
        RuntimeManager.StudioSystem.setParameterByName(EnemyActionSelector, 3);
        RuntimeManager.StudioSystem.setParameterByName(EnemySpecialSelector, 0);
        EnemySFX.Play();
    }

    public void PlaySadnessSpecialConnect() 
    {
        RuntimeManager.StudioSystem.setParameterByName(EnemySelector, 1);
        RuntimeManager.StudioSystem.setParameterByName(EnemyActionSelector, 3);
        RuntimeManager.StudioSystem.setParameterByName(EnemySpecialSelector, 1);
        EnemySFX.Play();
    }

    public void PlaySadnessSpecialDeactive() 
    {
        RuntimeManager.StudioSystem.setParameterByName(EnemySelector, 1);
        RuntimeManager.StudioSystem.setParameterByName(EnemyActionSelector, 3);
        RuntimeManager.StudioSystem.setParameterByName(EnemySpecialSelector, 2);
        EnemySFX.Play();
    }


     //Sadness Joy

    public void PlayJoyIdle() 
    {
        RuntimeManager.StudioSystem.setParameterByName(EnemySelector, 2);
        RuntimeManager.StudioSystem.setParameterByName(EnemyActionSelector, 0);
        EnemySFX.Play();
    }
    public void PlayJoyAttack() 
    {
        RuntimeManager.StudioSystem.setParameterByName(EnemySelector, 2);
        RuntimeManager.StudioSystem.setParameterByName(EnemyActionSelector, 1);
        EnemySFX.Play();
    }

    public void PlayJoyDamage() 
    {
        RuntimeManager.StudioSystem.setParameterByName(EnemySelector, 2);
        RuntimeManager.StudioSystem.setParameterByName(EnemyActionSelector, 2);
        EnemySFX.Play();
    }

    public void PlayJoySpecialActive() 
    {
        RuntimeManager.StudioSystem.setParameterByName(EnemySelector, 2);
        RuntimeManager.StudioSystem.setParameterByName(EnemyActionSelector, 3);
        RuntimeManager.StudioSystem.setParameterByName(EnemySpecialSelector, 0);
        EnemySFX.Play();
    }

    public void PlayJoySpecialConnect() 
    {
        RuntimeManager.StudioSystem.setParameterByName(EnemySelector, 2);
        RuntimeManager.StudioSystem.setParameterByName(EnemyActionSelector, 3);
        RuntimeManager.StudioSystem.setParameterByName(EnemySpecialSelector, 1);
        EnemySFX.Play();
    }

    public void PlayJoySpecialDeactive() 
    {
        RuntimeManager.StudioSystem.setParameterByName(EnemySelector, 2);
        RuntimeManager.StudioSystem.setParameterByName(EnemyActionSelector, 3);
        RuntimeManager.StudioSystem.setParameterByName(EnemySpecialSelector, 2);
        EnemySFX.Play();
    }
}
