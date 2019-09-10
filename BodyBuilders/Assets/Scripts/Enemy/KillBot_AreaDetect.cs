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
    }

        void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("Player entered area");
            nKB.chasePlayer = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("Player entered area");
            nKB.chasePlayer = false;
        }
    }
}