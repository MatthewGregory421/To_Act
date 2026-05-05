using UnityEngine;


public class PlayerHealth_E : MonoBehaviour
{
    public DeathManager deathManager;

    [Header("Health")]
    public int maxHealth = 5;
    public int currentHealth;

    [Header("Damage Effects")]
    public float knockbackForce = 5f;

    private Rigidbody2D rb;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
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
        Debug.Log("Player took damage! HP: " + currentHealth);

        Knockback(knockbackDirection, knockbackForce, 1f); // default duration multiplier

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