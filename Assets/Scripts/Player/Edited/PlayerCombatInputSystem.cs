using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class PlayerCombatInputSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerAnimations anim;
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
    [SerializeField] private float slamBounceForce = 4f;

    private readonly HashSet<GameObject> slammedEnemies = new HashSet<GameObject>();
    private readonly HashSet<GameObject> slammedObjects = new HashSet<GameObject>();
    private bool isSlamming;

    private bool wasGrounded;

    [Header("Ground Slam Cooldown")]
    [SerializeField] private float groundSlamCooldown = 3f;
    [SerializeField] private float groundSlamCooldownTimer;
    [SerializeField] private CoolDownIconUI groundSlamCooldownUI;

    [Header("Tilemap Destruction")]
    [SerializeField] private Tilemap destructibleTilemap;
    [SerializeField] private string destructibleTilemapName = "DestructibleTilemap";
    [SerializeField] private int slamTileBreakRadius = 2;

    [SerializeField] private float slamRadius = 2.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask floorLayer;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindDestructibleTilemap();
    }

    private void FindDestructibleTilemap()
    {
        GameObject found = GameObject.Find(destructibleTilemapName);

        if (found != null)
        {
            destructibleTilemap = found.GetComponent<Tilemap>();
        }
        else
        {
            destructibleTilemap = null;
        }
    }

    private void Awake()
    {
        if (movement == null)
            movement = GetComponent<PlayerMovementInputSystem>();

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        FindDestructibleTilemap();

        if (groundSlamCooldownUI == null)
            groundSlamCooldownUI = GameObject.Find("GroundSlamFill")?.GetComponent<CoolDownIconUI>();
    }

    // =====================================
    // UPDATE
    // =====================================

    private void Update()
    {
        anim.groundslam = isSlamming;
        
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        HandleGroundSlamCooldown();

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
            if (!movement.hasGroundSlam)
                return;

            if (groundSlamCooldownTimer > 0f)
                return;

            if (movement.isGrounded)
                return;

            playerSFXManager.PlayGroundSlam();

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -slamForce);

            isSlamming = true;
            
            slammedEnemies.Clear();
            slammedObjects.Clear();

            groundSlamCooldownTimer = groundSlamCooldown;

            if (groundSlamCooldownUI != null)
            {
                groundSlamCooldownUI.SetCooldownProgress(
                    groundSlamCooldownTimer,
                    groundSlamCooldown
                );
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

        anim.Attack();

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
        BreakTileBelowPlayer();

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, slamBounceForce);
    }

    private void HandleGroundSlamCooldown()
    {
        if (groundSlamCooldownTimer <= 0f)
            return;

        groundSlamCooldownTimer -= Time.deltaTime;

        if (groundSlamCooldownUI != null)
        {
            groundSlamCooldownUI.SetCooldownProgress(
                groundSlamCooldownTimer,
                groundSlamCooldown
            );
        }

        if (groundSlamCooldownTimer <= 0f)
        {
            groundSlamCooldownTimer = 0f;

            if (groundSlamCooldownUI != null)
            {
                groundSlamCooldownUI.SetReady();
            }
        }
    }

    private void BreakTileBelowPlayer()
    {
        if (destructibleTilemap == null)
        {
            Debug.LogWarning("No destructible tilemap assigned.");
            return;
        }

        Vector3 hitPosition = transform.position + Vector3.down * 1.1f;
        Vector3Int centerCell = destructibleTilemap.WorldToCell(hitPosition);

        for (int x = -slamTileBreakRadius; x <= slamTileBreakRadius; x++)
        {
            for (int y = -slamTileBreakRadius; y <= slamTileBreakRadius; y++)
            {
                Vector3Int cellPosition = new Vector3Int(
                    centerCell.x + x,
                    centerCell.y + y,
                    centerCell.z
                );

                if (destructibleTilemap.HasTile(cellPosition))
                {
                    destructibleTilemap.SetTile(cellPosition, null);
                }
            }
        }

        destructibleTilemap.RefreshAllTiles();
    }
}
