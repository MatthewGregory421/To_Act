using UnityEngine;
using FMODUnity;

public class Bench : MonoBehaviour
{
    public string benchID;

    public bool healPlayer = true;
    public bool resetEnemiesOnRest = true;

    [Header("Audio")]
    [SerializeField]
    private EventReference benchSitSound;

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
        Debug.Log("Sitting at bench: " + benchID);

        PlayBenchSitSound();

        WorldStateManager.Instance.SetCurrentBench(benchID);
        WorldStateManager.Instance.SetCurrentScene(
            gameObject.scene.name
        );

        PlayerMovementInputSystem player =
            FindFirstObjectByType<PlayerMovementInputSystem>();

        if (player != null)
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
                FindFirstObjectByType<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.FullHeal();
            }
        }

        if (resetEnemiesOnRest)
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