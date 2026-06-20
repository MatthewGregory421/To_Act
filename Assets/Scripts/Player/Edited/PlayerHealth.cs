using UnityEngine;
using System.Collections;


public class PlayerHealth : MonoBehaviour
{

    private bool isDead;
    public bool isInvincible;

    private PlayerMovementInputSystem movement;
    public PlayerSFXManager playerSFXManager;

    [Header("Health")]
    public int maxHealth = 5;
    public int currentHealth;

    [Header("Damage Effects")]
    public float knockbackForce = 5f;

    [Header("I-Frames")]
    public float invincibilityDuration = 1f;

    private Rigidbody2D rb;

    private void Start()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();

        movement = GetComponent<PlayerMovementInputSystem>();
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        if (isDead || isInvincible)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        playerSFXManager.PlayPlayerDamage();

        Debug.Log("Player took damage! HP: " + currentHealth);

        Knockback(
            knockbackDirection,
            knockbackForce,
            1f
        );

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(InvincibilityFrames());
    }

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;

        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;
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

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        if (movement != null)
            movement.enabled = false;

        currentHealth = maxHealth;
        isDead = false;

        string sceneName = WorldStateManager.Instance.GetCurrentScene();
        string benchID = WorldStateManager.Instance.GetCurrentBench();


        Debug.Log("Respawn scene: " + sceneName);
        Debug.Log("Respawn bench/spawn ID: " + benchID);

        // IMPORTANT: let SceneTransitionManager handle spawn + fade
        SceneTransitionManager.Instance.RespawnAtBench(sceneName, benchID);

        // wait until transition finishes
        while (SceneTransitionManager.Instance.IsTransitioning)
            yield return null;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (movement != null)
            movement.enabled = true;
    }

    public void FullHeal()
    {
        currentHealth = maxHealth;
    }
}