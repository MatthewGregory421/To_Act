using UnityEngine;

public class SadnessShield : MonoBehaviour
{
    [Header("References")]
    public EnemyBase enemyBase;

    [Header("Shield State")]
    public bool isActive = true;

    private void Awake()
    {
        isActive = true;
    }

    private void Start()
    {
        enemyBase.isInvincible = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return;

        // ONLY block player projectiles
        PlayerProjectile projectile = collision.GetComponent<PlayerProjectile>();

        if (projectile != null)
        {
            Destroy(collision.gameObject);
        }
    }
    public void BreakShield()
    {
        if (!isActive)
            return;

        isActive = false;

        enemyBase.isInvincible = false;

        gameObject.SetActive(false);
    }

}