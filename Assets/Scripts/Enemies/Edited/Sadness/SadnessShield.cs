using UnityEngine;

public class SadnessShield : MonoBehaviour
{
    [Header("References")]
    public EnemyBase enemyBase;
    public EnemyAnimations animations;

    private EnemySFXManager SFX => EnemySFXManager.Instance;

    [Header("Shield State")]
    public bool isActive = true;

    private void Awake()
    {
        isActive = true;
    }

    private void Start()
    {
        enemyBase.isInvincible = true;

        if (animations != null)
            animations.special = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return;

        PlayerProjectile projectile = collision.GetComponent<PlayerProjectile>();

        if (projectile != null)
        {
            Destroy(collision.gameObject);

            if (animations != null)
                animations.SpecialConnect();

            SFX?.PlaySadnessSpecialConnect();
        }
    }

    public void BreakShield()
    {
        if (!isActive)
            return;

        isActive = false;

        enemyBase.isInvincible = false;

        if (animations != null)
            animations.special = false;

        SFX?.PlaySadnessSpecialDeactive();

        gameObject.SetActive(false);
    }
}