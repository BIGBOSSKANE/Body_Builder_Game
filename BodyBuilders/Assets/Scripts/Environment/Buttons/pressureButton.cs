using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pressureButton : MonoBehaviour
{
    public bool activated;
    public bool heavyButton; // does the button require a heavyliftable or powercell to weigh it down?
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
            //GameObject.GetComponent<activate>().Activate(active);
        }
    }
}
