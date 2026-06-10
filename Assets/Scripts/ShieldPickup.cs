using UnityEngine;

public class ShieldPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovementInputSystem player =
            other.GetComponentInParent<PlayerMovementInputSystem>();

        if (player == null)
            return;

        player.hasShield = true;

        Destroy(gameObject);
    }
}