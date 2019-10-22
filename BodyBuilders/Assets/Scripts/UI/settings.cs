using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class settings : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Dropdown resolutionDD;

    Resolution[] resolutions;

    void Start ()
    {
        resolutions = Screen.resolutions;

        resolutionDD.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && 
               resolutions[i].height == Screen.currentResolution.width)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDD.AddOptions(options);
        resolutionDD.value = currentResolutionIndex;
        resolutionDD.RefreshShownValue();
    }

    public void SetResolution (int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    //Master Volume
    public void SetMasterVolume (float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    //Music Volume
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("music", volume);
    }

    //Effects Volume
    public void SetEffectsVolume(float volume)
    {
        audioMixer.SetFloat("effects", volume);
    }

    //Quality Dropdown Selector
    public void SetQuality (int QualityIndex)
    {
        QualitySettings.SetQualityLevel(QualityIndex);
    }

    //Fullscreen Toggle
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}