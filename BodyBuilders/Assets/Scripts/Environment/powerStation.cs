using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerStation : activate
{
    public bool holdingPowerCell = false; // has a power cell been placed in the power station, activating it
    public bool canStoreCharge = false;
    public string currentCore = "Empty";
    GameObject attachedPowerCell;
    powerCell powerCell;
    public GameObject [] activates;

    void Start()
    {
        currentCore = "Empty";
        if(transform.childCount > 0f)
        {
            currentCore = transform.GetChild(0).gameObject.name;
            attachedPowerCell = transform.GetChild(0).gameObject;
            attachedPowerCell.GetComponent<Rigidbody2D>().isKinematic = true;
            powerCell = attachedPowerCell.GetComponent<powerCell>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(activated && canStoreCharge) // if the power station can store a charge and dispense it to any power cells put inside
        {
            if(attachedPowerCell != null)
            {
                if(!overcharge)
                {
                    attachedPowerCell.name = "chargedPowerCell";
                    powerCell.activated = true;
                }
                else
                {
                    attachedPowerCell.name = "overchargedPowerCell";
                    powerCell.activated = true;
                    powerCell.overcharge = true;
                }
            }
        }

        if(powerCell != null)
        {
            if(powerCell.activated)
            {
                StateChange(activated);
            }
        }
        else
        {
            holdingPowerCell = false;
            if(!canStoreCharge)
            {
                activated = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "powerCell" && !holdingPowerCell)
        {
            attachedPowerCell = col.gameObject;

            attachedPowerCell.GetComponent<Rigidbody2D>().isKinematic = true;
            attachedPowerCell.transform.position = transform.position;
            attachedPowerCell.transform.parent = gameObject.transform;
        }
    }

    void StateChange(bool active)
    {
        foreach (GameObject activateable in activates)
        {
            if(activateable != null)
            {
                activate activateScript = activateable.GetComponent<activate>();
                activateScript.Activate(powerCell.activated);
                if(powerCell.overcharge)
                {
                    activateScript.Overcharge(active); // if the button is active, overcharge the activatable dependent object
                }
                // don't need to send a signal to rotate right
            }
        }
    }
}
