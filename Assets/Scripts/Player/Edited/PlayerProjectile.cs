using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public float speed = 12f;
    public int damage = 1;

    public LayerMask enemyLayer;

    private Rigidbody2D rb;
    private Vector2 moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
        rb.linearVelocity = moveDirection * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hit: " + collision.name);

        int layer = collision.gameObject.layer;

        // ENEMY HIT ONLY
        if (((1 << layer) & enemyLayer) != 0)
        {
            Vector2 dir = rb.linearVelocity.normalized;
            collision.GetComponent<EnemyBase>()?.TakeDamage(damage, dir);

            Destroy(gameObject);
            return;
        }

        // IMPORTANT CHANGE:
        // Ignore shield completely
        if (collision.CompareTag("PlayerShield"))
        {
            return;
        }

        // Optional: only destroy on actual world collision if you want
        Destroy(gameObject);
    }
}