using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatInputSystem : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementInputSystem movement;
    public Rigidbody2D rb;

    [Header("Attack")]
    public float attackRange = 1f;
    public int attackDamage = 1;
    public LayerMask enemyLayer;

    private bool attackLocked;

    [Header("Attack Cooldown")]
    public float attackCooldown = 0.3f;
    private float attackTimer;

    private Vector2 attackInput;
    private bool attackHeld;

    [Header("Attack Points")]
    public Transform sideAttackPoint;
    public Transform upAttackPoint;
    public Transform downAttackPoint;

    [Header("Ground Slam")]
    public float slamForce = 20f;

    [Header("Debug")]
    public GameObject debugAttackPrefab;
    public float debugLifeTime = 0.2f;

    private void Awake()
    {
        if (movement == null)
            movement = GetComponent<PlayerMovementInputSystem>();

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    // =====================================
    // INPUT
    // =====================================

    private void Update()
    {
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;
    }

    public void Attack(InputAction.CallbackContext context)
    {
        attackInput = context.ReadValue<Vector2>();

        // PRESS
        if (context.started && !attackLocked)
        {
            TryAttack();
            attackLocked = true;
        }

        // RELEASE
        if (context.canceled)
        {
            attackLocked = false;
        }
    }

    // =====================================
    // ATTACK
    // =====================================

    private void TryAttack()
    {
        if (attackTimer > 0)
            return;

        PerformAttack();
        attackTimer = attackCooldown;
    }

    private void PerformAttack()
    {
        Transform attackPoint = sideAttackPoint;

        Vector2 attackDirection = Vector2.right;

        float x = attackInput.x;
        float y = attackInput.y;

        // PRIORITY: UP
        if (y > 0.1f)
        {
            attackDirection = Vector2.up;
            attackPoint = upAttackPoint;
        }
        // DOWN (slam)
        else if (y < -0.1f)
        {
            attackDirection = Vector2.down;
            attackPoint = downAttackPoint;

            if (!movement.isGrounded)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    -slamForce
                );
            }
        }
        // LEFT / RIGHT
        else if (x > 0.1f)
        {
            attackDirection = Vector2.right;
        }
        else if (x < -0.1f)
        {
            attackDirection = Vector2.left;
        }
        // NO INPUT - use facing direction
        else
        {
            attackDirection = movement.facingDirection == 1
                ? Vector2.right
                : Vector2.left;
        }

        // DEBUG VISUAL
        SpawnDebugAttack(attackPoint.position);

        // HIT CHECK
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        foreach (Collider2D enemy in hits)
        {
            Debug.Log("Hit: " + enemy.name);

            // enemy.GetComponent<EnemyHealth>()?.TakeDamage(attackDamage);
        }

        Debug.DrawRay(
            attackPoint.position,
            attackDirection * attackRange,
            Color.red,
            0.5f
        );
    }

    private void SpawnDebugAttack(Vector3 position)
    {
        if (debugAttackPrefab == null)
            return;

        GameObject obj = Instantiate(
            debugAttackPrefab,
            position,
            Quaternion.identity
        );

        Destroy(obj, debugLifeTime);
    }

    // =====================================
    // GIZMOS
    // =====================================

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (sideAttackPoint != null)
        {
            Gizmos.DrawWireSphere(
                sideAttackPoint.position,
                attackRange
            );
        }

        if (upAttackPoint != null)
        {
            Gizmos.DrawWireSphere(
                upAttackPoint.position,
                attackRange
            );
        }

        if (downAttackPoint != null)
        {
            Gizmos.DrawWireSphere(
                downAttackPoint.position,
                attackRange
            );
        }
    }
}
