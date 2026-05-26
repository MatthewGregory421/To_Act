using UnityEngine;

public class EnemyMovement : EnemyBase
{
    [Header("Movement")]
    private int direction = 1;

    [Header("Detection")]
    public float detectionRange = 6f;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("Edge Detection")]
    public Transform edgeCheckPoint;
    public float edgeCheckDistance = 0.5f;

    [Header("Edge Behaviour")]
    public float edgePauseTime = 0.4f;
    private float edgePauseTimer;
    private bool isPausedAtEdge;

    [Header("Wall Detection")]
    public Transform wallCheckPoint;
    public float wallCheckDistance = 0.3f;
    public LayerMask wallLayer;

    protected virtual void Update()
    {
        if (isDead) return;

        bool chasing = PlayerDetected();

        HandleEdgePause();

        Move(chasing);

        // Only run environment checks when NOT paused
        if (!isPausedAtEdge)
        {
            if (IsGrounded())
            {
                CheckEdge(chasing);
                CheckWall();
            }
        }
    }

    // =========================
    // MOVEMENT
    // =========================

    void Move(bool chasing)
    {
        if (isPausedAtEdge)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (chasing)
        {
            direction = PlayerDirection();
        }
        else
        {
            PatrolMovement();
        }

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        FaceDirection(direction);
    }

    // =========================
    // PATROL LOGIC
    // =========================

    void PatrolMovement()
    {
        if (!IsGrounded())
        {
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(
            edgeCheckPoint.position,
            Vector2.down,
            edgeCheckDistance,
            groundLayer
        );

        if (hit.collider == null)
        {
            TriggerEdgePause();
        }
    }

    // =========================
    // PLAYER DETECTION
    // =========================

    bool PlayerDetected()
    {
        if (player == null) return false;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > detectionRange)
            return false;

        Vector2 origin = transform.position;
        Vector2 dir = ((Vector2)player.position - origin).normalized;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            dir,
            detectionRange,
            wallLayer | groundLayer
        );

        if (hit.collider == null)
            return true;

        if (hit.collider.transform == player)
            return true;

        return false;
    }

    int PlayerDirection()
    {
        if (player == null) return direction;

        return player.position.x > transform.position.x ? 1 : -1;
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
    // EDGE PAUSE SYSTEM
    // =========================

    void HandleEdgePause()
    {
        if (!isPausedAtEdge) return;

        edgePauseTimer -= Time.deltaTime;

        if (edgePauseTimer <= 0f)
        {
            isPausedAtEdge = false;

            // ensure movement is "released" before flipping
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            Flip();
        }
    }

    void TriggerEdgePause()
    {
        isPausedAtEdge = true;
        edgePauseTimer = edgePauseTime;
    }

    // =========================
    // CHECKS
    // =========================

    void CheckEdge(bool chasing)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            edgeCheckPoint.position,
            Vector2.down,
            edgeCheckDistance,
            groundLayer
        );

        if (hit.collider == null)
        {
            if (chasing)
            {
                // pause instead of flipping while chasing
                TriggerEdgePause();
            }
            else
            {
                // patrol behaviour = flip
                Flip();
            }
        }
    }

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
            Gizmos.DrawRay(groundCheckPoint.position, Vector2.down * groundCheckDistance);
        }

        if (edgeCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(edgeCheckPoint.position, Vector2.down * edgeCheckDistance);
        }

        if (wallCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Vector2 dir = Vector2.right * (Application.isPlaying ? direction : 1);
            Gizmos.DrawRay(wallCheckPoint.position, dir * wallCheckDistance);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}