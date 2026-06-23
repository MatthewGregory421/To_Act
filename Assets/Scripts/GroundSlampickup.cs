using UnityEngine;

public class GroundSlamPickup : MonoBehaviour
{
    [Header("Pickup ID")]
    [SerializeField] private string pickupID = "GroundSlamPickup";

    private void Start()
    {
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
        PlayerMovementInputSystem player =
            other.GetComponentInParent<PlayerMovementInputSystem>();

        if (player == null)
            return;

        player.hasGroundSlam = true;
        player.UpdateAbilityUI();

        WorldStateManager.Instance.CollectPickup(pickupID);

        Debug.Log("Collected pickup: " + pickupID);

        Destroy(gameObject);
    }
}