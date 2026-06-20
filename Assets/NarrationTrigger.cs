using UnityEngine;

public class NarrationTrigger : MonoBehaviour
{
    [SerializeField] private NarrationManager manager;

    [SerializeField]
    [Min(0)]
    private int index;

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        manager.RequestNarration(index);

        GetComponent<Collider2D>().enabled = false;
        enabled = false;
    }
}