using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public bool grounded;
    public bool blocking;
    public bool crouch;
    public bool groundslam;
    public float velocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Grounded", grounded);
        animator.SetBool("Blocking", blocking);
        animator.SetBool("Crouch", crouch);
        animator.SetBool("GroundSlam", groundslam);
        animator.SetFloat("Velocity", velocity);
    }

    public void SetTrigger(string trigger)
    {
        animator.SetTrigger(trigger);
        //Damage, Attack, Jump
    }
}
