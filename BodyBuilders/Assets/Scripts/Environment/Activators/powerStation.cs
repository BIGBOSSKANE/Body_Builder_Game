﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerStation : activate
{
    [Tooltip("Is there a power cell in the station?")] public bool holdingPowerCell = false; // has a power cell been placed in the power station, activating it
    [Tooltip("Can the power station store charge after the power cell is removed?")] public bool canStoreCharge = false;
    public string currentCore = "Empty";
    GameObject attachedPowerCell;
    SpriteRenderer spriteRenderer;
    Rigidbody2D attachedRb;
    powerCell powerCell;
    int powerCellIdentifier;
    [Tooltip("What game objects does this activate, increase the size to add more")] public GameObject [] activates;

    void Start()
    {
        currentCore = "Empty";
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if(transform.childCount > 0f)
        {
            currentCore = transform.GetChild(0).gameObject.name;
            attachedPowerCell = transform.GetChild(0).gameObject;
            attachedRb = attachedPowerCell.GetComponent<Rigidbody2D>();
            attachedRb.isKinematic = true;
            powerCell = attachedPowerCell.GetComponent<powerCell>();
            activated = powerCell.activated;
            overcharge = powerCell.overcharge;
        }

        if(activated)
        {
            if(overcharge) spriteRenderer.color = new Color32(166 , 254 , 0 , 255);
            else spriteRenderer.color = new Color32(254 , 161 , 0 , 255);
        }
        else
        {
            spriteRenderer.color = Color.black;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(attachedPowerCell != null)
        {
            if(transform.childCount == 0)
            {
                DetachPowercell();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "powerCell" && !holdingPowerCell)
        {
            Debug.Log("Added powercell");
            attachedPowerCell = col.gameObject;
            attachedRb = attachedPowerCell.GetComponent<Rigidbody2D>();
            attachedRb.velocity = Vector2.zero;
            attachedRb.isKinematic = true;
            attachedPowerCell.transform.position = transform.position;
            attachedPowerCell.transform.parent = gameObject.transform;
            powerCell = attachedPowerCell.GetComponent<powerCell>();
            powerCellIdentifier = powerCell.identifier;

            UpdateCharge(powerCell.activated , powerCell.overcharge);
        }
    }

    public void UpdateCharge(bool activate , bool overcharged)
    {
        if(activate) activated = true;
        if(overcharged && overchargeable) overcharge = true;

        if(activated)
        {    
            if(overcharge) spriteRenderer.color = new Color32(166 , 254 , 0 , 255);
            else spriteRenderer.color = new Color32(254 , 165 , 0 , 255);
        }
        else spriteRenderer.color = Color.black;
        StateChange(activated);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.tag == "powerCell" && holdingPowerCell && col.gameObject.GetComponent<powerCell>().identifier == powerCellIdentifier)
        {
            DetachPowercell();
        }
    }

    void DetachPowercell()
    {
        Debug.Log("Exited");
        attachedPowerCell = null;
        holdingPowerCell = false;
        powerCellIdentifier = 0;
        powerCell = null;
        if(!canStoreCharge)
        {
            activated = false;
            overcharge = false;
            spriteRenderer.color = Color.black;
        }
        StateChange(activated);
    }

    void StateChange(bool active)
    {
        //AkSoundEngine.PostEvent("ButtonPress" , gameObject);
        foreach (GameObject activateable in activates)
        {
            if(activateable != null)
            {
                activate activateScript = activateable.GetComponent<activate>();
                activateScript.Activate(activated);
                if(overcharge)
                {
                    activateScript.Overcharge(overcharge); // if the power station is overcharged, overcharge the activatable dependent object
                }
            }
        }
    }
}
