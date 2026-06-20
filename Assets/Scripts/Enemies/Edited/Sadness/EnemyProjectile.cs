using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;

    public LayerMask playerLayer;
    public LayerMask solidLayers;
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
        int layer = collision.gameObject.layer;

        // PLAYER
        if (((1 << layer) & playerLayer) != 0)
        {
            PlayerHealth player = collision.GetComponent<PlayerHealth>();

            if (player != null)
            {
                Vector2 knockbackDir =
                    (collision.transform.position - transform.position).normalized;

                player.TakeDamage(damage, knockbackDir);
            }

            Destroy(gameObject);
            return;
        }

        // WALLS / GROUND
        if (((1 << layer) & solidLayers) != 0)
        {
            Destroy(gameObject);
            return;
        }
    }
}