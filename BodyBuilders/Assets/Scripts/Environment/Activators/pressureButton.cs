/*
Creator: Daniel
Created 29/08/2019
Last Edited by: Daniel
Last Edit 29/08/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pressureButton : MonoBehaviour
{
    public bool activated;
    public bool heavyButton; // does the button require a heavyliftable or powercell to weigh it down?
    public bool right = true; // if 2 buttons are used to move or rotate something, does this apply a clockwise rotation, or a force ot the right?
    public bool overcharger = false; // does the button trigger the overcharge function of the activatable dependent object
    public GameObject [] activates;

    void OnTriggerExit2D(Collider2D col)
    {
        bool wasActivated = activated;

        activated = false;

        if(wasActivated != activated)
        {
            StateChange(activated);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        bool wasActivated = activated;
        
        if(!heavyButton)
        {
            activated = true;
            if(wasActivated != activated)
            {
                StateChange(activated);
            }
        }
        else if(heavyButton && (col.gameObject.tag == "HeavyLiftable" || col.gameObject.tag == "powerCell"))
        {
            activated = true;
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
