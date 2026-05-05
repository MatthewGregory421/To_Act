using UnityEngine;
using FMODUnity;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    private PlayerCombat combat;
    public PlayerSFXManager sfxManager;

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask obsticleLayer;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Footsteps")]
    public StudioEventEmitter footstepEmitter;

    public float minStepInterval = 0.5f;
    public float maxStepInterval = 0.25f;

    private float stepTimer;

    [Header("Double Jump Settings")]
    public bool doubleJumpEnabled = true; // toggle in inspector
    private bool canDoubleJump;
    public bool canJump = true;
    public UnityEvent jumpEvent;
    public UnityEvent doubleJumpEvent;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public Rigidbody2D rb;
    private float moveInput;
    public bool isGrounded;

    [Header("Crouch Settings")]
    public bool isCrouching;
    public float crouchSpeedMultiplyer = 0.6f;

    [Header("Crouch Collider Settings")]
    public CapsuleCollider2D capsule;

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    public float crouchHeightMultiplier = 0.5f;

    public LayerMask ceilingLayer;
    public Transform ceilingCheck;
    public float ceilingCheckRadius = 0.2f;

    [Header("Dash Settings")]
    public bool dashEnabled = true;
    public float dashForce = 18f;
    [Tooltip("How long the player remains in the dash state.")]
    public float dashDuration;
    public float dashCooldown = 0.8f;
    public UnityEvent dashEvent;

    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    private bool hasAirDashed;
    private float dashDirection;

    [Header("Combat State")]
    public bool isInvulnerable;

    [Header("Ground Slam State")]
    public bool isGroundSlamming = false; // currently slamming
    public float groundSlamSpeed = 25f;   // vertical fall speed during slam
    public bool canGroundSlam;

    [Header("Look Direction")]
    public int lookHorizontal;
    public int lookVertical;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();

        originalColliderSize = capsule.size;
        originalColliderOffset = capsule.offset;

        // Add this line:
        combat = GetComponent<PlayerCombat>();


        if (combat == null)
            Debug.LogError("PlayerCombat component not found on this GameObject!");
    }

    private void Update()
    {
        HandleMovementInput();
        HandleJump();
        HandleLookDirection();
        CheckGrounded();
        HandleDash();
        HandleFootsteps();
    }

    private void FixedUpdate()
    {
        if (isDashing)
            return;

        float speed = moveSpeed;
        if (isCrouching)
            speed *= crouchSpeedMultiplyer;

        // Normal left/right movement
        Vector2 velocity = rb.linearVelocity;

        // Check for wall in movement direction
        RaycastHit2D wallHit = Physics2D.Raycast(
            transform.position,
            new Vector2(moveInput, 0),
            0.6f,
            obsticleLayer
        );

        if (wallHit && moveInput != 0)
        {
            velocity.x = 0; // stop sticking
        }
        else
        {
            velocity.x = moveInput * speed;
        }

        rb.linearVelocity = velocity;
        

        // Apply ground slam downward force
        if (isGroundSlamming)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -groundSlamSpeed); // pull straight down
        }
    }

    void HandleMovementInput()
    {
        moveInput = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveInput = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            moveInput = 1f;
        }
    }

    void HandleFootsteps()
    {
        float currentSpeed = Mathf.Abs(rb.linearVelocity.x);
        float speedPercent = Mathf.Clamp01(currentSpeed / moveSpeed);
        float currentStepInterval = Mathf.Lerp(minStepInterval, maxStepInterval, speedPercent);

        if (currentSpeed > 0.1f && isGrounded && !isDashing)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                footstepEmitter.Play();
                stepTimer = currentStepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void HandleLookDirection()
    {
        if (isDashing)
            return;


        lookHorizontal = 0;
        lookVertical = 0;


        bool left = Input.GetKey(KeyCode.LeftArrow);
        bool right = Input.GetKey(KeyCode.RightArrow);

        if (left && !right)
            lookHorizontal = -1;
        else if (right && !left)
            lookHorizontal = 1;


        bool up = Input.GetKey(KeyCode.UpArrow);
        bool down = Input.GetKey(KeyCode.DownArrow);

        if (up && !down)
            lookVertical = 1;
        else if (down && !up)
            lookVertical = -1;


        bool crouchPressed = Input.GetKey(KeyCode.C);

        if (crouchPressed && isGrounded)
        {
            if (!isCrouching)
                EnterCrouch();

            isCrouching = true;
        }
        else
        {
            if (isCrouching && !IsCeilingAbove())
                ExitCrouch();
        }
    }

    void HandleJump()
    {
        if (isCrouching)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                canDoubleJump = true;
                sfxManager.PlayPlayerJumpEmitter();
                jumpEvent.Invoke();
            }
            else if (doubleJumpEnabled && canDoubleJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                canDoubleJump = false;
                sfxManager.PlayPlayerJumpEmitter();
                doubleJumpEvent.Invoke();
            }
        }
    }

    public void SetEnemyCollision(bool enabled)
    {
        int playerLayer = gameObject.layer;

        for (int i = 0; i < 32; i++)
        {
            if ((enemyLayer.value & (1 << i)) != 0)
            {
                Physics2D.IgnoreLayerCollision(playerLayer, i, !enabled);
            }
        }
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (!isGrounded && isCrouching)
        {
            ExitCrouch();
        }

        if (isGrounded)
        {
            canDoubleJump = true;
            hasAirDashed = false;
            canGroundSlam = true;

            // If player was ground slamming, spawn indicators
            if (isGroundSlamming)
            {
                combat.SpawnGroundSlamIndicators();
                isGroundSlamming = false;

                SetEnemyCollision(true);
            }
        }
    }

    void EnterCrouch()
    {
        capsule.size = new Vector2(
            originalColliderSize.x,
            originalColliderSize.y * crouchHeightMultiplier
        );

        capsule.offset = new Vector2(
            originalColliderOffset.x,
            originalColliderOffset.y - (originalColliderSize.y * (1 - crouchHeightMultiplier) / 2f)
        );
    }

    void ExitCrouch()
    {
        capsule.size = originalColliderSize;
        capsule.offset = originalColliderOffset;
        isCrouching = false;
    }

    bool IsCeilingAbove()
    {
        return Physics2D.OverlapCircle(
            ceilingCheck.position,
            ceilingCheckRadius,
            ceilingLayer
        );
    }

    void HandleDash()
    {
        if (!dashEnabled)
            return;

        if (dashCooldown > 0)
            dashCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.A) && dashCooldownTimer <= 0 && !isDashing && !isCrouching)
        {
            if (lookHorizontal == 0)
                return;

            if (isGrounded)
            {
                StartDash();
            }
            else if (!hasAirDashed)
            {
                hasAirDashed = true;
                StartDash();
            }
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;

            rb.linearVelocity = new Vector2(dashDirection * dashForce, 0f);

            if (dashTimer <= 0)
                EndDash();

        }
    }

    void StartDash()
    {
        isDashing = true;
        isInvulnerable = true;

        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        dashDirection = lookHorizontal;

        rb.gravityScale = 0f; // optional freeze gravity
        dashEvent.Invoke();
        sfxManager.PlayPlayerDashEmitter();
    }

    void EndDash()
    {
        isDashing = false;
        isInvulnerable = false;

        rb.gravityScale = 2f; // set back to normal gravity
    }

    public bool CheckDashCoolDown()
    {
        if(dashCooldownTimer <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
