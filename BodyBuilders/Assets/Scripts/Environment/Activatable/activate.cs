/*
Creator: Daniel
Created 29/08/2019
Last Edited by: Daniel
Last Edit 29/08/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activate : MonoBehaviour
{
    [Tooltip("Is it powered?")] public bool activated = false; // is the object powered and operating?
    [Tooltip("Is it in its advanced state?")] public bool overcharge = false; // is the object overcharged?
    [Tooltip("Does this have an overcharge state, or is it disabled?")] public bool overchargeable = true; // can overcharge be switched on and off by activators?
    [Tooltip("Does the object only track when the button value changes, instead of the on/off signal?")] public bool antiSync = true; // ignore the signal sent from the button, instead just get whenever it changes and use that to change this

    public void Activate(bool activateSignal)
    {
        if(antiSync)
        {
            activated = !activated;
        }
        else
        {
            activated = activateSignal;
        }
    }

    virtual public void ActivateDirection(bool right)
    {
        // overwrite with laser router code here
    }

    virtual public void Overcharge(bool overload)
    {
        // if additional code is required, overwrite this
        if(overchargeable)
        {
            if(overload)
            {
                overcharge = true;
            }
            else
            {
                overcharge = false;
            }
        }
    }
}
