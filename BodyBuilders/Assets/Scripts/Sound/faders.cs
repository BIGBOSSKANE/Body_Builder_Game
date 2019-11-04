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

    public void UpdateMasterVolume(float volume)
    {
        Master = volume;
        AkSoundEngine.SetRTPCValue("MasterFader" , volume);
    }
    
    public void UpdateAmbienceVolume(float volume)
    {
        Ambience = volume;
        AkSoundEngine.SetRTPCValue("AmbienceFader" , volume);
    }

    public void UpdateMusicVolume(float volume)
    {
        Music = volume;
        AkSoundEngine.SetRTPCValue("MusicFader" , volume);
    }
    
    public void UpdateSFXVolume(float volume)
    {
        SFX = volume;
        AkSoundEngine.SetRTPCValue("SFXVVolume" , volume);
    }
    
    public void UpdateVoicesVolume(float volume)
    {
        Voices = volume;
        AkSoundEngine.SetRTPCValue("VoicesFader" , volume);
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
