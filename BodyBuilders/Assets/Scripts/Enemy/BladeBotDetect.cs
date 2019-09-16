using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeBotDetect : MonoBehaviour
{
    newBladeBot nBB;
    public bool playerDetect = false;


    // Start is called before the first frame update
    void Start()
    {
        nBB = GameObject.Find("Bladebot").gameObject.GetComponent<newBladeBot>(); // we can swap this out for the scene manager once it has been added
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
            playerDetect = true;
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
            playerDetect = false;
    }
}