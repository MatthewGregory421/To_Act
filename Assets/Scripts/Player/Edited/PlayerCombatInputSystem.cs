using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class PlayerCombatInputSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerAnimations anim;
    public PlayerMovementInputSystem movement;
    public Rigidbody2D rb;

    public PlayerSFXManager playerSFXManager;

    [Header("Projectile")]
    public GameObject projectilePrefab;

    [Header("Shoot Points")]
    public Transform sideShootPoint;
    public Transform downShootPoint;

    [Header("Attack")]
    public float attackCooldown = 0.3f;

    private float attackTimer;
    private bool attackLocked;
    private Vector2 attackInput;

    [Header("Ground Slam")]
    public float slamForce = 20f;

    [SerializeField] private float slamBounceForce = 4f;

    // Controls how far the slam can damage enemies.
    [SerializeField] private float slamRadius = 1.25f;

    // Moves the damage circle down towards the player's feet.
    [SerializeField]
    private Vector2 slamImpactOffset =
        new Vector2(0f, -0.75f);

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask floorLayer;

    private readonly HashSet<GameObject> slammedEnemies =
        new HashSet<GameObject>();

    private readonly HashSet<GameObject> slammedObjects =
        new HashSet<GameObject>();

    private bool isSlamming;
    private bool wasGrounded;

    [Header("Ground Slam Cooldown")]
    [SerializeField] private float groundSlamCooldown = 3f;
    [SerializeField] private float groundSlamCooldownTimer;
    [SerializeField] private CoolDownIconUI groundSlamCooldownUI;

    [Header("Tilemap Destruction")]
    [SerializeField] private Tilemap destructibleTilemap;

    [SerializeField]
    private string destructibleTilemapName =
        "DestructibleTilemap";

    [SerializeField] private int slamTileBreakRadius = 2;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        FindDestructibleTilemap();
    }

    private void Awake()
    {
        if (movement == null)
            movement = GetComponent<PlayerMovementInputSystem>();

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        FindDestructibleTilemap();

        if (groundSlamCooldownUI == null)
        {
            groundSlamCooldownUI =
                GameObject.Find("GroundSlamFill")
                    ?.GetComponent<CoolDownIconUI>();
        }
    }

    private void FindDestructibleTilemap()
    {
        GameObject found =
            GameObject.Find(destructibleTilemapName);

        if (found != null)
        {
            destructibleTilemap =
                found.GetComponent<Tilemap>();
        }
        else
        {
            destructibleTilemap = null;
        }
    }

    // =====================================
    // UPDATE
    // =====================================

    private void Update()
    {
        if (anim != null)
            anim.groundslam = isSlamming;

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        HandleGroundSlamCooldown();

        // Detect when the player lands during a slam.
        if (
            isSlamming &&
            !wasGrounded &&
            movement.isGrounded
        )
        {
            GroundSlamImpact();
            isSlamming = false;
        }

        wasGrounded = movement.isGrounded;
    }

    // =====================================
    // INPUT
    // =====================================

    public void Attack(
        InputAction.CallbackContext context
    )
    {
        attackInput =
            context.ReadValue<Vector2>();

        if (context.started && !attackLocked)
        {
            TryAttack();
            attackLocked = true;
        }

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
        if (attackTimer > 0f)
            return;

        PerformAttack();
        attackTimer = attackCooldown;
    }

    private void PerformAttack()
    {
        float y = attackInput.y;

        // =====================================
        // UP INPUT — FACING-DIRECTION SHOT
        // =====================================

        if (y > 0.1f)
        {
            Vector2 shootDirection =
                movement.facingDirection == 1
                    ? Vector2.right
                    : Vector2.left;

            if (sideShootPoint == null)
            {
                Debug.LogWarning(
                    "No side shoot point assigned."
                );

                return;
            }

            if (playerSFXManager != null)
                playerSFXManager.PlayPlayerAttack();

            if (anim != null)
                anim.Attack();

            ShootProjectile(
                sideShootPoint.position,
                shootDirection
            );

            return;
        }

        // =====================================
        // DOWN INPUT — GROUND SLAM
        // =====================================

        if (y < -0.1f)
        {
            if (!movement.hasGroundSlam)
                return;

            if (groundSlamCooldownTimer > 0f)
                return;

            if (movement.isGrounded)
                return;

            if (playerSFXManager != null)
                playerSFXManager.PlayGroundSlam();

            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                -slamForce
            );

            isSlamming = true;

            slammedEnemies.Clear();
            slammedObjects.Clear();

            groundSlamCooldownTimer =
                groundSlamCooldown;

            if (groundSlamCooldownUI != null)
            {
                groundSlamCooldownUI
                    .SetCooldownProgress(
                        groundSlamCooldownTimer,
                        groundSlamCooldown
                    );
            }
        }
    }

    private void ShootProjectile(
        Vector2 spawnPosition,
        Vector2 direction
    )
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning(
                "No projectile prefab assigned!"
            );

            return;
        }

        GameObject projectile =
            Instantiate(
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

    // =====================================
    // GROUND SLAM IMPACT
    // =====================================

    private void GroundSlamImpact()
    {
        Vector2 impactPosition =
            (Vector2)transform.position +
            slamImpactOffset;

        /*
         * Only look for colliders on the Enemy and
         * Floor layers.
         *
         * The MusicDetection layer must not be
         * included in either mask.
         */
        int slamLayerMask =
            enemyLayer.value |
            floorLayer.value;

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                impactPosition,
                slamRadius,
                slamLayerMask
            );

        foreach (Collider2D hit in hits)
        {
            if (hit == null)
                continue;

            /*
             * Music detection colliders are triggers,
             * so the slam ignores trigger colliders.
             *
             * This prevents a large detection circle
             * from counting as the enemy's body.
             */
            if (hit.isTrigger)
                continue;

            GameObject obj = hit.gameObject;

            // Extra protection in case the detection
            // collider is accidentally on the wrong layer.
            if (obj.CompareTag("MusicDetectionTrigger"))
                continue;

            // =========================
            // ENEMY OR SHIELD
            // =========================

            EnemyBase enemy =
                obj.GetComponentInParent<EnemyBase>();

            if (enemy != null)
            {
                GameObject enemyRoot =
                    enemy.gameObject;

                /*
                 * Stops an enemy with multiple body
                 * colliders taking damage repeatedly.
                 */
                if (!slammedEnemies.Add(enemyRoot))
                    continue;

                SadnessShield shield =
                    enemy.GetComponentInChildren<
                        SadnessShield
                    >();

                if (
                    shield != null &&
                    shield.isActive
                )
                {
                    shield.BreakShield();
                    continue;
                }

                Vector2 direction =
                    (
                        (Vector2)enemy.transform.position -
                        impactPosition
                    ).normalized;

                enemy.TakeDamage(
                    1,
                    direction
                );

                continue;
            }

            // =========================
            // DESTRUCTIBLE FLOOR OBJECT
            // =========================

            DestructibleFloor floor =
                obj.GetComponentInParent<
                    DestructibleFloor
                >();

            if (floor != null)
            {
                GameObject floorRoot =
                    floor.gameObject;

                /*
                 * Stops a destructible object with
                 * multiple colliders taking repeated hits.
                 */
                if (!slammedObjects.Add(floorRoot))
                    continue;

                floor.TakeSlamHit(1);
            }
        }

        BreakTileBelowPlayer();

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            slamBounceForce
        );
    }

    // =====================================
    // GROUND SLAM COOLDOWN
    // =====================================

    private void HandleGroundSlamCooldown()
    {
        if (groundSlamCooldownTimer <= 0f)
            return;

        groundSlamCooldownTimer -=
            Time.deltaTime;

        if (groundSlamCooldownUI != null)
        {
            groundSlamCooldownUI
                .SetCooldownProgress(
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

    // =====================================
    // TILEMAP DESTRUCTION
    // =====================================

    private void BreakTileBelowPlayer()
    {
        if (destructibleTilemap == null)
        {
            Debug.LogWarning(
                "No destructible tilemap assigned."
            );

            return;
        }

        Vector3 hitPosition =
            transform.position +
            Vector3.down * 1.1f;

        Vector3Int centerCell =
            destructibleTilemap.WorldToCell(
                hitPosition
            );

        for (
            int x = -slamTileBreakRadius;
            x <= slamTileBreakRadius;
            x++
        )
        {
            for (
                int y = -slamTileBreakRadius;
                y <= slamTileBreakRadius;
                y++
            )
            {
                Vector3Int cellPosition =
                    new Vector3Int(
                        centerCell.x + x,
                        centerCell.y + y,
                        centerCell.z
                    );

                if (
                    destructibleTilemap.HasTile(
                        cellPosition
                    )
                )
                {
                    destructibleTilemap.SetTile(
                        cellPosition,
                        null
                    );
                }
            }
        }

        destructibleTilemap.RefreshAllTiles();
    }

    // =====================================
    // EDITOR DEBUG DISPLAY
    // =====================================

    private void OnDrawGizmosSelected()
    {
        Vector3 impactPosition =
            transform.position +
            (Vector3)slamImpactOffset;

        Gizmos.DrawWireSphere(
            impactPosition,
            slamRadius
        );
    }
}