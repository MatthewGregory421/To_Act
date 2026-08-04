using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementInputSystem : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public BoxCollider2D box;

    public PlayerSFXManager playerSFXManager;
    public PlayerAnimations playerAnimations;

    public PlayerHealth playerHealth;

    [Header("Abilities")]
    public bool hasShield;
    public bool hasGroundSlam;

    [Header("Ability UI")]
    [SerializeField] private GameObject shieldIcon;
    [SerializeField] private GameObject groundSlamIcon;

    [Header("Movement")]
    public float moveSpeed = 5f;
    private float horizontalMovement;

    public int facingDirection = 1;

    [Header("Bench Sitting")]
    public bool IsSitting { get; private set; }

    private RigidbodyConstraints2D constraintsBeforeSitting;

    [Header("Movement Feel")]
    public float acceleration = 20f;
    public float deceleration = 25f;
    public float airControlMultiplier = 0.6f;

    [Header("Footsteps")]
    public float footstepInterval = 0.4f;

    private float footstepTimer;

    [Header("Jump")]
    public float jumpForce = 10f;

    [Header("Double Jump")]
    public bool doubleJumpEnabled = true;
    private bool canDoubleJump;

    [Header("Ground Check")]
    public Transform groundCheck; 
    public float groundCheckRadius = 0.2f; 
    public LayerMask groundLayer;

    public bool isGrounded;

    [Header("Crouch")]
    public bool isCrouching;
    public float crouchSpeedMultiplier = 0.6f;
    public float crouchHeightMultiplier = 0.5f;

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    [Header("Ceiling Check")]
    public Transform ceilingCheck;
    public float ceilingCheckRadius = 0.2f;
    public LayerMask ceilingLayer;

    [Header("Wall Check")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.6f;
    public LayerMask obstacleLayer;

    public bool isTouchingWall;

    [Header("Wall Slide")]
    public float wallSlideSpeed = 2f;

    [Header("Shield")]
    public GameObject shieldBubble;

    public float shieldDuration = 2f;
    public float shieldCooldown = 3f;

    private float shieldTimer;
    private float cooldownTimer;

    private bool isShieldActive;
    private bool isOnCooldown;
    private bool shieldSFXPlayed;

    [Header("Shield UI")]
    [SerializeField] private CoolDownIconUI shieldCooldownUI;

    [Header("Dazed State")]
    public bool isDazed;
    private float dazedTimer;

    [Header("Dazed Movement")]
    public float dazedSpeedMultiplier = 0.25f;

    private void Start()
    {
        originalColliderSize = box.size;
        originalColliderOffset = box.offset;

        if (shieldCooldownUI == null)
            shieldCooldownUI = GameObject.Find("ShieldFill")?.GetComponent<CoolDownIconUI>();

        UpdateAbilityUI();

        if (WorldStateManager.Instance != null)
        {
            WorldStateManager.Instance.ApplyCollectedAbilitiesToPlayer();
        }
    }


    void Update()
    {
        CheckGrounded();
        HandleLookDirection();
        CheckWall();

        HandleShieldTimers();
        HandleDazed();

        
        playerAnimations.crouch = isCrouching;
        playerAnimations.grounded = isGrounded;

        
        if (isDazed)
        {
            //playerAnimations.SetBool("Dazed", true);
        }
        else
         {
            //playerAnimations.SetBool("Dazed", false);
        }
        
    }

    private void FixedUpdate()
    {
        if (IsSitting)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float speed = moveSpeed;

        if (isCrouching)
            speed *= crouchSpeedMultiplier;

        if (isDazed)
            speed *= dazedSpeedMultiplier;

        float targetSpeed = horizontalMovement * speed;

        Vector2 velocity = rb.linearVelocity;

        float accel = acceleration;

        if (!isGrounded)
            accel *= airControlMultiplier;

        if (Mathf.Abs(horizontalMovement) < 0.01f)
            accel = deceleration;

        // horizontal movement smoothing
        velocity.x = Mathf.MoveTowards(
            velocity.x,
            targetSpeed,
            accel * Time.fixedDeltaTime
        );

        // WALL SLIDE LOGIC
        bool isFalling = velocity.y < 0;

        if (!isGrounded && isTouchingWall && isFalling)
        {
            velocity.y = Mathf.Max(velocity.y, -wallSlideSpeed);
        }

        rb.linearVelocity = velocity;

        //playerAnimations.velocity = Mathf.Abs(rb.linearVelocity.x);
    }
    public void UpdateAbilityUI()
    {
        if (shieldIcon != null)
            shieldIcon.SetActive(hasShield);

        if (groundSlamIcon != null)
            groundSlamIcon.SetActive(hasGroundSlam);
    }


    // =========================
    // INPUT
    // =========================

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 movementInput = context.ReadValue<Vector2>();

        if (IsSitting)
        {
            horizontalMovement = 0f;

            // Pressing left or right gets the player off the bench.
            if (Mathf.Abs(movementInput.x) > 0.01f)
            {
                ExitBench();
                horizontalMovement = movementInput.x;
            }

            return;
        }

        horizontalMovement = movementInput.x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (IsSitting)
            return;

        if (!context.performed)
            return;

        if (isCrouching)
            return;

        if (isDazed)
            return;

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            playerSFXManager.PlayPlayerJump();

            playerAnimations.Jump();

            canDoubleJump = true;
        }
        else if (doubleJumpEnabled && canDoubleJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            playerSFXManager.PlayPlayerJump();

            playerAnimations.Jump();

            canDoubleJump = false;
        }
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        if (IsSitting)
            return;

        if (isDazed)
            return;

        if (context.performed && isGrounded)
        {
            EnterCrouch();
        }

        if (context.canceled)
        {
            if (!IsCeilingAbove())
            {
                TryExitCrouch();
            }
        }
    }

    public void Shield(InputAction.CallbackContext context)
    {
        if (IsSitting)
            return;

        if (isDazed)
            return;

        if (context.performed)
        {
            TryActivateShield();
        }
    }

    // =========================
    // CROUCH
    // =========================

    private void EnterCrouch()
    {
        isCrouching = true;

        box.size = new Vector2(originalColliderSize.x, originalColliderSize.y * crouchHeightMultiplier);

        box.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - (originalColliderSize.y * (1 - crouchHeightMultiplier) / 2f));

    }

    private void TryExitCrouch()
    {
        if (IsCeilingAbove())
            return;

        ExitCrouch();
    }

    private void ExitCrouch()
    {
        isCrouching = false;

        box.size = originalColliderSize;
        box.offset = originalColliderOffset;
    }

    // =========================
    // SHIELD
    // =========================

    public void ApplyAbilitiesFromCollectedPickups(List<string> pickups)
    {
        if (pickups == null)
            return;

        hasShield = pickups.Contains("ShieldPickup");
        hasGroundSlam = pickups.Contains("GroundSlamPickup");

        UpdateAbilityUI();

        Debug.Log(
            $"Abilities applied. Shield: {hasShield}, Ground Slam: {hasGroundSlam}"
        );
    }

    private void TryActivateShield()
    {
        if (!hasShield)
            
            return;

        if (isOnCooldown || isShieldActive)
            return;

        ActivateShield();
    }

    private void ActivateShield()
    {
        isShieldActive = true;
        playerHealth.isInvincible = true;
        shieldTimer = shieldDuration;

        if (shieldBubble != null)
            shieldBubble.SetActive(true);

        // ANIMATION
        playerAnimations.blocking = true;

        if (!shieldSFXPlayed)
        {
            playerSFXManager.PlayShieldActive();
            shieldSFXPlayed = true;
        }
    }

    private void DeactivateShield()
    {
        isShieldActive = false;
        playerHealth.isInvincible = false;

        if (shieldBubble != null)
            shieldBubble.SetActive(false);

        // ANIMATION
        playerAnimations.blocking = false;

        // SFX (ONCE)
        playerSFXManager.PlayShieldDeactive();
        shieldSFXPlayed = false;
    }

    private void HandleShieldTimers()
    {
        // ACTIVE SHIELD
        if (isShieldActive)
        {
            shieldTimer -= Time.deltaTime;

            if (shieldTimer <= 0f)
            {
                DeactivateShield();
                StartCooldown();
            }
        }

        // COOLDOWN
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;

            if (shieldCooldownUI != null)
            {
                shieldCooldownUI.SetCooldownProgress(cooldownTimer, shieldCooldown);
            }

            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;

                if (shieldCooldownUI != null)
                {
                    shieldCooldownUI.SetReady();
                }
            }
        }
    }

    private void StartCooldown()
    {
        isOnCooldown = true;
        cooldownTimer = shieldCooldown;

        if (shieldCooldownUI != null)
        {
            shieldCooldownUI.SetCooldownProgress(cooldownTimer, shieldCooldown);
        }
    }

    // =========================
    // DAZED SYSTEM LOGIC
    // =========================

    private void HandleDazed()
    {
        if (!isDazed)
            return;

        dazedTimer -= Time.deltaTime;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.5f, rb.linearVelocity.y);

        if (dazedTimer <= 0f)
        {
            isDazed = false;
        }
    }

    public void Stun(float duration)
    {
        isDazed = true;
        dazedTimer = duration;
    }

    // =========================
    // LOOK DIRECTION
    // =========================

    private void HandleLookDirection()
    {
        if (horizontalMovement > 0.01f)
        {
            facingDirection = 1;
        }
        else if (horizontalMovement < -0.01f)
        {
            facingDirection = -1;
        }

        transform.localScale = new Vector3(
            facingDirection,
            1f,
            1f
        );
    }

    // =========================
    // CHECKS
    // =========================

    private bool IsCeilingAbove()
    {
        return Physics2D.OverlapCircle(ceilingCheck.position, ceilingCheckRadius, ceilingLayer);
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        if (isGrounded)
        {
            canDoubleJump = true;
        }

        if (!isGrounded && isCrouching)
        {
            TryExitCrouch();
        }
    }

    private void CheckWall()
    {
        Vector2 dir = new Vector2(facingDirection, 0);

        isTouchingWall = Physics2D.Raycast(
            wallCheck.position,
            dir,
            wallCheckDistance,
            obstacleLayer
        );
    }

    public void SitAtBench(Transform sitPoint)
    {
        if (IsSitting)
            return;

        IsSitting = true;
        horizontalMovement = 0f;

        if (isCrouching)
        {
            ExitCrouch();
        }

        rb.linearVelocity = Vector2.zero;

        // Remember the player's normal constraints.
        constraintsBeforeSitting = rb.constraints;

        if (sitPoint != null)
        {
            rb.position = sitPoint.position;
        }

        // Prevent gravity or movement from pulling the player off the bench.
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        // Add this when your sitting animation is implemented:
        // playerAnimations.SetSitting(true);
    }

    private void ExitBench()
    {
        if (!IsSitting)
            return;

        IsSitting = false;

        // Restore the constraints the player had before sitting.
        rb.constraints = constraintsBeforeSitting;

        // Add this when your sitting animation is implemented:
        // playerAnimations.SetSitting(false);
    }
}
