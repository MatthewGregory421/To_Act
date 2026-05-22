using UnityEngine;

public class EnemyAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;
    bool moving;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Moving", moving);
    }

    public void EnemyAttack()
    {
        animator.SetTrigger("Attack");
    }
    public void EnemyTakeDamage()
    {
        animator.SetTrigger("TakeDamage");
    }
}
