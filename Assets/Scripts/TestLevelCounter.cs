using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestLevelCounter : MonoBehaviour
{
    public static TestLevelCounter Instance;

    public int totalEnemies = 9;
    private int defeatedEnemies = 0;

    public TextMeshProUGUI counterText;
    public GameObject winText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        winText.SetActive(false);
        UpdateText();
    }

    public void AddKill()
    {
        defeatedEnemies++;

        UpdateText();

        if (defeatedEnemies >= totalEnemies)
        {
            winText.SetActive(true);
        }
    }

    void UpdateText()
    {
        counterText.text = defeatedEnemies + " / " + totalEnemies + " enemies defeated";
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}