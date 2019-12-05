using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trophy : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            AkSoundEngine.PostEvent("GetTrophy" , col.gameObject);
            Destroy(gameObject);
        }
    }
}