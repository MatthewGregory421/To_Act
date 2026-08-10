using UnityEngine;

public class GroundSlamPickup : MonoBehaviour
{
    [Header("Pickup ID")]
    [SerializeField] private string pickupID = "GroundSlamPickup";

    private bool collected;


    private void Start()
    {
        if (WorldStateManager.Instance == null)
            return;

        Debug.Log(
            $"Checking pickup {pickupID} : " +
            WorldStateManager.Instance.IsPickupCollected(pickupID)
        );

        if (WorldStateManager.Instance.IsPickupCollected(pickupID))
        {
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
            return;

        PlayerMovementInputSystem player =
            other.GetComponentInParent<PlayerMovementInputSystem>();

        if (player == null)
            return;

        collected = true;


        // =========================
        // GIVE ABILITY
        // =========================

        player.hasGroundSlam = true;
        player.UpdateAbilityUI();


        // =========================
        // SAVE PICKUP STATE
        // =========================

        if (WorldStateManager.Instance != null)
        {
            WorldStateManager.Instance.CollectPickup(pickupID);
        }

        Debug.Log("Collected pickup: " + pickupID);


        // =========================
        // PICKUP ANIMATION
        // =========================

        player.StartPickupAnimation();


        // The pickup itself can disappear now.
        // The animation/input sequence is running on the PLAYER,
        // so destroying this object won't interrupt it.
        Destroy(gameObject);
    }
}