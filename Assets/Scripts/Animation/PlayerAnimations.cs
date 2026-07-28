using UnityEngine;
using System.Collections;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Animator leganimator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerSFXManager playerSFXManager;

    float velX;
    float velY;
    float aimX;
    float aimY;
    [Header("Effects")]
    [SerializeField] private Transform pSpawner;
    [SerializeField] private GameObject footstepeffect;
    [SerializeField] private GameObject landeffect;

    [Header("Animation State")]
    public bool grounded;
    public bool blocking;
    public bool crouch;
    public bool groundslam;
    //public float velocity;

    private bool prevGrounded;

    private bool takingDamage;

    bool fullbody = false;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (rb == null)
            rb = GetComponentInParent<Rigidbody2D>();
    }

    private void Update()
    {
        handlearrows();
        velX = rb.linearVelocityX;
        velY = rb.linearVelocityY;

        //Velocities
        animator.SetFloat("XVel", Mathf.Abs( velX));
        animator.SetFloat("YVel", velY);
        leganimator.SetFloat("VelX",Mathf.Abs( velX));
        leganimator.SetFloat("VelY", velY);
        animator.SetFloat("AimX", aimX);
        animator.SetFloat("AimY", aimY);

        //Bools
        animator.SetBool("Grounded", grounded);
        leganimator.SetBool("Grounded", grounded);
        animator.SetBool("Blocking", blocking);
        animator.SetBool("Crouched", crouch);
        animator.SetBool("Groundslam", groundslam);

        

        if (groundslam || blocking)
        {
            fullbody = true;
        }

        if (animator == null || rb == null)
            return;

        if (takingDamage)
            return;

        if (fullbody)
        {
            leganimator.gameObject.SetActive(false);
        }
        else
        {
            leganimator.gameObject.SetActive(true);
        }


        //Old
        /*
        if (animator == null || rb == null)
            return;

        if (takingDamage)
            return;

        
        animator.SetBool("Grounded", grounded);
        animator.SetBool("Blocking", blocking);
        animator.SetBool("Crouch", crouch);
        animator.SetBool("GroundSlam", groundslam);

        animator.SetFloat("Velocity", velocity);
        animator.SetFloat("Vvelocity", rb.linearVelocity.y);

        prevGrounded = grounded;
        */
    }

    void handlearrows()
    {
        //It has to be done...
        //Velocity doesn't change with orientation
        //AimX; 1 = forward, -1 = backward
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //If velocity right, and press left
           if(velX > 0)
            {
                aimX = -1;
            }
            else
            {
                aimX = 1;
            }
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            //If velocity right and press right
            if(velX > 0)
            {
                aimX = 1;
            }
            else
            {
                aimX = -1;
            }
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            aimY = 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            aimY = -1;
        }
    }

    public void ResetFullbody()
    {
        fullbody = false;
    }

    public void Jump()
    {

    }

    public void TakeDamage()
    {
        animator.SetTrigger("TakeDamage");
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }

    //Old
    /*
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
        animator.Play("Take damage", 0, 0f);

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        Debug.Log("Current State: " + state.shortNameHash);
        Debug.Log("Is in Take damage: " + state.IsName("Take damage"));
    }

    public void PlayFootstep()
    {
        if (!grounded)
            return;

        if (velocity < 0.1f)
            return;

        playerSFXManager?.PlayPlayerFootsteps();
    }
    */
}