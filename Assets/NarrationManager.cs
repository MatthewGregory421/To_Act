using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

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

            yield return new WaitForSeconds(duration + gapBetweenLines);
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
}