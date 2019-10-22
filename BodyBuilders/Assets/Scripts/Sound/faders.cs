using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class faders : MonoBehaviour
{
    public void UpdateMasterVolume(float volume)
    {
        AkSoundEngine.SetRTPCValue("MasterFader" , volume);
    }
    
    public void UpdateAmbienceVolume(float volume)
    {
        AkSoundEngine.SetRTPCValue("AmbienceFader" , volume);
    }

    public void UpdateMusicVolume(float volume)
    {
        AkSoundEngine.SetRTPCValue("MusicFader" , volume);
    }
    
    public void UpdateSFXVolume(float volume)
    {
        AkSoundEngine.SetRTPCValue("SFXVVolume" , volume);
    }
    
    public void UpdateVoicesVolume(float volume)
    {
        AkSoundEngine.SetRTPCValue("VoicesFader" , volume);
    }
}
