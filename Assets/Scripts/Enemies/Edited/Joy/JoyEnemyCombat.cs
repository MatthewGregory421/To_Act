using UnityEngine;
using System.Collections;

public class JoyEnemyCombat : MonoBehaviour
{
    [Header("References")]
    public EnemyBase enemyBase;
    public EnemyMovement enemyMovement;
    public EnemyChase chase;
    public Rigidbody2D rb;

    [Header("Ground Check")]
    public LayerMask groundLayer;

    [Header("Jump Attack")]
    public float jumpForce = 8f;
    public float horizontalBias = 0.7f;

    [Header("Windup")]
    public float windupTime = 0.6f;
    public float maxChaseDistanceDuringWindup = 8f;

    [Header("Cooldown / Recovery")]
    public float attackCooldown = 1.5f;
    public float landingRecoveryTime = 0.8f;

    [Header("Contact Damage")]
    public int damage = 1;
    public float contactCooldown = 0.6f;

    private float contactTimer;

    private enum AttackState
    {
        Idle,
        WindingUp,
        Attacking,
        Recovering
    }

    private AttackState state = AttackState.Idle;

    void Update()
    {
        if (enemyBase.isDead) return;
        if (enemyMovement.player == null) return;

        HandleContactCooldown();

        if (chase.chasing && state == AttackState.Idle)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    // =========================
    // CONTACT DAMAGE SYSTEM
    // =========================

    private void OnCollisionEnter2D(Collision2D other)
    {
        TryDealContactDamage(other.collider);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        TryDealContactDamage(other.collider);
    }

    void TryDealContactDamage(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (contactTimer > 0f) return;

        PlayerHealth ph = other.GetComponent<PlayerHealth>();
        if (ph == null) return;

        Vector2 dir = (other.transform.position - transform.position).normalized;

        ph.TakeDamage(damage, dir);

        contactTimer = contactCooldown;
    }

    void HandleContactCooldown()
    {
        if (contactTimer > 0f)
            contactTimer -= Time.deltaTime;
    }


    // =========================
    // ATTACK ROUTINE
    // =========================

    IEnumerator AttackRoutine()
    {
        state = AttackState.WindingUp;

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        float timer = 0f;

        // =========================
        // WINDUP
        // =========================
        while (timer < windupTime)
        {
            timer += Time.deltaTime;

            float distance = Vector2.Distance(transform.position, enemyMovement.player.position);

            if (distance > maxChaseDistanceDuringWindup || !chase.chasing)
            {
                state = AttackState.Idle;
                yield break;
            }

            yield return null;
        }

        // =========================
        // ATTACK
        // =========================
        state = AttackState.Attacking;

        int dir = enemyMovement.player.position.x > transform.position.x ? 1 : -1;

        Vector2 jumpDir = new Vector2(dir * horizontalBias, 1f).normalized;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(jumpDir * jumpForce, ForceMode2D.Impulse);

        // =========================
        // LANDING WAIT (SAFE)
        // =========================
        float landTimer = 0f;
        float maxWaitForLand = 2f;

        while (!IsGrounded() && landTimer < maxWaitForLand)
        {
            landTimer += Time.deltaTime;
            yield return null;
        }

        // =========================
        // RECOVERY
        // =========================
        state = AttackState.Recovering;

        yield return new WaitForSeconds(landingRecoveryTime);
        yield return new WaitForSeconds(attackCooldown);

        // =========================
        // RESET
        // =========================
        state = AttackState.Idle;
    }

    // =========================
    // HIT DETECTION (ROBUST)
    // =========================

    // =========================
    // GROUND CHECK
    // =========================

    bool IsGrounded()
    {
        return Physics2D.Raycast(
            transform.position,
            Vector2.down,
            0.2f,
            groundLayer
        );
    }
}