using UnityEngine;
using System.Collections;

public class AngerEnemyCombat : MonoBehaviour
{
    [Header("References")]
    public EnemyMovement enemyMovement;
    public EnemyChase enemyChase;

    [Header("Attack Settings")]
    public float attackRange = 2f;
    public int damage = 1;

    [Header("Timing")]
    public float windUpTime = 0.4f;
    public float attackCooldown = 1.2f;

    private bool isAttacking;
    private float cooldownTimer;

    void Update()
    {
        if (enemyMovement.player == null || enemyMovement == null) return;
        if (enemyMovement.isDead) return;

        cooldownTimer -= Time.deltaTime;

        float distance = Vector2.Distance(transform.position, enemyMovement.player.position);

        bool inRange = distance <= attackRange;
        bool canAttack = cooldownTimer <= 0f;

        // HARD OVERRIDE: if in range, STOP movement instantly
        if (inRange && !isAttacking && canAttack && enemyChase.chasing)
        {
            enemyMovement.SetDirection(0);
            enemyMovement.canMove = false;

            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // WIND UP
        yield return new WaitForSeconds(windUpTime);

        float distance = Vector2.Distance(transform.position, enemyMovement.player.position);

        if (distance <= attackRange)
        {
            DealDamage();
        }

        // RECOVER
        yield return new WaitForSeconds(0.1f);

        enemyMovement.canMove = true;
        isAttacking = false;

        cooldownTimer = attackCooldown;
    }

    void DealDamage()
    {
        Vector2 hitDir = (enemyMovement.player.position - transform.position).normalized;

        var playerHealth = enemyMovement.player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage, hitDir);
        }
    }
}