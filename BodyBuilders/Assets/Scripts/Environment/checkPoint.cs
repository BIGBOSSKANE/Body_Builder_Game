/*
Creator: Daniel
Created 24/08/2019
Last Edited by: Daniel
Last Edit 24/08/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class checkPoint : MonoBehaviour
{
    playerScript playerScript;
    checkpointData checkpointData;
    [Tooltip("Respawn with specific parts")] public bool configurationOverride = false;
    [Tooltip("Player respawn body configuration")] [Range (1 , 4)] public int partConfiguration = 1;
    [Tooltip("Player respawn head configuration")] [Range (1 , 4)] public int headConfiguration = 1;
    [Tooltip("Type of Arms (if any)")] public string armString = "BasicArms";
    [Tooltip("Type of Legs (if any)")] public string legString = "BasicLegs";


    void Start()
    {
        playerScript = GameObject.Find("Player").gameObject.GetComponent<playerScript>();
        checkpointData = GameObject.Find("Player").gameObject.GetComponent<checkpointData>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            if(!playerScript.dying)
            {
                if(!configurationOverride)
                {
                    partConfiguration = playerScript.partConfiguration;
                    headConfiguration = playerScript.headConfiguration;
                    armString = playerScript.armString;
                    legString = playerScript.legString;
                }
                checkpointData.SetCheckpoint(new Vector2(transform.position.x , transform.position.y + 1f) , partConfiguration , headConfiguration , armString , legString);
                Debug.Log("Checkpoint Reached");
            }
        }
    }
}