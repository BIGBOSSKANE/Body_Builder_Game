using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerCell : activate
{
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    bool wasActivated;
    bool wasOvercharged;
    public int identifier;


    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        wasActivated = false;
    }

    void Update()
    {
        if(activated != wasActivated) // if the power cell is charged
        {
            if(activated)
            {
                if(overcharge)
                {
                    gameObject.name = "overchargedPowerCell";
                    spriteRenderer.color = Color.green;
                }
                else
                {
                    gameObject.name = "chargedPowerCell";
                    spriteRenderer.color = Color.yellow;
                }
                if(gameObject.transform.parent != null && gameObject.transform.parent.gameObject.tag == "PowerStation")
                {
                    transform.parent.gameObject.GetComponent<powerStation>().UpdateCharge(activated , overcharge);
                }
                wasActivated = true;
            }
            else
            {
                gameObject.name = "unchargedPowerCell";
                spriteRenderer.color = Color.grey;
                wasActivated = false;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if(wasOvercharged != overcharge && overcharge)
        {
            activated = true;
        }
    }
}
