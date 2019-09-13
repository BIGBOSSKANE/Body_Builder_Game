/*
Creator: Daniel
Created: 13/04/2019
Laste Edited by: Daniel
Last Edit: 13/09/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnerSprites : MonoBehaviour
{
    public Sprite basic;
    public Sprite augment1;
    public Sprite augment2;

    public void SetSprite(int configuration , int partConfig)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        if(configuration == 1)
        {
            spriteRenderer.sprite = basic;
        }
        else if(configuration == 2)
        {
            spriteRenderer.sprite = augment1;
        }
        else if(configuration == 3)
        {
            spriteRenderer.sprite = augment2;
        }

        // Part Configuration

        if(partConfig == 1)
        {
            spriteRenderer.enabled = false;
        }
        else if(partConfig == 2)
        {
            if(gameObject.name == "LegIndicator") spriteRenderer.enabled = false;
            else if(gameObject.name == "ArmIndicator") spriteRenderer.enabled = true;
        }
        else if(partConfig == 3)
        {
            if(gameObject.name == "LegIndicator") spriteRenderer.enabled = true;
            else if(gameObject.name == "ArmIndicator") spriteRenderer.enabled = false;
        }
        else if(partConfig == 4)
        {
            if(gameObject.name == "LegIndicator") spriteRenderer.enabled = true;
            else if(gameObject.name == "ArmIndicator") spriteRenderer.enabled = true;
        }
    }
}
