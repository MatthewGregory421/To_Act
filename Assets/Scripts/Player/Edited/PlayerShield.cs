using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    public PlayerSFXManager sfxManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ONLY enemy projectiles should interact
        if (!collision.CompareTag("EnemyProjectile"))
            return;

        sfxManager.PlayShieldConnect();

        Destroy(collision.gameObject);
    }
}