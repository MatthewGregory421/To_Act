using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public Transform player;

    [Header("Health")]
    public int maxHealth = 3;
    protected int currentHealth;

    [Header("Knockback")]
    public float knockbackForce = 20f;
    public bool isKnockedBack;
    private float knockbackTimer;

    [Header("State")]
    public bool isDead;
    public bool isInvincible;

    [Header("Persistence")]
    public string enemyID;

    [Header("Death")]
    public bool destroyOnDeath = true;

    [Header("Movement")]
    public float moveSpeed = 3f;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;

        // NEW: persistence check
        if (WorldStateManager.Instance != null)
        {
            if (WorldStateManager.Instance.IsEnemyDead(enemyID))
            {
                gameObject.SetActive(false);
                return;
            }
        }
    }

    protected virtual void Update()
    {
        TryFindPlayer();

        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;

            if (knockbackTimer <= 0f)
                isKnockedBack = false;
        }

        print("current health = " + currentHealth);
    }

    public virtual void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (isDead || isInvincible) return;

        currentHealth -= damage;

        Debug.Log($"{gameObject.name} took {damage} damage. HP: {currentHealth}");

        Knockback(hitDirection, knockbackForce);

        if (currentHealth <= 0)
            Die();
    }

    public void Knockback(Vector2 direction, float force)
    {
        if (rb == null || isDead) return;

        direction.Normalize();

        isKnockedBack = true;
        knockbackTimer = 0.2f;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    protected virtual void Die()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log($"{gameObject.name} died");

        if (WorldStateManager.Instance != null)
        {
            WorldStateManager.Instance.KillEnemy(enemyID);
        }

        EnemyDrops drops = GetComponent<EnemyDrops>();
        if (drops != null)
        {
            drops.HandleDeath();
        }

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (destroyOnDeath)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }

    protected void TryFindPlayer()
    {
        if (player != null)
            return;

        GameObject found = GameObject.FindGameObjectWithTag("Player");

        if (found != null)
        {
            player = found.transform;
        }
    }

    // =========================
    // MOVEMENT SUPPORT (FOR CHILD SCRIPTS)
    // =========================

    protected void FaceDirection(float direction)
    {
        if (direction == 0) return;

        Vector3 scale = transform.localScale;
        scale.x = direction > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}