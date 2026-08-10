using UnityEngine;
using System.Collections;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Animator leganimator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerSFXManager playerSFXManager;
    [SerializeField] private PlayerMovementInputSystem playerMovement;

    float velX;
    float velY;

    // =========================
    // FOOTSTEPS
    // =========================

    [Header("Footsteps")]
    [SerializeField] private float footstepCooldown = 0.30f;

    private float lastFootstepTime = -999f;


    // =========================
    // EFFECTS
    // =========================

    [Header("Effects")]
    [SerializeField] private Transform pSpawner;
    [SerializeField] private GameObject footstepeffect;
    [SerializeField] private GameObject landeffect;
    [SerializeField] private GameObject slameffect;
    [SerializeField] private GameObject doublejumpeffect;


    // =========================
    // ANIMATION STATE
    // =========================

    [Header("Animation State")]
    public bool grounded;
    public bool blocking;
    public bool crouch;
    public bool groundslam;

    bool landeffectprimed;
    bool animgrounded;

    private bool prevGrounded;
    bool prevslamming;

    private bool takingDamage;

    bool fullbody = false;


    // =========================
    // UNITY
    // =========================

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (rb == null)
            rb = GetComponentInParent<Rigidbody2D>();

        if (playerMovement == null)
            playerMovement =
                GetComponentInParent<PlayerMovementInputSystem>();
    }

    private void Update()
    {
        if (animator == null || rb == null)
            return;


        // =========================
        // SLAM EFFECT
        // =========================

        if (!groundslam && prevslamming != groundslam)
        {
            SpawnEffect(slameffect);
        }


        // =========================
        // LAND EFFECT
        // =========================

        if (prevGrounded != grounded)
        {
            landeffectprimed = true;
        }

        if (grounded && Mathf.Abs(velY) <= 0f || Mathf.Abs(velY) >= 0f)
        {
            animgrounded = true;
        }

        if (landeffectprimed && animgrounded)
        {
            SpawnEffect(landeffect);
            landeffectprimed = false;
        }


        // =========================
        // INPUT / VELOCITY
        // =========================

        velX = rb.linearVelocityX;
        velY = rb.linearVelocityY;


        // =========================
        // ANIMATOR VELOCITIES
        // =========================

        animator.SetFloat("XVel", Mathf.Abs(velX));
        animator.SetFloat("YVel", velY);

        leganimator.SetFloat("VelX", Mathf.Abs(velX));
        leganimator.SetFloat("VelY", velY);


        // =========================
        // ANIMATOR BOOLS
        // =========================

        animator.SetBool("Grounded", grounded);
        leganimator.SetBool("Grounded", grounded);

        animator.SetBool("Blocking", blocking);
        animator.SetBool("Crouched", crouch);
        animator.SetBool("Groundslam", groundslam);


        // =========================
        // FULL BODY ANIMATIONS
        // =========================

        if (
            animator.GetCurrentAnimatorStateInfo(0).IsName("Groundslam") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Dazed") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("TakeDamage") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Sitting") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Crouch movement") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Pickup") ||
            blocking
        )
        {
            fullbody = true;
        }
        else
        {
            fullbody = false;
        }


        if (takingDamage)
            return;


        // =========================
        // LEG ANIMATOR
        // =========================

        if (fullbody)
        {
            leganimator.gameObject.SetActive(false);
        }
        else
        {
            leganimator.gameObject.SetActive(true);
        }


        prevslamming = groundslam;
        prevGrounded = grounded;
    }


    // =========================
    // ANIMATION FUNCTIONS
    // =========================

    public void UpdateSittingAnim(bool yea)
    {
        animator.SetBool("Sitting", yea);
    }

    public void ResetFullbody()
    {
        fullbody = false;
    }

    public void Jump()
    {
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }

    public void TakeDamage()
    {
        animator.SetTrigger("TakeDamage");

        AnimatorStateInfo state =
            animator.GetCurrentAnimatorStateInfo(0);

        Debug.Log("Current State: " + state.shortNameHash);
        Debug.Log(
            "Is in Take damage: " +
            state.IsName("Take damage")
        );
    }


    // =========================
    // FOOTSTEP SOUND
    // =========================

    public void PlayFootstep()
    {
        // Don't make footsteps while in the air.
        if (!grounded)
            return;

        // Don't make footsteps while standing still.
        if (Mathf.Abs(velX) < 0.1f)
            return;

        // Stop the animation event from spamming footsteps.
        if (Time.time - lastFootstepTime < footstepCooldown)
            return;

        // Record when this footstep played.
        lastFootstepTime = Time.time;

        // Play the actual FMOD footstep.
        playerSFXManager?.PlayPlayerFootsteps();

        // Optional visual footstep effect.
        // SpawnEffect(footstepeffect);
    }


    // =========================
    // DOUBLE JUMP EFFECT
    // =========================

    public void PlayDoubleJumpEffect()
    {
        SpawnEffect(doublejumpeffect);
    }


    // =========================
    // EFFECT SPAWNER
    // =========================

    public void SpawnEffect(GameObject effect)
    {
        if (effect == null || pSpawner == null)
            return;

        GameObject spawnedEffect =
            Instantiate(
                effect,
                pSpawner.position,
                Quaternion.identity
            );
    }

    // =========================
    // PICKUP ANIMATION
    // =========================

    public void PlayPickupAnimation()
    {
        if (animator == null)
            return;

        animator.ResetTrigger("Pickup");
        animator.SetTrigger("Pickup");
    }

    public void FinishPickupAnimation()
    {
        if (playerMovement != null)
        {
            playerMovement.FinishPickupAnimation();
        }
    }
}