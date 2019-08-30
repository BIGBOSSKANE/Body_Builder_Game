using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerCell : activate
{
    Rigidbody2D rb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

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
