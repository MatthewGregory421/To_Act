using UnityEngine;
using UnityEngine.Events;

public class EnemyAttack : MonoBehaviour
{
    public UnityEvent attackEvent;
    protected BaseEnemy baseEnemy;
    protected EnemyAI enemyAI;
    

    [Header("Attack Settings")]
    public float attackCooldown = 1f;

    public virtual float GetAttackRange()
    {
        return 1.5f;
    }

    protected float lastAttackTime;

    protected virtual void Awake()
    {
        baseEnemy = GetComponent<BaseEnemy>();
        if (baseEnemy == null)
            Debug.LogError("RandomBehaviorAttack: BaseEnemy component missing!");
    }

    public virtual bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    public virtual void TryAttack()
    {
        if (!CanAttack()) return;

        lastAttackTime = Time.time;
        PerformAttack();
        
    }

    protected virtual void PerformAttack()
    {
        // OVERRIDE THIS IN CHILD CLASSES
        Debug.Log("Enemy attacked (base)");
        attackEvent.Invoke();
    }
}