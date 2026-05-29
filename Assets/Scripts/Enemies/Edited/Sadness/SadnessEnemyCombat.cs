using UnityEngine;

public class SadnessEnemyCombat : MonoBehaviour
{
    [Header("References")]
    public EnemyBase enemyBase;
    public EnemyMovement movement;
    public EnemyProjectileSpawner projectileSpawner;
    public Transform player;

    [Header("Detection")]
    public float detectionRange = 8f;

    [Header("Line Of Sight")]
    public LayerMask lineOfSightMask;

    [Header("Spacing")]
    public float preferredDistance = 5f;
    public float retreatDistance = 3f;

    [Header("Behaviour")]
    public bool playerDetected;

    [Header("Edge Behaviour")]
    public float pauseAfterEdge = 1.5f;

    private bool movementLocked;
    private float movementLockTimer;

    [Header("Retreat Safety Check")]
    public Transform rearCheckPoint;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    private void OnEnable()
    {
        movement.OnHitEdge += HandleEdgePause;
    }

    private void OnDisable()
    {
        movement.OnHitEdge -= HandleEdgePause;
    }

    private void Update()
    {
        if (enemyBase.isDead) return;
        if (player == null) return;

        HandleMovementLock();

        DetectPlayer();

        // enable / disable shooting
        projectileSpawner.enabled = playerDetected;

        if (!playerDetected)
        {
            movement.canMove = true;
            return;
        }

        if (movementLocked)
        {
            movement.canMove = false;
            return;
        }

        HandleSpacing();
    }

    // =========================
    // PLAYER DETECTION
    // =========================

    void DetectPlayer()
    {
        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance <= detectionRange && HasLineOfSight())
        {
            playerDetected = true;
        }
        else
        {
            playerDetected = false;
        }
    }

    // =========================
    // LINE OF SIGHT
    // =========================

    bool HasLineOfSight()
    {
        Vector2 direction =
            player.position - transform.position;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            direction.normalized,
            detectionRange,
            lineOfSightMask
        );

        if (hit.collider != null)
        {
            return hit.collider.transform == player;
        }

        return false;
    }

    // =========================
    // DISTANCE CONTROL
    // =========================

    void HandleSpacing()
    {
        float deltaX = player.position.x - transform.position.x;
        float distance = Mathf.Abs(deltaX);

        // ALWAYS face player while active
        FacePlayer(deltaX);

        // =========================
        // EDGE PRIORITY (HIGHEST)
        // =========================
        bool atEdge = !HasGroundBehind();

        if (atEdge)
        {
            movement.canMove = false;
            return;
        }

        // =========================
        // TOO CLOSE -> RETREAT
        // =========================
        if (distance < retreatDistance)
        {
            int dir = (deltaX > 0) ? -1 : 1;

            movement.SetDirection(dir);
            movement.canMove = true;
            return;
        }

        // =========================
        // TOO FAR -> APPROACH
        // =========================
        if (distance > preferredDistance)
        {
            int dir = (deltaX > 0) ? 1 : -1;

            movement.SetDirection(dir);
            movement.canMove = true;
            return;
        }

        // =========================
        // IDEAL RANGE -> STOP
        // =========================
        movement.canMove = false;
    }

    void FacePlayer(float deltaX)
    {
        if (deltaX > 0)
            movement.SetDirection(1);
        else
            movement.SetDirection(-1);

        Vector3 scale = transform.localScale;

        scale.x = deltaX > 0
            ? Mathf.Abs(scale.x)
            : -Mathf.Abs(scale.x);

        transform.localScale = scale;
    }

    bool HasGroundBehind()
    {
        Vector2 origin = rearCheckPoint.position;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        return hit.collider != null;
    }

    // =========================
    // EDGE PAUSE EVENT
    // =========================

    void HandleEdgePause()
    {
        if (!playerDetected) return;

        movementLocked = true;
        movementLockTimer = pauseAfterEdge;
    }

    void HandleMovementLock()
    {
        if (!movementLocked) return;

        movementLockTimer -= Time.deltaTime;

        if (movementLockTimer <= 0f)
        {
            movementLocked = false;
        }
    }

    // =========================
    // DEBUG
    // =========================

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange
        );

        if (player != null)
        {
            Gizmos.color =
                playerDetected ? Color.red : Color.white;

            Gizmos.DrawLine(
                transform.position,
                player.position
            );
        }
    }
}