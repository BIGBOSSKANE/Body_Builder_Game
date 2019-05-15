using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pressure_Pad_BB : MonoBehaviour
{
    public GameObject movingPlatform;
    private animController aniCont;

    // Update is called once 
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log ("collided with button" );
            movingPlatform.GetComponent<animController>().enabled = true;
        }
    }
}