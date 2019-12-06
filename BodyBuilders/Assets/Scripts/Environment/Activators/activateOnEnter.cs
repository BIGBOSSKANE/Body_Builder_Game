/*
Creator: Daniel
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activateOnEnter : MonoBehaviour
{
    [Tooltip("Overcharge activatable dependents")] public bool overcharge = false;
    [Tooltip("What game objects does this activate, increase the size to add more")] public GameObject [] activates;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            foreach (GameObject activateable in activates)
            {
                if(activateable != null)
                {
                    activate activateScript = activateable.GetComponent<activate>();
                    activateScript.Activate(true);
                    if(overcharge)
                    {
                        activateScript.Overcharge(overcharge); // if the power station is overcharged, overcharge the activatable dependent object
                    }
                }
            }
            Destroy(gameObject);
        }
    }
}
