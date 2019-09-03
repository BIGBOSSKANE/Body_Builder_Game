/*
Creator: Daniel
Created 29/08/2019
Last Edited by: Daniel
Last Edit 29/08/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switchButton : MonoBehaviour
{
    public bool activated;
    public bool right = true; // if 2 buttons are used to move or rotate something, does this apply a clockwise rotation, or a force ot the right?
    public bool overcharger = false; // does the button overcharge activatable dependent objects?
    public GameObject [] activates;
    int triggerCounter;

    void Start()
    {
        triggerCounter = 0;
        activated = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        triggerCounter ++;
        if(triggerCounter == 1)
        {
            activated = !activated;
            StateChange(activated);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        triggerCounter --;
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
                activateScript.ActivateDirection(right);
                if(overcharger)
                {
                    activateScript.Overcharge(active); // if the button is active, overcharge the activatable dependent object
                }
            }
        }
    }
}
