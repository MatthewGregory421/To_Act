using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerCombat combat;

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask obsticleLayer;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Double Jump Settings")]
    public bool doubleJumpEnabled = true; // toggle in inspector
    private bool canDoubleJump;
    public bool canJump = true;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public Rigidbody2D rb;
    private float moveInput;
    private Vector2 attackInput;
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

    [Header("Combat State")]
    public bool isInvulnerable;

    [Header("Ground Slam State")]
    public bool isGroundSlamming = false; // currently slamming
    public float groundSlamSpeed = 25f;   // vertical fall speed during slam
    public bool canGroundSlam;

    [Header("Look Direction")]
    public int lookHorizontal;
    public int lookVertical;

    private SpriteRenderer spriteRenderer;

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<float>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnJump(context);
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        attackInput = context.ReadValue<Vector2>();
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnCrouchStart(context);
        }
        else if (context.canceled)
        {
            OnCrouchEnd(context);
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalColliderSize = capsule.size;
        originalColliderOffset = capsule.offset;

        // Add this line:
        combat = GetComponent<PlayerCombat>();


        if (combat == null)
            Debug.LogError("PlayerCombat component not found on this GameObject!");
    }

    private void Update()
    {
        HandleFacingDirection();
        HandleAttackDirection();

        CheckGrounded();

        Debug.Log(moveInput);
    }

    private void FixedUpdate()
    {
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
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                -groundSlamSpeed
            );
        }
    }

    void HandleFacingDirection()
    {
        if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    void HandleAttackDirection()
    {
        lookHorizontal = 0;
        lookVertical = 0;

        // Vertical attack priority
        if (attackInput.y > 0)
        {
            lookVertical = 1;
        }
        else if (attackInput.y < 0)
        {
            lookVertical = -1;
        }
        else if (attackInput.x > 0)
        {
            lookHorizontal = 1;
        }
        else if (attackInput.x < 0)
        {
            lookHorizontal = -1;
        }
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (isCrouching)
            return;

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            canDoubleJump = true;
        }
        else if (doubleJumpEnabled && canDoubleJump)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            canDoubleJump = false;
        }
    }

    void OnAttack(InputAction.CallbackContext context)
    {
        attackInput = context.ReadValue<Vector2>();
    }

    void OnCrouchStart(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            if (!isCrouching)
                EnterCrouch();

            isCrouching = true;
        }
    }

    void OnCrouchEnd(InputAction.CallbackContext context)
    {
        if (isCrouching && !IsCeilingAbove())
        {
            ExitCrouch();
        }
    }


    public void SetEnemyCollision(bool enabled)
    {
        int playerLayer = gameObject.layer;

        for (int i = 0; i < 32; i++)
        {
            if ((enemyLayer.value & (1 << i)) != 0)
            {
                Physics2D.IgnoreLayerCollision(
                    playerLayer,
                    i,
                    !enabled
                );
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
            canGroundSlam = true;

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
            originalColliderOffset.y -
            (originalColliderSize.y *
            (1 - crouchHeightMultiplier) / 2f)
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
}
