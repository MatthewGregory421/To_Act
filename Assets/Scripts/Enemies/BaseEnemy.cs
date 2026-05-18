using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 3;
    public int damage = 1;
    public float moveSpeed = 2f;

    [Header("Runtime")]
    public int currentHealth;

    [Header("References")]
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public Transform player;

    [Header("Damage Effects")]
    public float knockbackForce = 5f;
    public float flashDuration = 0.2f;

    protected Color originalColor;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
        else
            Debug.LogWarning("BaseEnemy: Player not found (check tag)");
    }

    public void SetVelocity(Vector2 vel)
    {
        if (rb == null) return;
        rb.linearVelocity = vel;
    }

    public void StopHorizontal()
    {
        if (rb == null) return;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    public void TakeDamage(int dmg, Vector2 knockbackDirection)
    {
        currentHealth -= dmg;

        // Knockback
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);
        }

        // Flash red
        if (sr != null)
            StartCoroutine(FlashRed());

        if (currentHealth <= 0)
            Die();
    }

    private System.Collections.IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}