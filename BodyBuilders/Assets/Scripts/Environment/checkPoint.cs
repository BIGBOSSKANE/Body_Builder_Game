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
    GameObject player;
    playerScript playerScript;
    checkpointData checkpointData;
    [Tooltip("Respawn with specific parts")] public bool configurationOverride = false;
    [Tooltip("Player respawn body configuration")] [Range (1 , 4)] public int partConfiguration = 1;
    [Tooltip("Player respawn head configuration")] [Range (1 , 4)] public int headConfiguration = 1;
    [Tooltip("Player respawn arms")] [Range (1 , 3)] public int armConfiguration = 1;
    [Tooltip("Player respawn legs")] [Range (1 , 3)] public int legConfiguration = 1;
    gameManager gameManager;
    BoxCollider2D boxCollider;
    public int armIdentifier;
    public int legIdentifier;
    public int augmentScalerIdentifier;
    public int augmentHookshotIdentifier;


    void Start()
    {
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;
        playerScript = GameObject.Find("Player").gameObject.GetComponent<playerScript>();
        checkpointData = GameObject.Find("Player").gameObject.GetComponent<checkpointData>();
        gameManager = GameObject.Find("GameManager").GetComponent<gameManager>();
    }

    void Update()
    {
        boxCollider.enabled = true;
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
                    armConfiguration = playerScript.armConfiguration;
                    legConfiguration = playerScript.legConfiguration;
                    armIdentifier = playerScript.armIdentifier;
                    legIdentifier = playerScript.legIdentifier;
                    augmentScalerIdentifier = playerScript.augmentScalerIdentifier;
                    augmentHookshotIdentifier = playerScript.augmentHookshotIdentifier;
                }
                gameManager.SetCheckpoint(new Vector2(transform.position.x , transform.position.y + 1f) , partConfiguration , headConfiguration , armConfiguration , legConfiguration,
                                          armIdentifier , legIdentifier , augmentScalerIdentifier , augmentHookshotIdentifier);
            }
        }
    }
}