using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    private bool isDead;
    public bool isInvincible;

    private PlayerMovementInputSystem movement;
    public PlayerSFXManager playerSFXManager;
    private HealthUI healthUI;
    public PlayerAnimations playerAnimations;

    [Header("Health")]
    public int maxHealth = 5;
    public int currentHealth;

    [Header("Damage Effects")]
    public float knockbackForce = 5f;

    [Header("I-Frames")]
    public float invincibilityDuration = 1f;

    [Header("Damage Flash")]
    public SpriteRenderer playerSprite;
    public Color damageColor = Color.red;
    public float flashSpeed = 0.1f;

    private Color originalColor;
    private Coroutine flashCoroutine;

    private Rigidbody2D rb;

    private void Start()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovementInputSystem>();

        if (playerSprite == null)
            playerSprite = GetComponentInChildren<SpriteRenderer>();

        if (playerSprite != null)
            originalColor = playerSprite.color;

        healthUI = FindFirstObjectByType<HealthUI>();

        if (healthUI != null)
        {
            healthUI.UpdateHealth(currentHealth, maxHealth);
        }

        playerAnimations = GetComponentInChildren<PlayerAnimations>();

        // Make sure the music knows the player's starting health.
        UpdateHealthMusic();
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        if (isDead || isInvincible)
            return;

        if (playerAnimations == null)
            Debug.LogWarning("PlayerHealth: No PlayerAnimations found!");
        else
            playerAnimations.TakeDamage();

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthUI != null)
        {
            healthUI.UpdateHealth(currentHealth, maxHealth);
        }

        // Update music after taking damage.
        UpdateHealthMusic();

        if (playerSFXManager != null)
            playerSFXManager.PlayPlayerDamage();

        if (CameraLag.Instance != null)
            CameraLag.Instance.ShakeCamera();

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

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(DamageFlash());

        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        if (playerSprite != null)
            playerSprite.color = originalColor;
    }

    private IEnumerator DamageFlash()
    {
        while (isInvincible)
        {
            if (playerSprite != null)
                playerSprite.color = damageColor;

            yield return new WaitForSeconds(flashSpeed);

            if (playerSprite != null)
                playerSprite.color = originalColor;

            yield return new WaitForSeconds(flashSpeed);
        }
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

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        if (movement != null)
            movement.enabled = false;

        FullHeal();
        isDead = false;

        string sceneName = WorldStateManager.Instance.GetCurrentScene();
        string benchID = WorldStateManager.Instance.GetCurrentBench();

        WorldStateManager.Instance.RespawnEnemiesFromBench();

        Debug.Log("Respawn scene: " + sceneName);
        Debug.Log("Respawn bench/spawn ID: " + benchID);

        SceneTransitionManager.Instance.RespawnAtBench(sceneName, benchID);

        while (SceneTransitionManager.Instance.IsTransitioning)
            yield return null;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (movement != null)
            movement.enabled = true;
    }

    public void Heal(int amount)
    {
        if (isDead)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthUI != null)
        {
            healthUI.UpdateHealth(currentHealth, maxHealth);
        }

        // Change from tense back to combat or neutral after healing.
        UpdateHealthMusic();

        Debug.Log("Player healed! HP: " + currentHealth);
    }

    public void FullHeal()
    {
        currentHealth = maxHealth;

        if (healthUI != null)
        {
            healthUI.UpdateHealth(currentHealth, maxHealth);
        }

        // Used when resting or respawning.
        UpdateHealthMusic();
    }

    private void UpdateHealthMusic()
    {
        if (MusicManager.Instance == null)
            return;

        MusicManager.Instance.SetPlayerAtOneHealth(
            currentHealth == 1
        );
    }
}