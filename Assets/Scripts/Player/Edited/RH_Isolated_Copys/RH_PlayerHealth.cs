using UnityEngine;
using UnityEngine.InputSystem;


public class RH_PlayerHealth : MonoBehaviour
{

    public RH_DeathManager deathManager;

    private bool isDead;

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

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player took damage! HP: " + currentHealth);

        Knockback(
            knockbackDirection,
            knockbackForce,
            1f
        );

        if (currentHealth <= 0)
            Die();
    }

    public void Knockback(
        Vector2 direction,
        float horizontalForce,
        float verticalForce
    )
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;

            Vector2 force = new Vector2(
                direction.x * horizontalForce,
                direction.y * verticalForce
            );

            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log("Player died!");

        deathManager.PlayerDied();
    }
}