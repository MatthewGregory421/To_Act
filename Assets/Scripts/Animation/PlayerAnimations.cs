using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] Transform pSpawner;
    [SerializeField] private GameObject footstepeffect;
    [SerializeField] private GameObject landeffect;
    public bool grounded;
    private bool prevgrounded;
    public bool blocking;
    public bool crouch;
    public bool groundslam;
    public float velocity;
    bool doublejump = true;
    [SerializeField] Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SpawnEffect(GameObject effect)
    {
       GameObject spawnedEffect = Instantiate(effect, pSpawner);
        spawnedEffect.transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    
    {
        /*if (grounded && prevgrounded == false) {
        
            SpawnEffect(landeffect);
        
        } */
        animator.SetBool("Grounded", grounded);
        animator.SetBool("Blocking", blocking);
        animator.SetBool("Crouch", crouch);
        animator.SetBool("GroundSlam", groundslam);
        animator.SetFloat("Velocity", velocity);
        animator.SetFloat("Vvelocity", rb.linearVelocityY);

        if (Input.GetKeyDown(KeyCode.Space)){
            if (grounded)
            {
                SetTrigger("Jump");
            }
            else { 
            
            if(doublejump)
                {
                    SetTrigger("Jump");
                    doublejump = false;
                }
            }
        }
        if(grounded)
        {
             doublejump = true;
        }

        prevgrounded = grounded;
            
    }
    public void SetBool(string name, bool value)
    {
        animator.SetBool(name, value);
    }

    public void SetTrigger(string trigger)
    {
        animator.SetTrigger(trigger);
        //Damage, Attack, Jump, take damage
    }
}
