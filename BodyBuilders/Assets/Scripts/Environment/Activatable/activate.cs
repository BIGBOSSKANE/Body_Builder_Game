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
    public bool activated = false; // is the object powered and operating?
    public bool overcharge = false; // is the object overcharged?
    public bool overchargeable = true; // can overcharge be switched on and off by activators?
    public bool antiSync = true; // ignore the signal sent from the button, instead just get whenever it changes and use that to change this

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
