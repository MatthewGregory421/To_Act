using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatInputSystem : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementInputSystem movement;
    public Rigidbody2D rb;

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
        }
        // DOWN SLAM
        else if (y < -0.1f)
        {
            shootDirection = Vector2.down;
            shootPoint = downShootPoint;

            // Ground slam only in air
            if (!movement.isGrounded)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    -slamForce
                );
            }
        }
        // NORMAL SHOT (FACE DIRECTION)
        else
        {
            shootDirection = movement.facingDirection == 1
                ? Vector2.right
                : Vector2.left;

            shootPoint = sideShootPoint;
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
}
