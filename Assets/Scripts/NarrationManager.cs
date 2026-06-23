using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
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
    [SerializeField] private string[] narrationLines;
    [SerializeField] private float textFadeTime = 0.25f;

    private readonly Queue<int> narrationQueue = new Queue<int>();
    private bool isPlaying = false;

    public bool IsPlaying => isPlaying;

    public void RequestNarration(int index)
    {
        Debug.Log($"Narration requested: {index}");

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

            Debug.Log($"Playing narration area {areaValue}, line {index}");

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
        if (narrationText == null)
            yield break;

        narrationText.text = line;

        Color color = narrationText.color;
        color.a = 0f;
        narrationText.color = color;

        float timer = 0f;

        while (timer < textFadeTime)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, timer / textFadeTime);
            narrationText.color = color;
            yield return null;
        }

        color.a = 1f;
        narrationText.color = color;

        yield return new WaitForSeconds(duration);

        timer = 0f;

        while (timer < textFadeTime)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, timer / textFadeTime);
            narrationText.color = color;
            yield return null;
        }

        color.a = 0f;
        narrationText.color = color;
        narrationText.text = "";
    }
}