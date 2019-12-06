/*
Creator: Daniel
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class editorFaders : MonoBehaviour
{
    faders faders;
    [Range(0 ,100)] public float Master = 100;
    [Range(0 ,100)] public float Ambience = 100;
    [Range(0 ,100)] public float Music = 100;
    [Range(0 ,100)] public float SFX = 100;
    [Range(0 ,100)] public float Voice = 100;
    float previousMaster = 200;
    float previousAmbience = 200;
    float previousMusic = 200;
    float previousSFX = 200;
    float previousVoice = 200;
    int frames = 0;

    void Start()
    {
        faders = GetComponent<faders>();
    }

    void Update()
    {
        frames ++;
        if(frames % 10 == 0) // check every 10 frames
        {
            if(Master != previousMaster)
            {
                faders.UpdateMasterVolume(Master);
                previousMaster = Master;
            }
            else if(Ambience != previousAmbience)
            {
                faders.UpdateAmbienceVolume(Ambience);
                previousAmbience = Ambience;
            }
            else if(Music != previousMusic)
            {
                faders.UpdateMusicVolume(Music);
                previousMusic = Music;
            }
            else if(SFX != previousSFX)
            {
                faders.UpdateSFXVolume(SFX);
                previousSFX = SFX;
            }
            else if(Voice != previousVoice)
            {
                faders.UpdateVoicesVolume(Voice);
                previousVoice = Voice;
            }
        }
    }

    public void UpdateMasterVolume(float volume)
    {
        Master = volume;
        faders.UpdateMasterVolume(Master);
    }
    
    public void UpdateAmbienceVolume(float volume)
    {
        Ambience = volume;
        faders.UpdateAmbienceVolume(Ambience);
    }

    public void UpdateMusicVolume(float volume)
    {
        faders.UpdateMusicVolume(Music);
        Music = volume;
    }
    
    public void UpdateSFXVolume(float volume)
    {
        faders.UpdateVoicesVolume(Voice);
        SFX = volume;
    }
    
    public void UpdateVoicesVolume(float volume)
    {
        Voice = volume;
    }
}
