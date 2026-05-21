using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private bool grounded;
    private bool blocking;
    private bool crouch;
    private bool groundslam;
    private float velocity;

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
