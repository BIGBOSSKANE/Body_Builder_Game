/*
Creator: Daniel
Created 24/08/2019
Last Edited by: Daniel
Last Edit 24/08/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkPoint : MonoBehaviour
{
    playerScript playerScript;

    void Start()
    {
        playerScript = GameObject.Find("Player").gameObject.GetComponent<playerScript>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            playerScript.SetSpawnPoint(new Vector2(transform.position.x , transform.position.y + 1f));
            Debug.Log("Checkpoint Reached");
        }
    }
}