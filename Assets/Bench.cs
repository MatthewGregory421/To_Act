using UnityEngine;

public class Bench : MonoBehaviour
{
    public string benchID;

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

        WorldStateManager.Instance.SetCurrentBench(benchID);
        WorldStateManager.Instance.SetCurrentScene(gameObject.scene.name);

        int slot = PlayerPrefs.GetInt("SelectedSlot");

        PlayerMovementInputSystem player =
            FindObjectOfType<PlayerMovementInputSystem>();

        if (player != null)
        {
            SaveManager.Instance.SaveGame(player, gameObject.scene.name, SaveManager.Instance.currentSlot);
        }

        if (healPlayer)
        {
            PlayerHealth ph = FindObjectOfType<PlayerHealth>();
            if (ph != null)
                ph.FullHeal();
        }

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