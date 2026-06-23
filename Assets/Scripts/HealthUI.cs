using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("Heart Icons")]
    [SerializeField] private Image[] hearts;

    [Header("Sprites")]
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < maxHealth)
            {
                hearts[i].gameObject.SetActive(true);
                hearts[i].sprite = i < currentHealth ? fullHeart : emptyHeart;
            }
            else
            {
                hearts[i].gameObject.SetActive(false);
            }
        }
    }
}
