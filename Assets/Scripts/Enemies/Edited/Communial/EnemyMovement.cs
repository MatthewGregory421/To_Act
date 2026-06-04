using UnityEngine;

public class EnemyMovement : EnemyBase
{
    [Header("Movement")]
    [SerializeField] private int direction = 1;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("Edge Detection")]
    public Transform edgeCheckPoint;
    public float edgeCheckDistance = 0.5f;

    [Header("Edge Pause")]
    public float edgePauseTime = 0.3f;
    private float edgePauseTimer;
    private bool isEdgePaused;

    [Header("Wall Detection")]
    public Transform wallCheckPoint;
    public float wallCheckDistance = 0.3f;
    public LayerMask wallLayer;

    [Header("Behaviour")]
    public bool canMove = true;

    public System.Action OnHitEdge;

    protected override void Update()
    {
        base.Update();

        if (isDead || isKnockedBack || isEdgePaused)
            return;

        if (canMove)
        {
            Move();

            if (IsGrounded())
            {
                CheckEdge();
                CheckWall();
            }
        }
    }

    // =========================
    // MOVEMENT
    // =========================

    void Move()
    {
        if (!canMove || isKnockedBack)
            return;

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        FaceDirection(direction);
    }

    // =========================
    // EXTERNAL CONTROL
    // =========================

    public void SetDirection(int newDirection)
    {
        direction = newDirection;
    }

    public int GetDirection()
    {
        return direction;
    }

    // =========================
    // GROUND CHECK
    // =========================

    bool IsGrounded()
    {
        return Physics2D.Raycast(
            groundCheckPoint.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
    }

    // =========================
    // EDGE CHECK
    // =========================

    void CheckEdge()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            edgeCheckPoint.position,
            Vector2.down,
            edgeCheckDistance,
            groundLayer
        );

        if (hit.collider == null)
        {
            isEdgePaused = true;
            edgePauseTimer = edgePauseTime;
        }
    }

    void HandleEdgePause()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        edgePauseTimer -= Time.deltaTime;

        if (edgePauseTimer <= 0f)
        {
            isEdgePaused = false;
            Flip();

            OnHitEdge?.Invoke();
        }
    }

    public bool HasGroundAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            edgeCheckPoint.position,
            Vector2.down,
            edgeCheckDistance,
            groundLayer
        );

        return hit.collider != null;
    }

    // =========================
    // WALL CHECK
    // =========================

    void CheckWall()
    {
        Vector2 dir = Vector2.right * direction;

        RaycastHit2D hit = Physics2D.Raycast(
            wallCheckPoint.position,
            dir,
            wallCheckDistance,
            wallLayer
        );

        if (hit.collider != null)
        {
            Flip();
        }
    }

    // =========================
    // FLIP
    // =========================

    void Flip()
    {
        direction *= -1;
    }

    // =========================
    // DEBUG VISUALS
    // =========================

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(
                groundCheckPoint.position,
                Vector2.down * groundCheckDistance
            );
        }

        if (edgeCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(
                edgeCheckPoint.position,
                Vector2.down * edgeCheckDistance
            );
        }

        if (wallCheckPoint != null)
        {
            Gizmos.color = Color.red;

            Vector2 dir = Vector2.right * (Application.isPlaying ? direction : 1);

            Gizmos.DrawRay(
                wallCheckPoint.position,
                dir * wallCheckDistance
            );
        }
    }
}