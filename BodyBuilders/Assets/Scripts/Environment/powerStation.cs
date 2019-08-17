using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerStation : MonoBehaviour
{
    public bool key = false; // does placing a charged cell in this power a door
    public bool unlocked = false; // has a power cell been placed in the power station, activating it
    public bool energised = false;
    public string currentCore = "Empty";
    GameObject attachedPowerCell;
    powerCell powerCell;

    void Start()
    {
        currentCore = "Empty";
        if(transform.childCount > 0f)
        {
            currentCore = transform.GetChild(0).gameObject.name;
            attachedPowerCell = transform.GetChild(0).gameObject;
            attachedPowerCell.GetComponent<Rigidbody2D>().isKinematic = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(energised)
        {
            energised = true;
            if(attachedPowerCell != null)
            {
                attachedPowerCell.name = "chargedPowerCell";
                powerCell = attachedPowerCell.GetComponent<powerCell>();
            }
        }

        if(attachedPowerCell != null && attachedPowerCell.transform.parent == gameObject.transform)
        {
            attachedPowerCell.transform.position = transform.position;
        }

        if(key && powerCell.charged)
        {
            unlocked = true;
        }
        else
        {
            unlocked = false;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "powerCell")
        {
            attachedPowerCell = col.gameObject;
            if(transform.childCount == 0f)
            {
                attachedPowerCell.GetComponent<Rigidbody2D>().isKinematic = true;
                attachedPowerCell.transform.position = transform.position;
                attachedPowerCell.transform.parent = gameObject.transform;
            }
        }
    }
}
