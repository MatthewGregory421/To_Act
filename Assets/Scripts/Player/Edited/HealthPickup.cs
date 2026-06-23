using UnityEngine;
using UnityEngine.VFX;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 1;
    public float lifetime = 10f;

    private PlayerSFXManager sfxManager;

    private void Start()
    {
        sfxManager = FindFirstObjectByType<PlayerSFXManager>();

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player != null)
        {
            player.Heal(healAmount);

            sfxManager?.PlayPlayerHealthUp();

            Destroy(gameObject);
        }
    }
}
