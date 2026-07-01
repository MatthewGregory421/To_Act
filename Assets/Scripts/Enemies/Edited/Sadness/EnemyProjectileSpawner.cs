using UnityEngine;

public class EnemyProjectileSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public Transform aimTarget;

    public System.Action onShoot;

    public bool canShoot;

    [Header("Settings")]
    private float fireRate = 3f;

    private float timer;

    private void Update()
    {
        if (!canShoot)
            return;

        TryFindAimTarget();

        if (aimTarget == null || projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning($"{name} cannot shoot. AimTarget: {aimTarget}, Projectile: {projectilePrefab}, FirePoint: {firePoint}");
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            onShoot?.Invoke(); // plays animation/sfx
            timer = fireRate;
        }
    }

    public void SpawnProjectile()
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Vector2 direction = (aimTarget.position - firePoint.position).normalized;

        proj.GetComponent<EnemyProjectile>()?.SetDirection(direction);
    }

    private void TryFindAimTarget()
    {
        if (aimTarget != null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Transform found = player.transform.Find("AimTarget");

        aimTarget = found != null ? found : player.transform;
    }
}
