using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public Transform player;

    [Header("Health")]
    public int maxHealth = 3;
    protected int currentHealth;

    [Header("State")]
    public bool isDead;

    [Header("Death")]
    public bool destroyOnDeath = true;

    [Header("Movement")]
    public float moveSpeed = 3f;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;

        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null)
                player = found.transform;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        Debug.Log($"{gameObject.name} took {damage} damage. HP: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    protected virtual void Die()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log($"{gameObject.name} died");

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (destroyOnDeath)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
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