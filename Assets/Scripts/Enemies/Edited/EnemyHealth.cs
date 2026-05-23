using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("Death")]
    public bool destroyOnDeath = true;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log($"{gameObject.name} took {damage} damage. HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died");
        TestLevelCounter.Instance.AddKill();

        // Optional: play death animation / spawn effects here

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}