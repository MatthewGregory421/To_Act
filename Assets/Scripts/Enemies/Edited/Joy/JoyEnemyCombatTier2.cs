using UnityEngine;
using System.Collections;
using System.Dynamic;

public class JoyEnemyCombatTier2 : MonoBehaviour
{
    [Header("References")]
    public EnemyBase enemyBase;
    public EnemyChase chase;
    public Rigidbody2D rb;
    public Transform player;

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

    [Header("Laugh Attack")]
    public float laughWindup = 1.5f;
    public float laughRange = 5f;
    public float stunDuration = 2f;

    [Header("Laugh Frequency")]
    public int minAttacksBeforeLaugh = 4;
    public int maxAttacksBeforeLaugh = 7;

    private int attacksUntilLaugh;

    private float contactTimer;

    private enum AttackState
    {
        Idle,
        WindingUp,
        Attacking,
        Recovering
    }

    private AttackState state = AttackState.Idle;

    private void Start()
    {
        ResetLaughCounter();
    }

    void Update()
    {
        if (enemyBase.isDead) return;
        if (player == null) return;

        HandleContactCooldown();

        if (chase.chasing && state == AttackState.Idle)
        {
            ChooseAttack();
        }
    }

    // =========================
    // ATTACK SELECTION
    // =========================

    void ChooseAttack()
    {
        // Force laugh attack after enough jumps
        if (attacksUntilLaugh <= 0)
        {
            float distance =
                Vector2.Distance(transform.position, player.position);

            // only laugh if player is close enough
            if (distance <= laughRange)
            {
                ResetLaughCounter();
                StartCoroutine(LaughAttackRoutine());
                return;
            }
        }

        // Otherwise jump attack
        attacksUntilLaugh--;

        StartCoroutine(JumpAttackRoutine());
    }

    void ResetLaughCounter()
    {
        attacksUntilLaugh =
            Random.Range(minAttacksBeforeLaugh,
                         maxAttacksBeforeLaugh + 1);
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
    // JUMP ATTACK
    // =========================

    IEnumerator JumpAttackRoutine()
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

            float distance = Vector2.Distance(transform.position, player.position);

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

        int dir = player.position.x > transform.position.x ? 1 : -1;

        Vector2 jumpDir = new Vector2(dir * horizontalBias, 1f).normalized;

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(jumpDir * jumpForce, ForceMode2D.Impulse);

        // =========================
        // LANDING WAIT
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
    // LAUGH ATTACK
    // =========================

    IEnumerator LaughAttackRoutine()
    {
        state = AttackState.WindingUp;

        // stop movement
        rb.linearVelocity = Vector2.zero;

        // disable chase temporarily
        chase.enabled = false;

        // =========================
        // LAUGH WINDUP
        // =========================

        // PLAY LAUGH ANIMATION HERE
        // PLAY LAUGH SOUND HERE

        yield return new WaitForSeconds(laughWindup);

        // =========================
        // STUN PLAYER
        // =========================

        float distance =
            Vector2.Distance(transform.position, player.position);

        if (distance <= laughRange)
        {
            PlayerMovementInputSystem movement =
                player.GetComponent<PlayerMovementInputSystem>();

            if (movement != null)
            {
                movement.Stun(stunDuration);
            }
        }

        // =========================
        // RECOVERY
        // =========================

        state = AttackState.Recovering;

        yield return new WaitForSeconds(attackCooldown);

        chase.enabled = true;

        state = AttackState.Idle;
    }

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
