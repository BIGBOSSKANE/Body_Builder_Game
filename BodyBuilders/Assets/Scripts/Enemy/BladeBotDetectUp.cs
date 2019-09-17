using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeBotDetectUp : MonoBehaviour
{
    newBladeBot nBB;

    void Start()
    {
        nBB = GameObject.Find("Bladebot").gameObject.GetComponent<newBladeBot>(); // we can swap this out for the scene manager once it has been added

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            nBB.ChargeU();
        }
    }
}