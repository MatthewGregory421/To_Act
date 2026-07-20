using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyMusicDetectionRange : MonoBehaviour
{
    private readonly HashSet<Collider2D> playerColliders =
        new HashSet<Collider2D>();

    private void Reset()
    {
        Collider2D detectionCollider = GetComponent<Collider2D>();

        if (detectionCollider != null)
        {
            detectionCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other))
            return;

        if (!playerColliders.Add(other))
            return;

        // This is the first player collider to enter this enemy's range.
        if (playerColliders.Count == 1)
        {
            MusicManager.Instance?.EnterEnemyDetectionRange();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!playerColliders.Remove(other))
            return;

        // All player colliders have now left this enemy's range.
        if (playerColliders.Count == 0)
        {
            MusicManager.Instance?.ExitEnemyDetectionRange();
        }
    }

    private void OnDisable()
    {
        // Prevent the music getting stuck if the enemy is destroyed or disabled
        // while the player is still inside its detection range.
        if (playerColliders.Count > 0)
        {
            MusicManager.Instance?.ExitEnemyDetectionRange();
            playerColliders.Clear();
        }
    }

    private bool IsPlayer(Collider2D other)
    {
        return other.CompareTag("Player") ||
               other.transform.root.CompareTag("Player");
    }
}