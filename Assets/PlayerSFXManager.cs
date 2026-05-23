using UnityEngine;
using FMODUnity;

public class PlayerSFXManager : MonoBehaviour
{
    public StudioEventEmitter PlayerSFX;

    [SerializeField]
    [ParamRef]
    private string PlayerActionSelector = null;

    [SerializeField]
    [ParamRef]
    private string PlayerSpecialSelector = null;

    [SerializeField]
    [ParamRef]
    private string PlayerSpecialController = null;

    //public void PlayPlayerDash() 
    //{
       // RuntimeManager.StudioSystem.setParameterByName(PlayerActionSelector, 0);
        //PlayerSFX.Play();
    //}

    public void PlayPlayerJump() 
    {
        RuntimeManager.StudioSystem.setParameterByName(PlayerActionSelector, 1);
        PlayerSFX.Play();
    }

    public void PlayPlayerAttack() 
    {
        RuntimeManager.StudioSystem.setParameterByName(PlayerActionSelector, 2);
        PlayerSFX.Play();
    }

    public void PlayPlayerFootsteps() 
    {
        RuntimeManager.StudioSystem.setParameterByName(PlayerActionSelector, 3);
        PlayerSFX.Play();
    }

    public void PlayPlayerDamage() 
    {
        RuntimeManager.StudioSystem.setParameterByName(PlayerActionSelector, 4);
        PlayerSFX.Play();
    }

    public void PlayGroundSlam() 
    {
        RuntimeManager.StudioSystem.setParameterByName(PlayerActionSelector, 5);
        RuntimeManager.StudioSystem.setParameterByName(PlayerSpecialSelector, 1);
        PlayerSFX.Play();
    }

    public void PlayShieldActive() 
    {
        RuntimeManager.StudioSystem.setParameterByName(PlayerActionSelector, 5);
        RuntimeManager.StudioSystem.setParameterByName(PlayerSpecialSelector, 0);
        RuntimeManager.StudioSystem.setParameterByName(PlayerSpecialController, 0);
        PlayerSFX.Play();
    }

    public void PlayShieldConnect() 
    {
        RuntimeManager.StudioSystem.setParameterByName(PlayerActionSelector, 5);
        RuntimeManager.StudioSystem.setParameterByName(PlayerSpecialSelector, 0);
         RuntimeManager.StudioSystem.setParameterByName(PlayerSpecialController, 1);
        PlayerSFX.Play();
    }

    public void PlayShieldDeactive() 
    {
        RuntimeManager.StudioSystem.setParameterByName(PlayerActionSelector, 5);
        RuntimeManager.StudioSystem.setParameterByName(PlayerSpecialSelector, 0);
         RuntimeManager.StudioSystem.setParameterByName(PlayerSpecialController, 2);
        PlayerSFX.Play();
    }
}
