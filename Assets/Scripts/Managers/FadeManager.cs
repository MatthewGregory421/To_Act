using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.2f;

    private Coroutine currentFade;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator FadeOut()
    {
        if (fadeImage == null)
        {
            Debug.LogWarning("FadeManager: No fade image assigned.");
            yield break;
        }

        float timer = 0f;

        while (timer < fadeDuration)
        {
            if (fadeImage == null)
                yield break;

            timer += Time.unscaledDeltaTime;

            Color color = fadeImage.color;
            color.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadeImage.color = color;

            yield return null;
        }

        if (fadeImage != null)
        {
            Color final = fadeImage.color;
            final.a = 1f;
            fadeImage.color = final;
        }
    }

    public IEnumerator FadeIn()
    {
        if (fadeImage == null)
        {
            Debug.LogWarning("FadeManager: No fade image assigned.");
            yield break;
        }

        float timer = 0f;

        while (timer < fadeDuration)
        {
            if (fadeImage == null)
                yield break;

            timer += Time.unscaledDeltaTime;

            Color color = fadeImage.color;
            color.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            fadeImage.color = color;

            yield return null;
        }

        if (fadeImage != null)
        {
            Color final = fadeImage.color;
            final.a = 0f;
            fadeImage.color = final;
        }
    }
}