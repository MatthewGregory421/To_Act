using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 1;
    public float lifetime = 10f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player != null)
        {
            player.currentHealth = Mathf.Clamp(
                player.currentHealth + healAmount,
                0,
                player.maxHealth
            );

            Debug.Log("Player healed!");

            Destroy(gameObject);
        }
    }
}
