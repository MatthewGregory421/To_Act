using UnityEngine;

public class EndOfDemoTrigger : MonoBehaviour
{
    [Header("UI")]
    public GameObject endScreen;

    private bool triggered = false;
    private bool returningToMenu = false;

    private void Start()
    {
        if (endScreen != null)
            endScreen.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        Time.timeScale = 0f;

        if (endScreen != null)
            endScreen.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        if (returningToMenu) return;

        returningToMenu = true;

        UISFXManager.Instance?.PlayUIConfirm();

        Time.timeScale = 1f;

        if (endScreen != null)
            endScreen.SetActive(false);

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.StartCoroutine(
                SceneTransitionManager.Instance.LoadMainMenuWithFade()
            );
        }
        else
        {
            Debug.LogWarning("SceneTransitionManager missing. Cannot return to MainMenu.");
        }
    }
}