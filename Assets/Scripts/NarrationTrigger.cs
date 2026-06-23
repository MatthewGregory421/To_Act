using UnityEngine;

public class NarrationTrigger : MonoBehaviour
{
    [SerializeField] private NarrationManager manager;

    [Header("Persistence")]
    [SerializeField] private string narrationID;

    [SerializeField]
    [Min(0)]
    private int index;

    private bool triggered;

    private void Start()
    {
        if (string.IsNullOrEmpty(narrationID))
        {
            narrationID = gameObject.scene.name + "_" + gameObject.name + "_" + index;
        }

        if (WorldStateManager.Instance != null &&
            WorldStateManager.Instance.HasPlayedNarrationTrigger(narrationID))
        {
            DisableTrigger();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        if (WorldStateManager.Instance != null)
        {
            WorldStateManager.Instance.PlayNarrationTrigger(narrationID);
        }

        manager.RequestNarration(index);

        DisableTrigger();
    }

    private void DisableTrigger()
    {
        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
            col.enabled = false;

        enabled = false;
    }
}