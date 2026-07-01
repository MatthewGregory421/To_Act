using UnityEngine;

public class EnemyAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public bool moving;
    public bool special;
    public Vector2 velocity;

    [SerializeField] private EnemyProjectileSpawner projectileSpawner;

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Moving", moving);
        animator.SetBool("Special", special);
        //animator.SetFloat("vVelocity", velocity.y);
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

    public void SpawnProjectile()
    {
        if (projectileSpawner != null)
            projectileSpawner.SpawnProjectile();
    }
}
