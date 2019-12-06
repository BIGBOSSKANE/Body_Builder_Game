//Created by Kane Girvan
//Edited by Daniel
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class settings : MonoBehaviour
{

    public Dropdown resolutionDD;
    faders faders;

    public Slider masterFader;
    public Slider musicFader;
    public Slider sfxFader;

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

        faders = GameObject.Find("WwiseGlobal").gameObject.GetComponent<faders>();
        masterFader.value = PlayerPrefs.GetFloat("MasterFader" , 50f);
        sfxFader.value = PlayerPrefs.GetFloat("SFXFader" , 50f);
        musicFader.value = PlayerPrefs.GetFloat("MusicFader" , 50f);
    }

    public void SetResolution (int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    //Master Volume
    public void SetMasterVolume (float volume)
    {
        faders.UpdateMasterVolume(volume);
    }

    //Music Volume
    public void SetMusicVolume(float volume)
    {
        faders.UpdateMusicVolume(volume);
    }

    //Effects Volume
    public void SetEffectsVolume(float volume)
    {
        faders.UpdateSFXVolume(volume);
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