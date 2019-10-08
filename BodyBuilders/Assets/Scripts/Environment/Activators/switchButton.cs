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
    [Tooltip("Is the button currently triggered?")] public bool activated = false;
    [Tooltip("If the button is used to rotate something, is this left/clockwise or right/anticlockwise?")] public bool right = true; // if 2 buttons are used to move or rotate something, does this apply a clockwise rotation, or a force ot the right?
    [Tooltip("Switch rotation direction when deactivating?")] public bool switchRight = true; // if 2 buttons are used to move or rotate something, does this apply a clockwise rotation, or a force ot the right?
    [Tooltip("Does this button overcharge the activated dependents?")] public bool overcharger = false; // does the button overcharge activatable dependent objects?
    [Tooltip("Does the button avoid ever deactivating?")] public bool stayActivated = false;
    [Tooltip("What does this button activate?")] public GameObject [] activates;
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
            if(!(stayActivated && activated))
            {
                activated = !activated;
                StateChange(activated);
            }
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
