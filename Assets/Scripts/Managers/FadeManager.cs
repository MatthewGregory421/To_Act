using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.25f;

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
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            Color color = fadeImage.color;
            color.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadeImage.color = color;

            yield return null;
        }

        Color final = fadeImage.color;
        final.a = 1f;
        fadeImage.color = final;
    }

    public IEnumerator FadeIn()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            Color c = fadeImage.color;
            c.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            fadeImage.color = c;

            yield return null;
        }

        Color final = fadeImage.color;
        final.a = 0f;
        fadeImage.color = final;
    }
}