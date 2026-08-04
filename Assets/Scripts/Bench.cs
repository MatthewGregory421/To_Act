using UnityEngine;
using FMODUnity;

public class Bench : MonoBehaviour
{
    public string benchID;

    public bool healPlayer = true;
    public bool resetEnemiesOnRest = true;

    [Header("Sitting")]
    [SerializeField] private Transform sitPoint;

    [Header("Audio")]
    [SerializeField] private EventReference benchSitSound;

    private bool playerInRange;

    private void Update()
    {
        if (!playerInRange)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Sit();
        }
    }

    private void Sit()
    {
        PlayerMovementInputSystem player =
            FindFirstObjectByType<PlayerMovementInputSystem>();

        if (player == null)
            return;

        // Prevent repeatedly sitting while already seated.
        if (player.IsSitting)
            return;

        Debug.Log("Sitting at bench: " + benchID);

        PlayBenchSitSound();

        WorldStateManager.Instance.SetCurrentBench(benchID);
        WorldStateManager.Instance.SetCurrentScene(
            gameObject.scene.name
        );

        // Lock the player to the bench.
        player.SitAtBench(sitPoint);

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame(
                player,
                gameObject.scene.name,
                SaveManager.Instance.currentSlot
            );
        }

        if (healPlayer)
        {
            PlayerHealth playerHealth =
                player.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.FullHeal();
            }
        }

        if (resetEnemiesOnRest &&
            WorldStateManager.Instance != null)
        {
            WorldStateManager.Instance.RestAtBench();
        }
    }

    private void PlayBenchSitSound()
    {
        if (benchSitSound.IsNull)
            return;

        RuntimeManager.PlayOneShot(
            benchSitSound,
            transform.position
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}