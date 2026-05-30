using UnityEngine;

public class ObjectiveArrow : MonoBehaviour
{
    [Header("Target")]
    public Transform targetBox;

    [Header("Player Search")]
    public string playerTag = "Player";

    [Header("Arrow Movement")]
    public float distanceFromPlayer = 1.5f;

    [Header("Arrow Rotation")]
    public float rotationOffset = -90f;

    private Transform player;

    private void Update()
    {
        // Find player automatically if missing
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag(playerTag);

            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
            else
            {
                return;
            }
        }

        // Make sure target exists
        if (targetBox == null)
            return;

        // Direction from player to target
        Vector2 direction = (targetBox.position - player.position).normalized;

        // Position arrow around player
        transform.position = (Vector2)player.position + (direction * distanceFromPlayer);

        // Rotate arrow toward target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Destroy arrow when player reaches target
        if (collision.transform == targetBox)
        {
            Destroy(gameObject);
        }
    }
}