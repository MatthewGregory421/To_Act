using UnityEngine;

public class EnemyAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public bool moving;
    public bool special;
    public Vector2 velocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Moving", moving);
        animator.SetBool("Special", special);
        animator.SetFloat("vVelocity", velocity.y);
    }

    public void EnemyAttack()
    {
        animator.SetTrigger("Attack");
    }
    public void EnemyTakeDamage()
    {
        animator.SetTrigger("TakeDamage");
    }

    public void SpecialConnect()
    {
        animator.SetTrigger("SpecialConnect");
    }
}
