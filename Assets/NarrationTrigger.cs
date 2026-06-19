using UnityEngine;

public class NarrationTrigger : MonoBehaviour
{
    public NarrationManager manager;
    public int index;

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        manager.PlayNarration(index);

        GetComponent<Collider2D>().enabled = false;
        enabled = false;
    }
}