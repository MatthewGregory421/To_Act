using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementInputSystem : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public CapsuleCollider2D capsule;

    [Header("Movement")]
    public float moveSpeed = 5f;
    private float horizontalMovement;

    [Header("Jump")]
    public float jumpForce = 10f;

    [Header("Double Jump")]
    public bool doubleJumpEnabled = true;
    private bool canDoubleJump;

    [Header("Ground Check")]
    public Transform groundCheck; 
    public float groundCheckRadius = 0.2f; 
    public LayerMask groundLayer;

    private bool isGrounded;

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

    private void Start()
    {
        originalColliderSize = capsule.size;
        originalColliderOffset = capsule.offset;
    }


    void Update()
    {
        CheckGrounded();
    }

    private void FixedUpdate()
    {
        float speed = moveSpeed;

        if (isCrouching)
        {
            speed *= crouchSpeedMultiplier;
        }

        rb.linearVelocity = new Vector2(horizontalMovement * speed, rb.linearVelocity.y);
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (isCrouching)
            return;

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            canDoubleJump = true;
        }
        else if (doubleJumpEnabled && canDoubleJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            canDoubleJump = false;
        }
    }

    public void Crouch(InputAction.CallbackContext context)
    {
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

    private void EnterCrouch()
    {
        isCrouching = true;

        capsule.size = new Vector2(originalColliderSize.x, originalColliderSize.y * crouchHeightMultiplier);

        capsule.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - (originalColliderSize.y * (1 - crouchHeightMultiplier) / 2f));

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

        capsule.size = originalColliderSize;
        capsule.offset = originalColliderOffset;
    }

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
}
