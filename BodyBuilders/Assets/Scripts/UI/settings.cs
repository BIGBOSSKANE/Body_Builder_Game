using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class settings : MonoBehaviour
{
    //set public audio vairable

  //public void SetVolume (float volume)
  //{
  //    
  //}

    public void SetQuality (int QualityIndex)
    {
        QualitySettings.SetQualityLevel(QualityIndex);
    }
}
