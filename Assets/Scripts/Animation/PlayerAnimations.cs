using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerSFXManager playerSFXManager;

    [Header("Effects")]
    [SerializeField] private Transform pSpawner;
    [SerializeField] private GameObject footstepeffect;
    [SerializeField] private GameObject landeffect;

    [Header("Animation State")]
    public bool grounded;
    public bool blocking;
    public bool crouch;
    public bool groundslam;
    public float velocity;

    private bool prevGrounded;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (animator == null || rb == null)
            return;

        animator.SetBool("Grounded", grounded);
        animator.SetBool("Blocking", blocking);
        animator.SetBool("Crouch", crouch);
        animator.SetBool("GroundSlam", groundslam);

        animator.SetFloat("Velocity", velocity);
        animator.SetFloat("Vvelocity", rb.linearVelocity.y);

        prevGrounded = grounded;
    }

    public void SpawnEffect(GameObject effect)
    {
        if (effect == null || pSpawner == null)
            return;

        GameObject spawnedEffect = Instantiate(effect, pSpawner.position, Quaternion.identity);
    }

    public void SetBool(string name, bool value)
    {
        animator.SetBool(name, value);
    }

    public void SetTrigger(string trigger)
    {
        animator.SetTrigger(trigger);
    }

    public void Jump()
    {
        SetTrigger("Jump");
    }

    public void Attack()
    {
        SetTrigger("Attack");
    }

    public void TakeDamage()
    {
        SetTrigger("TakeDamage");
    }

    public void PlayFootstep()
    {
        if (!grounded)
            return;

        if (velocity < 0.1f)
            return;

        playerSFXManager?.PlayPlayerFootsteps();
    }
}