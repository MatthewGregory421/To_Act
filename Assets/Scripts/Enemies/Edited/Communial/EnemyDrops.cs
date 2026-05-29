using UnityEngine;

public class EnemyDrops : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;

    [Header("Drops")]
    public GameObject miniCreaturePrefab;
    public GameObject healthPickupPrefab;

    [Range(1, 10)]
    public int minHealthDrops = 1;
    public int maxHealthDrops = 3;

    public float scatterForce = 2f;

    public bool destroyOnDeath = true;

    public bool isDead;

    // Call this from your EnemyBase.Die()
    public void HandleDeath()
    {
        if (isDead) return;

        isDead = true;

        SpawnMiniCreature();
        SpawnHealthDrops();

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (destroyOnDeath)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }

    void SpawnMiniCreature()
    {
        if (miniCreaturePrefab == null) return;

        Instantiate(miniCreaturePrefab, transform.position, Quaternion.identity);
    }

    void SpawnHealthDrops()
    {
        if (healthPickupPrefab == null) return;

        int amount = Random.Range(minHealthDrops, maxHealthDrops + 1);

        for (int i = 0; i < amount; i++)
        {
            GameObject drop = Instantiate(
                healthPickupPrefab,
                transform.position,
                Quaternion.identity
            );

            Rigidbody2D rbDrop = drop.GetComponent<Rigidbody2D>();

            if (rbDrop != null)
            {
                Vector2 force = new Vector2(
                    Random.Range(-1f, 1f),
                    Random.Range(1f, 2f)
                ).normalized * scatterForce;

                rbDrop.AddForce(force, ForceMode2D.Impulse);
            }
        }
    }
}
