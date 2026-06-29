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
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        if (isDead || isInvincible)
            return;

        playerAnimations?.TakeDamage();

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (healthUI != null)
        {
            healthUI.UpdateHealth(currentHealth, maxHealth);
        }

        if (playerSFXManager != null)
            playerSFXManager.PlayPlayerDamage();

        if (CameraLag.Instance != null)
            CameraLag.Instance.ShakeCamera();

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

        FullHeal();
        isDead = false;

        string sceneName = WorldStateManager.Instance.GetCurrentScene();
        string benchID = WorldStateManager.Instance.GetCurrentBench();

        WorldStateManager.Instance.RespawnEnemiesFromBench();

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

        Debug.Log("Player healed! HP: " + currentHealth);
    }

    public void FullHeal()
    {
        currentHealth = maxHealth;

        if (healthUI != null)
        {
            healthUI.UpdateHealth(currentHealth, maxHealth);
        }
    }
}