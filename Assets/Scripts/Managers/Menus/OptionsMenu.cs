using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;

public class OptionsMenu : MonoBehaviour
{
    [Header("Audio Sliders")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider narrationVolumeSlider;

    [Header("Display")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;
    private Bus narrationBus;

    private Resolution[] resolutions;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;

    private UISFXManager UI => UISFXManager.Instance;

    private void Awake()
    {
        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        narrationBus = RuntimeManager.GetBus("bus:/Narration");
    }

    private void Start()
    {
        SetupResolutions();
        LoadSettings();
    }

    public void SetMasterVolume(float volume)
    {
        float adjustedVolume = SliderToVolume(volume);
        Debug.Log("Master Volume: " + volume + " Adjusted: " + adjustedVolume);
        masterBus.setVolume(adjustedVolume);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        float adjustedVolume = SliderToVolume(volume);
        Debug.Log("Music Volume: " + volume + " Adjusted: " + adjustedVolume);
        musicBus.setVolume(adjustedVolume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        float adjustedVolume = SliderToVolume(volume);
        Debug.Log("SFX Volume: " + volume + " Adjusted: " + adjustedVolume);
        sfxBus.setVolume(adjustedVolume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetNarrationVolume(float volume)
    {
        float adjustedVolume = SliderToVolume(volume);
        Debug.Log("Narration Volume: " + volume + " Adjusted: " + adjustedVolume);
        narrationBus.setVolume(adjustedVolume);
        PlayerPrefs.SetFloat("NarrationVolume", volume);
    }

    private float SliderToVolume(float sliderValue)
    {
        sliderValue = Mathf.Clamp01(sliderValue);

        if (sliderValue <= 0.0001f)
            return 0f;

        return Mathf.Pow(sliderValue, 2f);
    }

    private void SetupResolutions()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;

            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int index)
    {
        if (index < 0 || index >= resolutions.Length)
            return;

        Resolution resolution = resolutions[index];

        Screen.SetResolution(
            resolution.width,
            resolution.height,
            Screen.fullScreen
        );

        PlayerPrefs.SetInt("ResolutionIndex", index);
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
    }

    public void CloseOptions()
    {
        if (UISFXManager.Instance != null)
            UISFXManager.Instance.PlayUIBack();

        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    private void LoadSettings()
    {
        float master = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float music = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float narration = PlayerPrefs.GetFloat("NarrationVolume", 1f); 

        masterBus.setVolume(SliderToVolume(master));
        musicBus.setVolume(SliderToVolume(music));
        sfxBus.setVolume(SliderToVolume(sfx));
        narrationBus.setVolume(SliderToVolume(narration));

        masterVolumeSlider.value = master;
        musicVolumeSlider.value = music;
        sfxVolumeSlider.value = sfx;
        narrationVolumeSlider.value = narration;

        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        Screen.fullScreen = fullscreen;
        fullscreenToggle.isOn = fullscreen;

        int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", resolutions.Length - 1);
        resolutionIndex = Mathf.Clamp(resolutionIndex, 0, resolutions.Length - 1);

        resolutionDropdown.value = resolutionIndex;
        resolutionDropdown.RefreshShownValue();

        SetResolution(resolutionIndex);
    }
}