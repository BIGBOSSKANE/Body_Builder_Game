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
    public bool activated = false;
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
}
