﻿//Created by Kane Girvan
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBot_AreaDetect : MonoBehaviour
{
    public GameObject kB;
    KillBot nKB;

    void Start()
    {
        nKB = kB.GetComponent<KillBot>();
        kB = gameObject.transform.parent.Find("Killbot").gameObject;
    }

        void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            nKB.chasePlayer = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            nKB.chasePlayer = false;
        }
    }
}