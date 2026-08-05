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
        Debug.Log("PlayGroundSlam called");

        RuntimeManager.StudioSystem.setParameterByName(
            PlayerActionSelector,
            5,
            true
        );

        RuntimeManager.StudioSystem.setParameterByName(
            PlayerSpecialSelector,
            1,
            true
        );

        // Make FMOD apply both global parameter changes
        // before the event begins playing.
        RuntimeManager.StudioSystem.flushCommands();

        PlaySafe();
    }

    public void PlayShieldActive()
    {
        Debug.Log("PlayShieldActive called");

        RuntimeManager.StudioSystem.setParameterByName(
            PlayerActionSelector,
            5,
            true
        );

        RuntimeManager.StudioSystem.setParameterByName(
            PlayerSpecialSelector,
            0,
            true
        );

        RuntimeManager.StudioSystem.setParameterByName(
            PlayerSpecialController,
            0,
            true
        );

        PlaySafe();
    }

    public void PlayShieldConnect()
    {
        RuntimeManager.StudioSystem.setParameterByName(
            PlayerActionSelector,
            5,
            true
        );

        RuntimeManager.StudioSystem.setParameterByName(
            PlayerSpecialSelector,
            0,
            true
        );

        RuntimeManager.StudioSystem.setParameterByName(
            PlayerSpecialController,
            1,
            true
        );

        PlaySafe();
    }

    public void PlayShieldDeactive()
    {
        RuntimeManager.StudioSystem.setParameterByName(
            PlayerActionSelector,
            5,
            true
        );

        RuntimeManager.StudioSystem.setParameterByName(
            PlayerSpecialSelector,
            0,
            true
        );

        RuntimeManager.StudioSystem.setParameterByName(
            PlayerSpecialController,
            2,
            true
        );

        PlaySafe();
    }
}