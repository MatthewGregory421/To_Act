using UnityEngine;

public class EnemyProjectileSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public Transform aimTarget;

    [Header("Settings")]
    public float fireRate = 1.5f;

    private float timer;

    private void Update()
    {
        TryFindAimTarget();

        if (aimTarget == null || projectilePrefab == null) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Shoot();
            timer = fireRate;
        }
    }

    private void TryFindAimTarget()
    {
        if (aimTarget != null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Transform found = player.transform.Find("AimTarget");

        if (found != null)
            aimTarget = found;
    }


    private void Shoot()
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Vector2 direction = (aimTarget.position - firePoint.position).normalized;

        proj.GetComponent<EnemyProjectile>()?.SetDirection(direction);
    }
}
