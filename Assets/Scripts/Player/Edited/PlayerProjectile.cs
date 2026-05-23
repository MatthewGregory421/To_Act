using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public float speed = 12f;
    public int damage = 1;

    public LayerMask enemyLayer;
    public LayerMask solidLayers;

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

        // ENEMY
        if (((1 << layer) & enemyLayer) != 0)
        {
            collision.GetComponent<EnemyHealth>()?.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // SOLID
        if (((1 << layer) & solidLayers) != 0)
        {
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}