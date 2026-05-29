using UnityEngine;
using System.Collections;

public class AngerEnemyCombat_Tier2 : MonoBehaviour
{
    [Header("References")]
    public EnemyMovement enemyMovement;
    public EnemyChase enemyChase;
    public Transform player;
    public Rigidbody2D rb;

    [Header("Attack Settings")]

    public int normalDamage = 1;
    public int chargeDamage = 2;

    [Header("Ranges")]
    public float normalAttackRange = 2f;
    public float chargeAttackRange = 6f;

    [Header("Charge Hit")]
    public float chargeHitRadius = 1.8f;

    [Header("Charge Settings")]
    public float chargeSpeed = 10f;
    public float chargeTime = 0.6f;
    public float chargeWindUpTime = 0.8f;

    [Header("Timing")]
    public float windUpTime = 0.4f;
    public float attackCooldown = 1.5f;

    private bool isAttacking;
    private bool isCharging;
    private float cooldownTimer;

    void Update()
    {
        if (player == null || enemyMovement == null) return;
        if (enemyMovement.isDead) return;

        cooldownTimer -= Time.deltaTime;

        float distance = Vector2.Distance(transform.position, player.position);

        bool inNormalRange = distance <= normalAttackRange;
        bool inChargeRange = distance <= chargeAttackRange;
        bool canAttack = cooldownTimer <= 0f;

        // HARD LOCK: nothing can interrupt charge once started
        if (isCharging) return;

        if (!isAttacking && canAttack && enemyChase.chasing)
        {
            enemyMovement.SetDirection(0);
            enemyMovement.canMove = false;

            float roll = Random.value;

            // Charge happens from FARTHER away
            if (inChargeRange && roll < 0.5f)
                StartCoroutine(ChargeAttack());
            else if (inNormalRange)
                StartCoroutine(NormalAttack());
        }
    }

    // =========================
    // NORMAL ATTACK
    // =========================
    IEnumerator NormalAttack()
    {
        isAttacking = true;

        yield return new WaitForSeconds(windUpTime);

        TryDealDamage(normalDamage, 1.2f);

        yield return new WaitForSeconds(0.1f);

        EndAttack();
    }

    // =========================
    // CHARGE ATTACK (LOCKED)
    // =========================
    IEnumerator ChargeAttack()
    {
        isAttacking = true;
        isCharging = true;

        // wind-up telegraph
        yield return new WaitForSeconds(chargeWindUpTime);

        Vector2 dir = (player.position - transform.position).normalized;

        float timer = 0f;

        // FULL LOCK CHARGE
        while (timer < chargeTime)
        {
            rb.linearVelocity = dir * chargeSpeed;

            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= chargeHitRadius)
        {
            TryDealDamage(chargeDamage, 2f);
        }

        yield return new WaitForSeconds(0.1f);

        EndAttack();
        isCharging = false;
    }

    // =========================
    // DAMAGE
    // =========================
    void TryDealDamage(int dmg, float knockback)
    {
        Vector2 hitDir = (player.position - transform.position).normalized;

        var playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(dmg, hitDir);
        }
    }

    // =========================
    // RESET
    // =========================
    void EndAttack()
    {
        enemyMovement.canMove = true;
        enemyMovement.SetDirection(0);

        isAttacking = false;
        cooldownTimer = attackCooldown;
    }
}