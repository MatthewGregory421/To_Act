using UnityEngine;

public class GroundSlampickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovementInputSystem player =
            other.GetComponentInParent<PlayerMovementInputSystem>();

        if (player == null)
            return;

        player.hasGroundSlam = true;

        Destroy(gameObject);
    }
}