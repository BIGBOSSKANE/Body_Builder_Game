using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonSwitchScript : MonoBehaviour
{
    laserRouter laserRouter;
    int triggerCounter;
    public bool triggered;

    void OnTriggerEnter2D(Collider2D col)
    {
        triggerCounter ++;
        if(triggerCounter == 1) // this will ensure that the button will not trigger again until everything is removed from it
        {
            triggered = !triggered;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        triggerCounter --;
        if(triggerCounter == 0)
        {
            triggered = false;
        }
    }
}
