using UnityEngine;

public class SpawnEnemyTrigger : MonoBehaviour
{
    [Header("Object To Spawn")]
    public GameObject objectPrefab;

    [Header("Spawn Points")]
    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public Transform spawnPoint3;

    [Header("Trigger Settings")]
    public bool destroyTriggerAfterUse = true;

    private bool hasSpawned;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only trigger once
        if (hasSpawned)
            return;

        // Optional: only player can trigger
        if (!collision.CompareTag("Player"))
            return;

        hasSpawned = true;

        // Spawn objects
        Instantiate(objectPrefab, spawnPoint1.position, Quaternion.identity);
        Instantiate(objectPrefab, spawnPoint2.position, Quaternion.identity);
        Instantiate(objectPrefab, spawnPoint3.position, Quaternion.identity);

        // Optional cleanup
        if (destroyTriggerAfterUse)
        {
            Destroy(gameObject);
        }
    }
}
