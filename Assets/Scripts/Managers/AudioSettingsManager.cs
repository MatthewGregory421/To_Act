using System;
using System.Collections;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

[DefaultExecutionOrder(-1000)]
public class AudioSettingsManager : MonoBehaviour
{
    public static AudioSettingsManager Instance { get; private set; }

    public bool IsReady { get; private set; }

    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;
    private Bus narrationBus;
    private Bus ambienceBus;
    private Bus uiBus;

    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";
    private const string NarrationVolumeKey = "NarrationVolume";
    private const string AmbienceVolumeKey = "AmbienceVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        StartCoroutine(InitializeFMOD());
    }

    private IEnumerator InitializeFMOD()
    {
        while (!RuntimeManager.IsInitialized)
        {
            yield return null;
        }

        Exception lastException = null;
        const int maxAttempts = 300;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            try
            {
                masterBus =
                    RuntimeManager.GetBus("bus:/");

                musicBus =
                    RuntimeManager.GetBus("bus:/Mix Buss/Music");

                sfxBus =
                    RuntimeManager.GetBus("bus:/Mix Buss/SFX");

                narrationBus =
                    RuntimeManager.GetBus("bus:/Mix Buss/Narration");

                ambienceBus =
                    RuntimeManager.GetBus("bus:/Ambience");

                uiBus =
                    RuntimeManager.GetBus("bus:/Mix Buss/UI");

                lastException = null;
            }
            catch (Exception exception)
            {
                lastException = exception;
            }

            bool allBusesValid =
                masterBus.isValid() &&
                musicBus.isValid() &&
                sfxBus.isValid() &&
                narrationBus.isValid() &&
                ambienceBus.isValid() &&
                uiBus.isValid();

            if (allBusesValid)
            {
                Debug.Log(
                    "AudioSettingsManager: All FMOD buses loaded successfully."
                );

                IsReady = true;
                ApplySavedVolumes();
                yield break;
            }

            yield return null;
        }

        Debug.LogError(
            "AudioSettingsManager could not load all FMOD buses after 300 frames.\n" +
            "Check that the bus names are correct and rebuild your FMOD banks.\n" +
            lastException
        );
    }

    public void ApplySavedVolumes()
    {
        float master =
            PlayerPrefs.GetFloat(MasterVolumeKey, 1f);

        float music =
            PlayerPrefs.GetFloat(MusicVolumeKey, 1f);

        float sfx =
            PlayerPrefs.GetFloat(SFXVolumeKey, 1f);

        float narration =
            PlayerPrefs.GetFloat(NarrationVolumeKey, 1f);

        float ambience =
            PlayerPrefs.GetFloat(AmbienceVolumeKey, 1f);

        SetBusVolume(masterBus, master, "Master");
        SetBusVolume(musicBus, music, "Music");
        SetBusVolume(sfxBus, sfx, "SFX");
        SetBusVolume(narrationBus, narration, "Narration");
        SetBusVolume(ambienceBus, ambience, "Ambience");

        // UI sounds are controlled by the SFX slider.
        SetBusVolume(uiBus, sfx, "UI");
    }

    public void SetMasterVolume(float value)
    {
        value = Mathf.Clamp01(value);

        PlayerPrefs.SetFloat(MasterVolumeKey, value);

        if (IsReady)
        {
            SetBusVolume(masterBus, value, "Master");
        }
    }

    public void SetMusicVolume(float value)
    {
        value = Mathf.Clamp01(value);

        PlayerPrefs.SetFloat(MusicVolumeKey, value);

        if (IsReady)
        {
            SetBusVolume(musicBus, value, "Music");
        }
    }

    public void SetSFXVolume(float value)
    {
        value = Mathf.Clamp01(value);

        PlayerPrefs.SetFloat(SFXVolumeKey, value);

        if (!IsReady)
            return;

        SetBusVolume(sfxBus, value, "SFX");

        // UI sounds use the SFX setting too.
        SetBusVolume(uiBus, value, "UI");
    }

    public void SetNarrationVolume(float value)
    {
        value = Mathf.Clamp01(value);

        PlayerPrefs.SetFloat(NarrationVolumeKey, value);

        if (IsReady)
        {
            SetBusVolume(narrationBus, value, "Narration");
        }
    }

    public void SetAmbienceVolume(float value)
    {
        value = Mathf.Clamp01(value);

        PlayerPrefs.SetFloat(AmbienceVolumeKey, value);

        if (IsReady)
        {
            SetBusVolume(ambienceBus, value, "Ambience");
        }
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
    }

    private void SetBusVolume(
        Bus bus,
        float sliderValue,
        string busName
    )
    {
        if (!bus.isValid())
        {
            Debug.LogWarning(
                $"Cannot change {busName} volume because its FMOD bus is invalid."
            );

            return;
        }

        float adjustedVolume =
            SliderToVolume(sliderValue);

        FMOD.RESULT result =
            bus.setVolume(adjustedVolume);

        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError(
                $"Failed to set {busName} bus volume. FMOD result: {result}"
            );
        }
    }

    private float SliderToVolume(float sliderValue)
    {
        sliderValue = Mathf.Clamp01(sliderValue);

        if (sliderValue <= 0.0001f)
            return 0f;

        return sliderValue * sliderValue;
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            PlayerPrefs.Save();
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}