﻿//Created by Kane Girvan
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeBotDetectLeft : MonoBehaviour
{
    public GameObject bladeBot;
    newBladeBot nBB;

    void Start()
    {
        nBB = bladeBot.gameObject.GetComponent<newBladeBot>(); // we can swap this out for the scene manager once it has been added
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            nBB.ChargeL();
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            nBB.ChargeR();
        }
    }
}