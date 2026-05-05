using UnityEngine;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviour
{
    private PlayerMovement_E movement;
    public PlayerSFXManager sfxManager;

    [Header("Attack Settings")]
    public float attackCooldown = 0.3f;
    private float attackTimer;

    [Header("Debug Attack Prefab")]
    public GameObject attackPrefab;
    public float attackDistance = 1f;

    [Header("Ground Slam Settings")]
    public int groundSlamCount = 4;
    public float groundSlamSpacing = 0.5f;

    [Header("Knockback Settings")]
    public float normalHorizontalForce = 6f;
    public float normalVerticalForce = 2f;

    public float slamHorizontalForce = 10f;
    public float slamVerticalForce = 6f;

    [Header("Floating Text")]
    public GameObject floatingTextPrefab;

    private HashSet<BaseEnemy> enemiesHit = new HashSet<BaseEnemy>();

    private int lastHorizontal = 1;

    void Start()
    {
        movement = GetComponent<PlayerMovement_E>();
    }

    void Update()
    {
        UpdateLastHorizontal();
        HandleAttack();
    }

    void UpdateLastHorizontal()
    {
        if (movement.lookHorizontal != 0)
            lastHorizontal = movement.lookHorizontal;
    }

    void HandleAttack()
    {
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        int ver = movement.lookVertical;

        if (!movement.isGrounded && ver < 0 && Input.GetKeyDown(KeyCode.D) && !movement.isGroundSlamming)
        {
            enemiesHit.Clear();
            movement.isGroundSlamming = true;
            sfxManager.PlayPlayerGroundSlamEmitter();
            movement.rb.linearVelocity = Vector2.zero;
            movement.SetEnemyCollision(false);
            attackTimer = attackCooldown;
            return;
        }

        if (Input.GetKeyDown(KeyCode.D) && attackTimer <= 0)
        {
            enemiesHit.Clear();
            PerformAttack();
            attackTimer = attackCooldown;
        }
    }

    void PerformAttack()
    {
        int hor = movement.lookHorizontal;
        int ver = movement.lookVertical;

        string attackDirection = "Neutral";

        if (ver > 0) attackDirection = "Up";
        else if (ver < 0) attackDirection = "Down";
        else if (hor != 0) attackDirection = hor < 0 ? "Left" : "Right";
        else attackDirection = lastHorizontal < 0 ? "Left" : "Right";

        Vector2 dir = Vector2.zero;
        if (attackDirection == "Left") dir = Vector2.left;
        else if (attackDirection == "Right") dir = Vector2.right;
        else if (attackDirection == "Up") dir = Vector2.up;
        else if (attackDirection == "Down") dir = Vector2.down;

        Vector3 spawnPos = transform.position + (Vector3)(dir * attackDistance);
        GameObject indicator = Instantiate(attackPrefab, spawnPos, Quaternion.identity);
        Destroy(indicator, 0.5f);

        sfxManager.PlayPlayerAttackEmitter();

        // Damage check
        Collider2D[] hits = Physics2D.OverlapCircleAll(spawnPos, 0.5f);
        foreach (Collider2D hit in hits)
        {
            BaseEnemy enemy = hit.GetComponent<BaseEnemy>();
            if (enemy != null && !enemiesHit.Contains(enemy))
            {
                enemiesHit.Add(enemy);

                Vector2 knockDir = ((Vector2)(enemy.transform.position - transform.position)).normalized;
                enemy.TakeDamage(1, knockDir); // <-- APPLY DAMAGE + KNOCKBACK + FLASH\

                // Spawn floating text above enemy
                Vector3 textPos = enemy.transform.position + Vector3.up * 1.5f;
                Instantiate(floatingTextPrefab, textPos, Quaternion.identity);
            }
        }

        Debug.Log("Attack Direction: " + attackDirection);
    }

    void GroundSlam()
    {
        Debug.Log("Ground Slam!");

        for (int i = 1; i <= groundSlamCount; i++)
        {
            float offset = i * groundSlamSpacing;

            Vector3 leftPos = transform.position + Vector3.left * offset;
            GameObject leftIndicator = Instantiate(attackPrefab, leftPos, Quaternion.identity);
            Destroy(leftIndicator, 0.5f);

            Vector3 rightPos = transform.position + Vector3.right * offset;
            GameObject rightIndicator = Instantiate(attackPrefab, rightPos, Quaternion.identity);
            Destroy(rightIndicator, 0.5f);
        }
    }

    public void SpawnGroundSlamIndicators()
    {
        GroundSlam();

        Debug.Log("Ground Slam Impact!");

        Collider2D[] centerHits = Physics2D.OverlapCircleAll(transform.position, 0.6f);
        foreach (Collider2D hit in centerHits)
        {
            BaseEnemy enemy = hit.GetComponent<BaseEnemy>();
            if (enemy != null && !enemiesHit.Contains(enemy))
            {
                enemiesHit.Add(enemy);
                Vector2 dir = Vector2.down;
                enemy.TakeDamage(1, dir); // <-- slam damage + knockback

                // Spawn floating text above enemy
                Vector3 textPos = enemy.transform.position + Vector3.up * 1.5f;
                Instantiate(floatingTextPrefab, textPos, Quaternion.identity);
            }
        }

        for (int i = 1; i <= groundSlamCount; i++)
        {
            float offset = i * groundSlamSpacing;

            // LEFT
            Vector3 leftPos = transform.position + Vector3.left * offset;
            Collider2D[] leftHits = Physics2D.OverlapCircleAll(leftPos, 0.5f);
            foreach (Collider2D hit in leftHits)
            {
                BaseEnemy enemy = hit.GetComponent<BaseEnemy>();
                if (enemy != null && !enemiesHit.Contains(enemy))
                {
                    enemiesHit.Add(enemy);
                    Vector2 dir = ((Vector2)(enemy.transform.position - transform.position)).normalized;
                    enemy.TakeDamage(1, dir);

                    // Spawn floating text above enemy
                    Vector3 textPos = enemy.transform.position + Vector3.up * 1.5f;
                    Instantiate(floatingTextPrefab, textPos, Quaternion.identity);
                }
            }

            // RIGHT
            Vector3 rightPos = transform.position + Vector3.right * offset;
            Collider2D[] rightHits = Physics2D.OverlapCircleAll(rightPos, 0.5f);
            foreach (Collider2D hit in rightHits)
            {
                BaseEnemy enemy = hit.GetComponent<BaseEnemy>();
                if (enemy != null && !enemiesHit.Contains(enemy))
                {
                    enemiesHit.Add(enemy);
                    Vector2 dir = ((Vector2)(enemy.transform.position - transform.position)).normalized;
                    enemy.TakeDamage(1, dir);

                    // Spawn floating text above enemy
                    Vector3 textPos = enemy.transform.position + Vector3.up * 1.5f;
                    Instantiate(floatingTextPrefab, textPos, Quaternion.identity);
                }
            }
        }
    }
}