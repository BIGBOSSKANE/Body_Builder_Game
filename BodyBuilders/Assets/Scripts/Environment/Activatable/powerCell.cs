/*
Creator: Daniel
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerCell : activate
{
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    bool wasActivated;
    public int identifier;
    public Sprite unchargedSprite;
    public Sprite chargedSprite;
    public Sprite overchargedSprite;


    void Start()
    {
        gameObject.name = "PowerCell";
        rb = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        wasActivated = false;
    }

    void Update()
    {
        if (activated != wasActivated) // on the next frame after the power cell changes charge
        {
            if (activated)
            {
                if (overcharge)
                {
                    spriteRenderer.sprite = overchargedSprite;
                }
                else
                {
                    spriteRenderer.sprite = chargedSprite;
                }

                if (gameObject.transform.parent != null && gameObject.transform.parent.gameObject.tag == "PowerStation")
                {
                    transform.parent.gameObject.GetComponent<powerStation>().UpdateCharge(activated, overcharge);
                }
                wasActivated = true;
            }
            else
            {
                gameObject.name = "unchargedPowerCell";
                spriteRenderer.sprite = unchargedSprite;
                wasActivated = false;
            }
        }

        wasActivated = activated;
    }
}
