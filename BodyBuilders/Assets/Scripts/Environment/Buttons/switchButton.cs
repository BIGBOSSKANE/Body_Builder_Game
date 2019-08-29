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
        foreach (GameObject activateable in activates)
        {
            activateable.GetComponent<activate>().Activate(active);
        }
    }
}
