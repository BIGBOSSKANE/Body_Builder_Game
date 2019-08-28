using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timerButton : MonoBehaviour
{
    public bool activated = false;
    public float totalTime = 5f;
    float timeRemaining;
    public GameObject [] activates;
    int triggerCounter;

    void Start()
    {
        triggerCounter = 0;
        timeRemaining = totalTime;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        triggerCounter ++;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        triggerCounter --;
    }

    void Update()
    {
        timeRemaining -= Time.deltaTime;
        {
            if(timeRemaining < 0f)
            {
                timeRemaining = 0f;
            }
        }

        if(triggerCounter > 0)
        {
            bool wasActivated = activated;
            timeRemaining = totalTime;
            activated = true;
            if(wasActivated != activated)
            {
                StateChange(activated);
            }
        }

        if(triggerCounter == 0 && timeRemaining == 0f)
        {
            bool wasActivated = activated;
            activated = false;
            if(wasActivated != activated)
            {
                StateChange(activated);
            }
        }
    }

    void StateChange(bool active)
    {
        foreach (GameObject activateable in activates)
        {
            //GameObject.GetComponent<activate>().Activate(active);
        }
    }
}
