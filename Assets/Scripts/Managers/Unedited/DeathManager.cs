using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathManager : MonoBehaviour
{
    public MusicManager musicManager;
    public UISFXManager uiSFXManager;

    public GameObject deathCanvas;

    private void Start()
    {
        deathCanvas.SetActive(false);
    }

    public void PlayerDied()
    {
        Time.timeScale = 0;
        deathCanvas.SetActive(true);
        musicManager.MusDeadFO();
        musicManager.SilenceMusEmitter();
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        uiSFXManager.PlayConfirm();
        SceneManager.LoadScene("MainMenu");
    }

    public void Respawn()
    {
        Time.timeScale = 1;
        uiSFXManager.PlayConfirm();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
