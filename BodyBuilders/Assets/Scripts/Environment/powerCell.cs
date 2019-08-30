using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerCell : activate
{
    void Update()
    {
        if(activated) // if the power cell is charged
        {
            if(overcharge)
            {
                gameObject.name = "overchargedPowerCell";
            }
            else
            {
                gameObject.name = "chargedPowerCell";
            }
        }
        else
        {
            gameObject.name = "unchargedPowerCell";
        }
    }
}
