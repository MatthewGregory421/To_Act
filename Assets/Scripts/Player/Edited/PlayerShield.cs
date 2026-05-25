using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    public PlayerSFXManager sfxManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // only care about enemy projectiles
        EnemyProjectile projectile = collision.GetComponent<EnemyProjectile>();

        if (projectile != null)
        {
            sfxManager.PlayShieldConnect();
            Destroy(collision.gameObject);
        }
    }
}