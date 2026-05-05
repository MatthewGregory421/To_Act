using UnityEngine;
using FMODUnity;

public class RandomBehaviorAttack : EnemyAttack
{
    public SadnessSFXManager sadnessSFXManager;

    [Header("Behaviour Settings")]
    public float decisionCooldown = 1.5f;
    private float nextDecisionTime;

    private enum ActionState { Idle, BackingUp, Charging, IdleMove, Attacking }
    private ActionState currentState = ActionState.Idle;

    [Header("Movement")]
    public float chargeSpeed = 4f;
    public float maxChargeDistance = 3f;

    [Header("Back Away Settings")]
    public float backAwayDistance = 3f;  // how far to move backwards
    public float backAwaySpeed = 2f;     // speed while backing up

    private bool backAwayInitialized = false;
    private Vector2 backAwayStartPos;
    private int backAwayDirection; // +1 = right, -1 = left

    [Header("Action Cooldowns")]
    public float chargeCooldown = 3f;
    public float backAwayCooldown = 2f;
    public float normalAttackCooldown = 1f;

    private float lastChargeTime;
    private float lastBackAwayTime;
    private bool hasAttackedDuringCharge;
    private float actionDuration;

    [Header("Collision / Attack Settings")]
    public int collisionDamage = 1;

    private Vector2 startPosition;
    private Vector2 chargeDirection;

    [Header("Safety")]
    public float verticalIgnoreThreshold = 1.0f;

    public enum TestAttackMode { None, NormalAttack, Charge, BackAway, IdleMove }
    public TestAttackMode testMode = TestAttackMode.None;

    // =========================
    // MAIN UPDATE
    // =========================
    public void HandleState()
    {
        if (baseEnemy == null || baseEnemy.player == null)
            return;

        float verticalDiff = Mathf.Abs(baseEnemy.player.position.y - transform.position.y);

        // Only stop movement if too high, but allow attack
        if (verticalDiff > verticalIgnoreThreshold)
        {
            baseEnemy.SetVelocity(Vector2.zero);

            // Force attack if possible
            if (CanNormalAttack())
            {
                currentState = ActionState.Attacking;
                PerformAttack();
                lastAttackTime = Time.time;
                actionDuration = 0.5f;
            }

            return;
        }

        // Tick down action timer
        if (actionDuration > 0f)
        {
            actionDuration -= Time.deltaTime;
            if (actionDuration <= 0f && currentState != ActionState.Charging)
            {
                currentState = ActionState.Idle;
                backAwayInitialized = false;
            }
        }

        // STATE MACHINE
        switch (currentState)
        {
            case ActionState.Idle:
                HandleDecision();
                break;
            case ActionState.BackingUp:
                BackAway(); // ONLY BackAway sets velocity here
                break;
            case ActionState.Charging:
                ChargeForward();
                break;
            case ActionState.IdleMove:
                HandleDecision();

                // Optional: automatic test attack even if player out of range
                if (CanNormalAttack())
                {
                    currentState = ActionState.Attacking;
                    PerformAttack();
                    lastAttackTime = Time.time;
                    actionDuration = 0.5f;
                    Debug.Log("Idle attack performed from current position");
                }
                break;
            case ActionState.Attacking:
                baseEnemy.SetVelocity(Vector2.zero);
                break;
        }
    }

    // =========================
    // DECISION MAKING
    // =========================
    void HandleDecision()
    {
        if (baseEnemy == null || baseEnemy.player == null)
            return;

        // ----------- TEST MODE ----------- 
        if (testMode != TestAttackMode.None)
        {
            switch (testMode)
            {
                case TestAttackMode.NormalAttack:
                    currentState = ActionState.Attacking;
                    PerformAttack();
                    actionDuration = 0.5f;
                    return;
                case TestAttackMode.Charge:
                    StartCharge();
                    return;
                case TestAttackMode.BackAway:
                    currentState = ActionState.BackingUp;
                    backAwayInitialized = false;
                    actionDuration = 0.5f;
                    return;
                case TestAttackMode.IdleMove:
                    currentState = ActionState.IdleMove;
                    actionDuration = 0.5f;
                    return;
            }
        }

        // ----------- NORMAL RANDOM DECISION -----------
        if (Time.time < nextDecisionTime)
            return;

        nextDecisionTime = Time.time + decisionCooldown;
        baseEnemy.SetVelocity(Vector2.zero);

        var possibleActions = new System.Collections.Generic.List<int>();
        if (CanCharge()) possibleActions.Add(1);
        if (CanBackAway()) possibleActions.Add(2);
        if (CanNormalAttack()) possibleActions.Add(3);
        possibleActions.Add(4); // idle move

        int decision = possibleActions[Random.Range(0, possibleActions.Count)];

        switch (decision)
        {
            case 1:
                StartCharge();
                lastChargeTime = Time.time;
                break;
            case 2:
                currentState = ActionState.BackingUp;
                lastBackAwayTime = Time.time;
                actionDuration = 0.5f;
                backAwayInitialized = false;
                break;
            case 3:
                currentState = ActionState.Attacking;
                PerformAttack();
                lastAttackTime = Time.time;
                actionDuration = 0.5f;
                break;
            case 4:
                currentState = ActionState.IdleMove;
                actionDuration = 0.5f;
                break;
        }
    }

    bool CanCharge() => Time.time >= lastChargeTime + chargeCooldown;
    bool CanBackAway() => Time.time >= lastBackAwayTime + backAwayCooldown;
    bool CanNormalAttack() => Time.time >= lastAttackTime + normalAttackCooldown;

    // =========================
    // BACK AWAY
    // =========================
    void BackAway()
    {
        if (!backAwayInitialized)
        {
            backAwayStartPos = transform.position;
            backAwayDirection = transform.localScale.x > 0 ? -1 : 1; // backward relative to facing
            backAwayInitialized = true;
        }

        // Raycast for walls behind
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            new Vector2(backAwayDirection, 0),
            0.2f,
            LayerMask.GetMask("Ground", "Obstacle")
        );

        if (hit)
        {
            StopBackAway();
            return;
        }

        // Move backward
        baseEnemy.SetVelocity(new Vector2(backAwayDirection * backAwaySpeed, baseEnemy.rb.linearVelocity.y));

        // Stop after reaching target distance
        float distanceMoved = Mathf.Abs(transform.position.x - backAwayStartPos.x);
        if (distanceMoved >= backAwayDistance)
        {
            StopBackAway();
        }
    }

    void StopBackAway()
    {
        baseEnemy.SetVelocity(Vector2.zero);
        currentState = ActionState.Idle;
        backAwayInitialized = false;
    }

    // =========================
    // CHARGE
    // =========================
    void StartCharge()
    {
        hasAttackedDuringCharge = false;
        startPosition = transform.position;
        float dir = Mathf.Sign(baseEnemy.player.position.x - transform.position.x);
        chargeDirection = new Vector2(dir, 0f);
        currentState = ActionState.Charging;
    }

    void ChargeForward()
    {
        baseEnemy.SetVelocity(new Vector2(chargeDirection.x * chargeSpeed, baseEnemy.rb.linearVelocity.y));

        float distance = Vector2.Distance(startPosition, transform.position);
        float distToPlayer = Vector2.Distance(transform.position, baseEnemy.player.position);

        if ((distance >= maxChargeDistance || distToPlayer < 0.5f) && !hasAttackedDuringCharge)
        {
            PerformChargeAttack();
        }
    }

    void PerformChargeAttack()
    {
        var playerHealth = baseEnemy.player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            Vector2 knockDir = (baseEnemy.player.position - transform.position).normalized;
            playerHealth.TakeDamage(collisionDamage, knockDir);
        }

        hasAttackedDuringCharge = true;
        currentState = ActionState.BackingUp;
        backAwayInitialized = false;
        actionDuration = 0.5f;
        lastBackAwayTime = Time.time;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == ActionState.Charging)
        {
            bool hitPlayer = collision.gameObject.CompareTag("Player");
            bool hitWall = collision.gameObject.layer == LayerMask.NameToLayer("Ground") ||
                           collision.gameObject.layer == LayerMask.NameToLayer("Obstacle");

            if (hitPlayer)
            {
                var playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    Vector2 knockDir = (collision.transform.position - transform.position).normalized;
                    playerHealth.TakeDamage(collisionDamage, knockDir);
                }
            }

            if (hitPlayer || hitWall)
            {
                currentState = ActionState.BackingUp;
                lastBackAwayTime = Time.time;
                actionDuration = 0.5f;
                backAwayInitialized = false;
            }
        }
    }

    // =========================
    // BASIC ATTACK
    // =========================
    protected override void PerformAttack()
    {
        sadnessSFXManager.PlaySadnessAttackEmitter();

        var playerHealth = baseEnemy.player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            Vector2 knockDir = (baseEnemy.player.position - transform.position).normalized;
            playerHealth.TakeDamage(collisionDamage, knockDir);
            attackEvent.Invoke();
        }
    }

    // =========================
    // IDLE MOVE
    // =========================
    void IdleMove()
    {
        // Stay in place; no movement
        baseEnemy.SetVelocity(Vector2.zero);

        // Optionally, attempt attack
        if (CanNormalAttack())
        {
            currentState = ActionState.Attacking;
            PerformAttack();
            lastAttackTime = Time.time;
            actionDuration = 0.5f;
            Debug.Log("IdleMove attack performed from current position");
        }
    }

    public override void TryAttack()
    {
        // no-op
    }
}