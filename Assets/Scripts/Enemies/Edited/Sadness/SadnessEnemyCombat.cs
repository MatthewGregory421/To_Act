using UnityEngine;

public class SadnessEnemyCombat : MonoBehaviour
{
    [Header("References")]
    public EnemyBase enemyBase;
    public EnemyMovement enemyMovement;
    public EnemyAnimations animations;
    public EnemyProjectileSpawner projectileSpawner;

    private EnemySFXManager SFX => EnemySFXManager.Instance;

    [Header("Detection")]
    public float detectionRange = 8f;

    [Header("Line Of Sight")]
    public LayerMask lineOfSightMask;

    [Header("Spacing")]
    public float preferredDistance = 5f;
    public float retreatDistance = 3f;

    [Header("Behaviour")]
    public bool playerDetected;
    private bool wasPlayerDetected;

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
        enemyMovement.OnHitEdge += HandleEdgePause;
        animations.EnemyAttack();
        projectileSpawner.onShoot += PlayShootSFX;

        enemyBase.OnDamaged += HandleDamageSFX;
    }

    private void OnDisable()
    {
        enemyMovement.OnHitEdge -= HandleEdgePause;

        projectileSpawner.onShoot -= PlayShootSFX;

        enemyBase.OnDamaged -= HandleDamageSFX;
    }

    private void Update()
    {
        if (enemyBase.isDead) return;
        if (enemyMovement.player == null) return;

        HandleMovementLock();

        DetectPlayer();

        // enable / disable shooting
        projectileSpawner.enabled = playerDetected;

        if (!playerDetected && wasPlayerDetected)
        {
            SFX?.PlaySadnessIdle();
        }

        wasPlayerDetected = playerDetected;

        if (!playerDetected)
        {
            enemyMovement.canMove = true;
            return;
        }

        if (movementLocked)
        {
            enemyMovement.canMove = false;
            return;
        }

        HandleSpacing();
    }

    void HandleDamageSFX()
    {
        SFX?.PlaySadnessDamage();
    }

    void PlayShootSFX()
    {
        if (!playerDetected) return;
        SFX?.PlaySadnessAttack();
    }

    // =========================
    // PLAYER DETECTION
    // =========================

    void DetectPlayer()
    {
        float distance = Vector2.Distance(
            transform.position,
            enemyMovement.player.position
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
            enemyMovement.player.position - transform.position;

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
    // DISTANCE CONTROL
    // =========================

    void HandleSpacing()
    {
        float deltaX = enemyMovement.player.position.x - transform.position.x;
        float distance = Mathf.Abs(deltaX);

        // ALWAYS face player while active
        FacePlayer(deltaX);

        // =========================
        // EDGE PRIORITY (HIGHEST)
        // =========================
        bool atEdge = !HasGroundBehind();

        if (atEdge)
        {
            enemyMovement.canMove = false;
            return;
        }

        // =========================
        // TOO CLOSE -> RETREAT
        // =========================
        if (distance < retreatDistance)
        {
            int dir = (deltaX > 0) ? -1 : 1;

            enemyMovement.SetDirection(dir);
            enemyMovement.canMove = true;
            return;
        }

        // =========================
        // TOO FAR -> APPROACH
        // =========================
        if (distance > preferredDistance)
        {
            int dir = (deltaX > 0) ? 1 : -1;

            enemyMovement.SetDirection(dir);
            enemyMovement.canMove = true;
            return;
        }

        // =========================
        // IDEAL RANGE -> STOP
        // =========================
        enemyMovement.canMove = false;
    }

    void FacePlayer(float deltaX)
    {
        if (deltaX > 0)
            enemyMovement.SetDirection(1);
        else
            enemyMovement.SetDirection(-1);

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

        if (enemyMovement.player != null)
        {
            Gizmos.color =
                playerDetected ? Color.red : Color.white;

            Gizmos.DrawLine(
                transform.position,
                enemyMovement.player.position
            );
        }
    }
}