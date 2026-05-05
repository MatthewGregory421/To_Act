using UnityEngine;
using FMODUnity;

public class PlayerSFXManager : MonoBehaviour
//Copy from this line down
{
    //For Josh, this script can be used as a TEMPLATE, for SFX management and triggering DO NOT ADD OR SUBTRACT THINGS FROM THIS SCRIPT, COPY IT'S CONTENTS AND MAKE YOUR OWN
    // Okay, so it might make the most sense to have each sound effect cluster (i.e. player, emotion specific enemies, etc) handled by a seperate script so just keep that in mind
    // Step one though is copy this script to a new script, name that script whatever makes the most sence. Put that script on the SceneManager object. 
    // Note that when you're copying, you only need to take the stuff between the copy labels

    //Step two, these are your emitter controllers, copy one of the lines below and change that light blue lettering (i.e. "MusOver") to whatever makes the most sense for the event you want to call
    // now over in unity, you can drag that event emitter from whatever object it's on into the correctly labelled field and Bob's your mums brother. Now this script can control that emitter with specific controls. I reccomend doing this step last because it'll save you going back and forth a bunch.
    public StudioEventEmitter PlayerDash;
    public StudioEventEmitter PlayerJump;
    public StudioEventEmitter PlayerAttack;
    public StudioEventEmitter PlayerGroundSlam;
    public StudioEventEmitter PlayerFootstep;
    public StudioEventEmitter PlayerHit;


    // For Josh, this is the section you need.
    //Basically these voids are how other scripts are going to call this one, in order to play sound effects.
    // Copy the script below and change their "names" (e.g."PlayPlayerDashEmitter") to a sound effect specific Play order (i.e. "PlayPlayerWalkEmitter")
    // Delete one of the stop orders in the below script and change the other one to "TheNameOfTheEmitterYouWantToPlay.Play();" (e.g. "PlayerWalk.Play();")
    // Since there's no stop order, you'll need to set up event stoppages in FMod so that we don't end up with 5 billion silent sound effect instances
    // Next, give Matt a call, get him to check the script and ask how to connect it up to the neccessary scripts and object to make sure it's called properly
    // NOTE, these voids will do everything you put in them, so you'll want to have seperate ones for each sound effect and just have the external script calling multiple voids.

    //---------------------
    //Player Sound Effects
    //---------------------

    public void PlayPlayerDashEmitter()
    {
        PlayerDash.Play();
    }
    public void PlayPlayerJumpEmitter()
    {
        PlayerJump.Play();
    }
    public void PlayPlayerAttackEmitter()
    {
        PlayerAttack.Play();
    }
    public void PlayPlayerGroundSlamEmitter()
    {
        PlayerGroundSlam.Play();
    }
    public void PlayPlayerFootstepEmitter()
    {
        PlayerFootstep.Play();
    }
    public void PlayPlayerHitEmitter()
    {
        PlayerHit.Play();
    }
}
//Copy from this line up
//Below is a commented example of how to set up code to change parameter values (as I presently understand it, I've not tested this)
    //public void PlayerJumpEmitterInt1()
    //{
    //  PlayerJump.SetParameter("Intensity", 1)
    //}