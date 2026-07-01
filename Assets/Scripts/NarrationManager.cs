using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.UI;
using TMPro;

public class NarrationManager : MonoBehaviour
{
    [SerializeField] private EventReference narrationEvent;

    [Header("FMOD Parameters")]
    [SerializeField] private string areaParameter = "Music Selector";
    [SerializeField] private string narratorParameter = "NarratorSelect";

    [Header("Area")]
    [SerializeField] private float areaValue = 1f; // Hub = 1, Sadness = 3, etc.

    [Header("Narration Durations")]
    [SerializeField] private float[] narrationDurations;

    [Header("Spacing")]
    [SerializeField] private float gapBetweenLines = 0.25f;

    [Header("Subtitle / Thought Text")]
    [SerializeField] private TMP_Text narrationText;
    [SerializeField] private GameObject narrationPanel;
    [SerializeField] private Image narrationPanelImage;
    [SerializeField] private string[] narrationLines;
    [SerializeField] private float textFadeTime = 0.25f;

    [SerializeField]
    [Range(0f, 1f)]
    private float panelMaxAlpha = 100f / 255f;

    private readonly Queue<int> narrationQueue = new Queue<int>();
    private bool isPlaying = false;

    public bool IsPlaying => isPlaying;

    private void Start()
    {
        narrationPanel.SetActive(false);
    }

    public void RequestNarration(int index)
    {

        narrationQueue.Enqueue(index);

        if (!isPlaying)
        {
            StartCoroutine(PlayNarrationQueue());
        }
    }

    private IEnumerator PlayNarrationQueue()
    {
        isPlaying = true;

        while (narrationQueue.Count > 0)
        {
            int index = narrationQueue.Dequeue();

            RuntimeManager.StudioSystem.setParameterByName(areaParameter, areaValue);
            RuntimeManager.StudioSystem.setParameterByName(narratorParameter, index);

            RuntimeManager.PlayOneShot(narrationEvent);

            float duration = GetDuration(index);
            string line = GetNarrationLine(index);

            yield return StartCoroutine(ShowNarrationText(line, duration));

            yield return new WaitForSeconds(gapBetweenLines);
        }

        isPlaying = false;
    }

    private float GetDuration(int index)
    {
        if (index >= 0 && index < narrationDurations.Length)
        {
            return narrationDurations[index];
        }

        Debug.LogWarning($"No duration set for narration index {index}. Using fallback duration.");
        return 5f;
    }

    private string GetNarrationLine(int index)
    {
        if (index >= 0 && index < narrationLines.Length)
        {
            return narrationLines[index];
        }

        Debug.LogWarning($"No narration text set for index {index}.");
        return "";
    }

    private IEnumerator ShowNarrationText(string line, float duration)
    {
        if (narrationText == null || narrationPanel == null || narrationPanelImage == null)
            yield break;

        narrationText.text = line;
        narrationPanel.SetActive(true);

        Color textColor = narrationText.color;
        Color panelColor = narrationPanelImage.color;

        textColor.a = 0f;
        panelColor.a = 0f;

        narrationText.color = textColor;
        narrationPanelImage.color = panelColor;

        float timer = 0f;

        // Fade In
        while (timer < textFadeTime)
        {
            timer += Time.deltaTime;

            float t = timer / textFadeTime;

            textColor.a = Mathf.Lerp(0f, 1f, t);
            panelColor.a = Mathf.Lerp(0f, panelMaxAlpha, t);

            narrationText.color = textColor;
            narrationPanelImage.color = panelColor;

            yield return null;
        }

        textColor.a = 1f;
        panelColor.a = panelMaxAlpha;

        narrationText.color = textColor;
        narrationPanelImage.color = panelColor;

        yield return new WaitForSeconds(duration);

        timer = 0f;

        // Fade Out
        while (timer < textFadeTime)
        {
            timer += Time.deltaTime;

            float t = timer / textFadeTime;

            textColor.a = Mathf.Lerp(1f, 0f, t);
            panelColor.a = Mathf.Lerp(panelMaxAlpha, 0f, t);

            narrationText.color = textColor;
            narrationPanelImage.color = panelColor;

            yield return null;
        }

        textColor.a = 0f;
        panelColor.a = 0f;

        narrationText.color = textColor;
        narrationPanelImage.color = panelColor;

        narrationText.text = "";
        narrationPanel.SetActive(false);
    }
}