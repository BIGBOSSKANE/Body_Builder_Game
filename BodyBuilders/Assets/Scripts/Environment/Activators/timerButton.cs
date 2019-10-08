/*
Creator: Daniel
Created 29/08/2019
Last Edited by: Daniel
Last Edit 29/08/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timerButton : MonoBehaviour
{
    [Tooltip("Is the button currently triggered?")] public bool activated = false;
    [Tooltip("If the button is used to rotate something, is this left/clockwise or right/anticlockwise?")] public bool right = true; // if 2 buttons are used to move or rotate something, does this apply a clockwise rotation, or a force ot the right?
    [Tooltip("Switch rotation direction when deactivating?")] public bool switchRight = true; // if 2 buttons are used to move or rotate something, does this apply a clockwise rotation, or a force ot the right?
    [Tooltip("Does this button overcharge the activated dependents?")] public bool overcharger = false; // does the button overcharge activatable dependent objects?
    [Tooltip("Total time the button remains active for?")] public float totalTime = 5f;
    float timeRemaining;
    [Tooltip("What does this button activate?")] public GameObject [] activates;
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
        AkSoundEngine.PostEvent("ButtonPress" , gameObject);
        foreach (GameObject activateable in activates)
        {
            if(activateable != null)
            {
                activate activateScript = activateable.GetComponent<activate>();
                activateScript.Activate(active);
                if(!activated && switchRight) activateScript.ActivateDirection(!right);
                else activateScript.ActivateDirection(right);
                if(overcharger)
                {
                    activateScript.Overcharge(active); // if the button is active, overcharge the activatable dependent object
                }
            }
        }
    }
}
