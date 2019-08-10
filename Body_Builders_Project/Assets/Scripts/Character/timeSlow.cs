using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timeSlow : MonoBehaviour
{
    float slowedSpeed = 0.2f;
    bool slowTime = false;
    bool restoreTime = false;
    float slowDownDuration = 2f;
    public float actualTimeScale;
    float holdTimer = 0f;

    public void TimeSlow()
    {
        Time.timeScale = slowedSpeed;
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
        Time.timeScale += (1f / slowDownDuration) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale , 0f , 1f);
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

/*  Need to talk to Brad about this code, it works except that the floats keep being equal to 2 values
    {
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        Debug.Log("slow down duration: " + slowDownDuration); // for some reason slowDownDuration is returning 2 values when it is a public variable
        Debug.Log("slowed speed: " + slowedSpeed);
        if(slowTime == true)
        {
            Time.timeScale -= Mathf.Clamp((1f / (slowDownDuration * 0.15f)) * Time.unscaledDeltaTime , 0f , Time.timeScale);
        }

        if(Time.timeScale <= slowedSpeed)
        {
            Debug.Log("hold");
            slowTime = false;
            
            holdTimer += Time.deltaTime*(1f/slowedSpeed);
            Debug.Log("holdTimer: " + holdTimer);
            if(holdTimer >= (slowDownDuration * 0.7f))
            {
                Debug.Log("restore");
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
            Time.timeScale += Mathf.Clamp((1f / (slowDownDuration * 0.15f)) * Time.unscaledDeltaTime , slowedSpeed , 1f - Time.timeScale);
        }
        
        actualTimeScale = Time.timeScale;
        Time.timeScale = Mathf.Clamp(Time.timeScale, slowedSpeed , 1f);
    }
*/
}