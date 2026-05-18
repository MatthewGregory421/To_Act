using UnityEngine;
using UnityEngine.SceneManagement;

public class RH_DeathManager : MonoBehaviour
{
    public GameObject deathCanvas;

    private void Start()
    {
        deathCanvas.SetActive(false);
    }

    public void PlayerDied()
    {
        Time.timeScale = 0;
        deathCanvas.SetActive(true);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void Respawn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
