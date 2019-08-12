using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timeSlow : MonoBehaviour
{
    public float slowedSpeed = 0.2f;
    bool slowTime = false;
    bool restoreTime = false;
    public float slowDownDuration = 2f;
    public float actualTimeScale;
    float holdTimer = 0f;

    public void TimeSlow()
    {
        restoreTime = false;
        slowTime = true;
    }

    public void TimeNormal()
    {
        Time.timeScale = 1f;
        slowTime = false;
        restoreTime = false;
    }

    void Update()
    {
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        if(slowTime == true)
        {
            Time.timeScale -= Mathf.Clamp((1f / (slowDownDuration * 0.15f)) * Time.unscaledDeltaTime , 0f , Time.timeScale);
        }

        if(Time.timeScale <= slowedSpeed)
        {
            slowTime = false;
            
            holdTimer += Time.deltaTime*(1f/slowedSpeed);
            if(holdTimer >= (slowDownDuration * 0.4f))
            {
                restoreTime = true;
            }
            
        }
        else if(Time.timeScale >= 1f && restoreTime == true)
        {
            restoreTime = false;
        }

        if(restoreTime == true)
        {
            holdTimer = 0f;
            Time.timeScale += Mathf.Clamp((1f / (slowDownDuration * 0.45f)) * Time.unscaledDeltaTime , slowedSpeed , 1f - Time.timeScale);
        }
        
        actualTimeScale = Time.timeScale;
        Time.timeScale = Mathf.Clamp(Time.timeScale, slowedSpeed , 1f);
    }
}