using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    private Resolution[] resolutions;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;

    private void Start()
    {
        SetupResolutions();
        LoadSettings();
    }

    public void SetMasterVolume(float volume)
    {
        if (AudioSettingsManager.Instance != null)
            AudioSettingsManager.Instance.SetMasterVolume(volume);
        else
            PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        if (AudioSettingsManager.Instance != null)
            AudioSettingsManager.Instance.SetMusicVolume(volume);
        else
            PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (AudioSettingsManager.Instance != null)
            AudioSettingsManager.Instance.SetSFXVolume(volume);
        else
            PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetNarrationVolume(float volume)
    {
        if (AudioSettingsManager.Instance != null)
            AudioSettingsManager.Instance.SetNarrationVolume(volume);
        else
            PlayerPrefs.SetFloat("NarrationVolume", volume);
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
        resolutionDropdown.SetValueWithoutNotify(currentResolutionIndex);
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
        if (AudioSettingsManager.Instance != null)
        {
            AudioSettingsManager.Instance.SaveSettings();
        }

        if (UISFXManager.Instance != null)
        {
            UISFXManager.Instance.PlayUIBack();
        }

        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    private void LoadSettings()
    {
        float master = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float music = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float narration = PlayerPrefs.GetFloat("NarrationVolume", 1f);

        masterVolumeSlider.SetValueWithoutNotify(master);
        musicVolumeSlider.SetValueWithoutNotify(music);
        sfxVolumeSlider.SetValueWithoutNotify(sfx);
        narrationVolumeSlider.SetValueWithoutNotify(narration);

        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        Screen.fullScreen = fullscreen;
        fullscreenToggle.SetIsOnWithoutNotify(fullscreen);

        int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", resolutions.Length - 1);
        resolutionIndex = Mathf.Clamp(resolutionIndex, 0, resolutions.Length - 1);

        resolutionDropdown.SetValueWithoutNotify(resolutionIndex);
        resolutionDropdown.RefreshShownValue();

        SetResolution(resolutionIndex);
    }
}