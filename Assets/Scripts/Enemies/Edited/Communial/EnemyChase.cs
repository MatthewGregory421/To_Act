using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    [Header("References")]
    public EnemyMovement enemyMovement;

    [Header("Detection")]
    public float detectionRange = 6f;

    [Header("Line Of Sight")]
    public LayerMask lineOfSightMask;

    [Header("Chase")]
    public bool chasing;

    [Header("Chase Behaviour")]
    public float chasePauseAfterEdge = 2f;
    private float chasePauseTimer;
    private bool chaseLocked;

    void Update()
    {
        if (enemyMovement.player == null) return;

        HandleChaseLock();
        DetectPlayer();

        if (chasing && !chaseLocked)
        {
            ChasePlayer();
        }
    }

    // =========================
    // PLAYER DETECTION
    // =========================

    void DetectPlayer()
    {
        float distance = Vector2.Distance(transform.position, enemyMovement.player.position);

        if (distance <= detectionRange && HasLineOfSight())
        {
            chasing = true;
        }
        else if (!chaseLocked)
        {
            chasing = false;
        }
    }

    void OnEnable()
    {
        enemyMovement.OnHitEdge += HandleEdgeDuringChase;
    }

    void OnDisable()
    {
        enemyMovement.OnHitEdge -= HandleEdgeDuringChase;
    }

    // =========================
    // CHASE PLAYER
    // =========================

    void ChasePlayer()
    {
        float deltaX = enemyMovement.player.position.x - transform.position.x;

        if (Mathf.Abs(deltaX) < 0.4f)
        {
            enemyMovement.SetDirection(0); // full stop idle
            return;
        }

        if (deltaX > 0)
            enemyMovement.SetDirection(1);
        else
            enemyMovement.SetDirection(-1);
    }

    void HandleChaseLock()
    {
        if (!chaseLocked) return;

        chasePauseTimer -= Time.deltaTime;

        if (chasePauseTimer <= 0f)
        {
            chaseLocked = false;
        }
    }

    void HandleEdgeDuringChase()
    {
        if (!chasing) return;

        chaseLocked = true;
        chasePauseTimer = chasePauseAfterEdge;

        chasing = false;
    }

    // =========================
    // LINE OF SIGHT
    // =========================

    bool HasLineOfSight()
    {
        Vector2 direction = enemyMovement.player.position - transform.position;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            direction.normalized,
            detectionRange,
            lineOfSightMask
        );

        if (hit.collider != null)
        {
            return hit.collider.transform == enemyMovement.player;
        }

        return false;
    }

    // =========================
    // DEBUG
    // =========================

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (enemyMovement.player != null)
        {
            Gizmos.color = chasing ? Color.red : Color.white;

            Gizmos.DrawLine(
                transform.position,
                enemyMovement.player.position
            );
        }
    }
}