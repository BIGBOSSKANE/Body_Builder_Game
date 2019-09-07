using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeBot_AreaDetect : MonoBehaviour
{
    public GameObject bB;
    NewBladeBot nBB;

    void Start()
    {
        nBB = bB.GetComponent<NewBladeBot>();
    }

        void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("Player entered area");
            nBB.chasePlayer = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("Player entered area");
            nBB.chasePlayer = false;
        }
    }
}