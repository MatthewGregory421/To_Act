using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public float speed = 12f;
    public int damage = 1;

    public LayerMask enemyLayer;

    private Rigidbody2D rb;
    private Vector2 moveDirection;

    private PlayerSFXManager sfxManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Finds the player's SFX manager in the scene
        sfxManager = FindFirstObjectByType<PlayerSFXManager>();
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
        rb.linearVelocity = moveDirection * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hit: " + collision.name);

        // Ignore the player's shield
        if (collision.CompareTag("PlayerShield"))
        {
            return;
        }

        int layer = collision.gameObject.layer;

        // Play the pop sound before destroying the projectile
        if (sfxManager != null)
        {
            sfxManager.PlayPopSFX();
        }
        else
        {
            Debug.LogWarning("PlayerProjectile could not find PlayerSFXManager.");
        }

        // Enemy hit
        if (((1 << layer) & enemyLayer) != 0)
        {
            Vector2 dir = rb.linearVelocity.normalized;

            EnemyBase enemy = collision.GetComponent<EnemyBase>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage, dir);
            }

            Destroy(gameObject);
            return;
        }

        // World hit
        Destroy(gameObject);
    }
}