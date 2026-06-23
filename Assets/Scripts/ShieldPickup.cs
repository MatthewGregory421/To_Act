using UnityEngine;

public class ShieldPickup : MonoBehaviour
{
    [Header("Pickup ID")]
    [SerializeField] private string pickupID = "ShieldPickup";

    private void Start()
    {
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

        player.hasShield = true;
        player.UpdateAbilityUI();

        WorldStateManager.Instance.CollectPickup(pickupID);

        Debug.Log("Collected pickup: " + pickupID);

        Destroy(gameObject);
    }
}