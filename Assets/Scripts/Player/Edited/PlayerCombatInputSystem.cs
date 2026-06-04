using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatInputSystem : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementInputSystem movement;
    public Rigidbody2D rb;

    public PlayerSFXManager playerSFXManager;

    [Header("Projectile")]
    public GameObject projectilePrefab;

    [Header("Shoot Points")]
    public Transform sideShootPoint;
    public Transform upShootPoint;
    public Transform downShootPoint;

    [Header("Attack")]
    public float attackCooldown = 0.3f;

    private float attackTimer;
    private bool attackLocked;

    private Vector2 attackInput;

    [Header("Ground Slam")]
    public float slamForce = 20f;

    private readonly HashSet<GameObject> slammedEnemies = new HashSet<GameObject>();
    private readonly HashSet<GameObject> slammedObjects = new HashSet<GameObject>();
    private bool isSlamming;

    private bool wasGrounded;

    [SerializeField] private float slamRadius = 2.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask floorLayer;

    private void Awake()
    {
        if (movement == null)
            movement = GetComponent<PlayerMovementInputSystem>();

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    // =====================================
    // UPDATE
    // =====================================

    private void Update()
    {
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        // detect landing
        if (isSlamming && !wasGrounded && movement.isGrounded)
        {
            GroundSlamImpact();
            isSlamming = false;
        }

        wasGrounded = movement.isGrounded;
    }

    // =====================================
    // INPUT
    // =====================================

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
        Vector2 shootDirection;
        Transform shootPoint;

        float y = attackInput.y;

        // UP SHOT
        if (y > 0.1f)
        {
            shootDirection = Vector2.up;
            shootPoint = upShootPoint;

            playerSFXManager.PlayPlayerAttack();
        }
        // DOWN SLAM
        else if (y < -0.1f)
        {
            playerSFXManager.PlayGroundSlam();

            if (!movement.isGrounded)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    -slamForce
                );

                isSlamming = true;
                slammedEnemies.Clear();
                slammedObjects.Clear();
            }

            return;
        }
        // NORMAL SHOT (FACE DIRECTION)
        else
        {
            shootDirection = movement.facingDirection == 1
                ? Vector2.right
                : Vector2.left;

            shootPoint = sideShootPoint;

            playerSFXManager.PlayPlayerAttack();
        }

        ShootProjectile(
            shootPoint.position,
            shootDirection
        );
    }

    private void ShootProjectile(Vector2 spawnPosition, Vector2 direction)
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("No projectile prefab assigned!");
            return;
        }

        GameObject projectile = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        PlayerProjectile projectileScript =
            projectile.GetComponent<PlayerProjectile>();

        if (projectileScript != null)
        {
            projectileScript.SetDirection(direction);
        }
    }

    private void GroundSlamImpact()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            slamRadius
        );

        foreach (Collider2D hit in hits)
        {
            GameObject obj = hit.gameObject;

            if (slammedObjects.Contains(obj))
                continue;

            slammedObjects.Add(obj);

            // =========================
            // 1. SHIELD PRIORITY
            // =========================
            SadnessShield shield = obj.GetComponentInParent<SadnessShield>();
            if (shield != null && shield.isActive)
            {
                shield.BreakShield();
                continue;
            }

            // =========================
            // 2. ENEMY DAMAGE
            // =========================
            EnemyBase enemy = obj.GetComponentInParent<EnemyBase>();
            if (enemy != null)
            {
                Vector2 dir = (enemy.transform.position - transform.position).normalized;
                enemy.TakeDamage(1, dir);
                continue;
            }

            // =========================
            // 3. DESTRUCTIBLE FLOOR
            // =========================
            DestructibleFloor floor = obj.GetComponent<DestructibleFloor>();
            if (floor != null)
            {
                floor.TakeSlamHit(1);
                continue;
            }
        }
    }
}
