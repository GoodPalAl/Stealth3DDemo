using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    // Volume Sliders
    [SerializeField]
    AudioMixer audioMixer = null;

    // Graphics
    [SerializeField]
    Dropdown graphicsDropdown = null;
    enum GFX_Options
    {
        TRASH, LOW, MEDIUM, HIGH, SUPER, ULTRA
    };
    [SerializeField]
    GFX_Options gfxDefaultIndex;

    // Resolutions
    [SerializeField]
    Dropdown resolutionDropdown = null;
    Resolution[] resolutions;

    // Volume
    [SerializeField]
    Slider musicSlider = null;
    [SerializeField]
    Slider sfxSlider = null;

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currResIndex = 0;
        for (int i = 0; i < resolutions.Length; ++i)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currResIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currResIndex;
        resolutionDropdown.RefreshShownValue();

        QualitySettings.SetQualityLevel((int)gfxDefaultIndex);
        graphicsDropdown.value = (int)gfxDefaultIndex;
        graphicsDropdown.RefreshShownValue();

        float musicVolume = 0;
        audioMixer.GetFloat("music", out musicVolume);
        musicSlider.value = musicVolume;

        float sfxVolume = 0;
        audioMixer.GetFloat("sfx", out sfxVolume);
        sfxSlider.value = sfxVolume;
    }

    public void SetResolution (int resIndex)
    {
        Resolution res = resolutions[resIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("music", volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("sfx", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
