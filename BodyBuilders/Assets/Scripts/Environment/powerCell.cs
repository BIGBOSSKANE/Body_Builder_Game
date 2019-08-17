using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerCell : MonoBehaviour
{
    public bool charged = false;

    void Update()
    {
        if(charged)
        {
            gameObject.name = "chargedPowerCell";
        }
        else
        {
            gameObject.name = "unchargedPowerCell";
        }
    }
}
