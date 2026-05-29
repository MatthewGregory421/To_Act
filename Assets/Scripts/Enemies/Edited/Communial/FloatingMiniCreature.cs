using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FloatingMiniCreature : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float changeDirectionTime = 2f;
    public float floatStrength = 0.5f;

    public Vector2 roamBounds = new Vector2(10f, 10f);

    private Rigidbody2D rb;
    private Vector2 direction;
    private float timer;

    private Vector3 startPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;

        PickNewDirection();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            PickNewDirection();
        }
    }

    void FixedUpdate()
    {
        Vector2 floatBob = Vector2.up * Mathf.Sin(Time.time * 2f) * floatStrength;

        rb.linearVelocity = (direction * moveSpeed) + floatBob;

        KeepInBounds();
    }

    void PickNewDirection()
    {
        direction = Random.insideUnitCircle.normalized;
        timer = changeDirectionTime;
    }

    void KeepInBounds()
    {
        Vector3 offset = transform.position - startPos;

        if (Mathf.Abs(offset.x) > roamBounds.x)
            direction.x = -direction.x;

        if (Mathf.Abs(offset.y) > roamBounds.y)
            direction.y = -direction.y;
    }
}
