using UnityEngine;

public class Bench : MonoBehaviour
{
    [Header("Bench ID")]
    public string benchID;

    [Header("Optional")]
    public bool healPlayer = true;
    public bool resetEnemiesOnRest = true;

    private bool playerInRange;

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Sit();
        }
    }

    private void Sit()
    {
        Debug.Log("Sitting at bench: " + benchID);

        // Set respawn point
        WorldStateManager.Instance.SetCurrentBench(benchID, transform.position);

        // Remember which scene this bench belongs to
        WorldStateManager.Instance.SetCurrentScene(gameObject.scene.name);

        // Optional heal hook
        if (healPlayer)
        {
            PlayerHealth ph = FindObjectOfType<PlayerHealth>();
            if (ph != null)
            {
                ph.FullHeal();
            }
        }

        // Optional enemy reset (Hollow Knight style)
        if (resetEnemiesOnRest)
        {
            WorldStateManager.Instance.RestAtBench();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
