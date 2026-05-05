using UnityEngine;
using FMODUnity;


public class PlayerHealth : MonoBehaviour
{
    public DeathManager deathManager;
    public PlayerSFXManager sfxManager;

    [Header("Health")]
    public int maxHealth = 5;
    public int currentHealth;

    [Header("Damage Effects")]
    public float knockbackForce = 5f;
    public float flashDuration = 0.2f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            KillPlayer();
        }
    }

    // Simple damage + knockback + flash
    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        currentHealth -= damage;
        sfxManager.PlayPlayerHitEmitter();
        Debug.Log("Player took damage! HP: " + currentHealth);

        Knockback(knockbackDirection, knockbackForce, 1f); // default duration multiplier
        FlashRed(flashDuration);

        if (currentHealth <= 0)
            Die();
    }

    // Public knockback method
    public void Knockback(Vector2 direction, float horizontalForce, float verticalForce)
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // reset velocity
            Vector2 force = new Vector2(direction.x * horizontalForce, direction.y * verticalForce);
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }

    // Public flash method
    public void FlashRed(float duration)
    {
        if (spriteRenderer != null)
            StartCoroutine(FlashRoutine(duration));
    }

    private System.Collections.IEnumerator FlashRoutine(float duration)
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(duration);
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        Debug.Log("Player died!");
        deathManager.PlayerDied();
    }

    void KillPlayer()
    {
        currentHealth = 1;
    }
}