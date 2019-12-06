/*
Creator: Daniel
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class faders : MonoBehaviour
{
    [Range(0 ,100)] public float Master = 50;
    [Range(0 ,100)] public float Ambience = 50;
    [Range(0 ,100)] public float Music = 50;
    [Range(0 ,100)] public float SFX = 50;
    [Range(0 ,100)] public float Voices = 50;
    float previousMaster = 200;
    float previousAmbience = 200;
    float previousMusic = 200;
    float previousSFX = 200;
    float previousVoices = 200;
    int frames = 0;

    void Awake()
    {
        Master = PlayerPrefs.GetFloat("MasterFader" , 50f);
        UpdateMasterVolume(Master);
        Ambience = PlayerPrefs.GetFloat("AmbienceFader" , 50f);
        UpdateAmbienceVolume(Ambience);
        Music = PlayerPrefs.GetFloat("MusicFader" , 50f);
        UpdateMusicVolume(Music);
        SFX = PlayerPrefs.GetFloat("SFXFader" , 50f);
        UpdateSFXVolume(SFX);
        Voices = PlayerPrefs.GetFloat("VoicesFader" , 50f);
        UpdateVoicesVolume(Voices);
    }

    public void UpdateMasterVolume(float volume)
    {
        Master = volume;
        AkSoundEngine.SetRTPCValue("MasterFader" , volume);
        PlayerPrefs.SetFloat("MasterFader" , volume);
    }
    
    public void UpdateAmbienceVolume(float volume)
    {
        Ambience = volume;
        AkSoundEngine.SetRTPCValue("AmbienceFader" , volume);
        PlayerPrefs.SetFloat("AmbienceFader" , volume);
    }

    public void UpdateMusicVolume(float volume)
    {
        Music = volume;
        AkSoundEngine.SetRTPCValue("MusicFader" , volume);
        PlayerPrefs.SetFloat("MusicFader" , volume);
    }
    
    public void UpdateSFXVolume(float volume)
    {
        SFX = volume;
        AkSoundEngine.SetRTPCValue("SFXVolume" , volume);
        PlayerPrefs.SetFloat("SFXVolume" , volume);
    }
    
    public void UpdateVoicesVolume(float volume)
    {
        Voices = volume;
        AkSoundEngine.SetRTPCValue("VoicesFader" , volume);
        PlayerPrefs.SetFloat("VoicesFader" , volume);
    }

    void Update() // this allows for volume control in the editor;
    {
        frames ++;
        if(frames % 10 == 0) // check every 10 frames
        {
            if(Master != previousMaster)
            {
                UpdateMasterVolume(Master);
                previousMaster = Master;
            }
            else if(Ambience != previousAmbience)
            {
                UpdateAmbienceVolume(Ambience);
                previousAmbience = Ambience;
            }
            else if(Music != previousMusic)
            {
                UpdateMusicVolume(Music);
                previousMusic = Music;
            }
            else if(SFX != previousSFX)
            {
                UpdateSFXVolume(SFX);
                previousSFX = SFX;
            }
            else if(Voices != previousVoices)
            {
                UpdateVoicesVolume(Voices);
                previousVoices = Voices;
            }
        }
    }
}
