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

    private void PlaySafe()
    {
        if (PlayerSFX == null)
            return;

        try
        {
            PlayerSFX.Play();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Player SFX failed: " + e.Message);
        }
    }

    public void PlayPlayerHealthUp()
    {
        RuntimeManager.StudioSystem.setParameterByName(PlayerActionSelector, 6);
        PlaySafe();
    }

    public void PlayPlayerJump()
    {
        RuntimeManager.StudioSystem.setParameterByName(PlayerActionSelector, 1);
        PlaySafe();
    }

    public void PlayPlayerAttack()
    {
        RuntimeManager.StudioSystem.setParameterByName(PlayerActionSelector, 2);
        PlaySafe();
    }

    public void PlayPopSFX()
    {
        RuntimeManager.StudioSystem.setParameterByName(PlayerActionSelector, 7);
        PlaySafe();
    }

    public void PlayPlayerFootsteps()
    {
        RuntimeManager.StudioSystem.setParameterByName(PlayerActionSelector, 3);
        PlaySafe();
    }

    public void PlayPlayerDamage()
    {
        RuntimeManager.StudioSystem.setParameterByName(PlayerActionSelector, 4);
        PlaySafe();
    }

    public void PlayGroundSlam()
    {
        RuntimeManager.StudioSystem.setParameterByName(PlayerActionSelector, 5);
        RuntimeManager.StudioSystem.setParameterByName(PlayerSpecialSelector, 1);
        PlaySafe();
    }

    public void PlayShieldActive()
    {
        RuntimeManager.StudioSystem.setParameterByName(PlayerActionSelector, 5);
        RuntimeManager.StudioSystem.setParameterByName(PlayerSpecialSelector, 0);
        RuntimeManager.StudioSystem.setParameterByName(PlayerSpecialController, 0);
        PlaySafe();
    }

    public void PlayShieldConnect()
    {
        RuntimeManager.StudioSystem.setParameterByName(PlayerActionSelector, 5);
        RuntimeManager.StudioSystem.setParameterByName(PlayerSpecialSelector, 0);
        RuntimeManager.StudioSystem.setParameterByName(PlayerSpecialController, 1);
        PlaySafe();
    }

    public void PlayShieldDeactive()
    {
        RuntimeManager.StudioSystem.setParameterByName(PlayerActionSelector, 5);
        RuntimeManager.StudioSystem.setParameterByName(PlayerSpecialSelector, 0);
        RuntimeManager.StudioSystem.setParameterByName(PlayerSpecialController, 2);
        PlaySafe();
    }
}