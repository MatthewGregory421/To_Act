using UnityEngine;

public class EndOfDemoTrigger : MonoBehaviour
{
    [Header("UI")]
    public GameObject endScreen;

    private bool triggered = false;

    private void Start()
    {
        if (endScreen != null)
        {
            endScreen.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            // Stop time
            Time.timeScale = 0f;

            // Show end screen
            if (endScreen != null)
            {
                endScreen.SetActive(true);
            }
        }
    }

    public void ReturnToMainMenu()
    {
        UISFXManager.Instance?.PlayUIConfirm();

        Time.timeScale = 1f;

        if (endScreen != null)
            endScreen.SetActive(false);

        StartCoroutine(
            SceneTransitionManager.Instance.LoadSceneDirect("MainMenu")
        );
    }
}