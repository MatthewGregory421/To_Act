using UnityEngine;

public class DestructibleFloor : MonoBehaviour
{
    [Header("Health")]
    public int health = 1;

    public void TakeSlamHit(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Break();
        }
    }

    private void Break()
    {
        // Optional: VFX / sound here
        Destroy(gameObject);
    }
}
