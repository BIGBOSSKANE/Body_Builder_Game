/*
Creator: Daniel
Created: 12/04/2019
Laste Edited by: Daniel
Last Edit: 13/09/2019
*/


using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class gameManager : MonoBehaviour
{
    public static gameManager GameManager;
    public int currentLevel = -1;
    [Tooltip("Prevent checkpoint save between loads. Turn this off when building")] public bool preventSave = false;
    public float xPosition;
    public float yPosition;
    public int partConfiguration;
    public int headConfiguration;
    GameObject arms;
    public int armConfiguration;
    GameObject legs;
    public int legConfiguration;
    GameObject scalerAugment;
    GameObject hookshotAugment;

    GameObject player;
    GameObject storedSavedPlayer;
    playerScript playerScript;

    GameObject playerSpawner;
    playerSpawner playerSpawnerScript;

    void Awake()
    {
    // SINGLETON    
        if(GameManager == null)
        {
            GameManager = this;
        }
        else if(GameManager != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

    // FIND PLAYER
        if(GameObject.Find("Player") != null)
        {
            player = GameObject.Find("Player").gameObject;
            playerScript = player.GetComponent<playerScript>();
        }
    }

    public void CheckpointStartCheck() // Called on player start
    {
        if(GameObject.Find("Player").gameObject != null)
        {
            if(currentLevel != SceneManager.GetActiveScene().buildIndex) // first load into the level
            {
/*
                player = GameObject.Find("Player").gameObject;
                playerScript = player.GetComponent<playerScript>();
                xPosition = player.transform.position.x;
                yPosition = player.transform.position.y;
                partConfiguration = playerScript.partConfiguration;
                headConfiguration = playerScript.headConfiguration;
                armConfiguration = playerScript.armConfiguration;
                currentLevel = SceneManager.GetActiveScene().buildIndex;
*/
                // Destroy last level's player spawner, grab the new one, and make it a child of this object
                Destroy(playerSpawner);
                playerSpawner = GameObject.Find("PlayerSpawner").gameObject;
                playerSpawner.transform.parent = gameObject.transform;
                playerSpawnerScript = playerSpawner.GetComponent<playerSpawner>();

                xPosition = playerSpawner.transform.position.x;
                yPosition = playerSpawner.transform.position.y;
                partConfiguration = playerSpawnerScript.partConfiguration;
                headConfiguration = playerSpawnerScript.headConfiguration;
                armConfiguration = playerSpawnerScript.armConfiguration;
                legConfiguration = playerSpawnerScript.legConfiguration;
                currentLevel = SceneManager.GetActiveScene().buildIndex;
                
                Debug.Log("First Spawn");
            }

            playerSpawnerScript.OverrideVariables(new Vector2(xPosition , yPosition) , partConfiguration , headConfiguration , armConfiguration , legConfiguration);

/*
            player = GameObject.Find("Player").gameObject;

            if(partConfiguration != 1 && partConfiguration != 3) arms = player.transform.Find(playerScript.armString).gameObject;
            legConfiguration = playerScript.legConfiguration;
            if(legConfiguration != 1) legs = player.transform.Find(playerScript.legString).gameObject;
            playerScript = player.GetComponent<playerScript>();
*/
            playerScript.Respawn(new Vector2(xPosition , yPosition) , partConfiguration , headConfiguration , armConfiguration , legConfiguration);
        }
    }

    public void SetCheckpoint(Vector2 checkpointPos , int bodyParts , int headParts , int armsConfig , int legsConfig , GameObject playerPrefab) // set checkpoint when entering a checkpoint
    {
        player = playerPrefab;
        playerScript = player.GetComponent<playerScript>();

        xPosition = checkpointPos.x;
        yPosition = checkpointPos.y;
        partConfiguration = playerScript.partConfiguration;
        headConfiguration = playerScript.headConfiguration;
        armConfiguration = armsConfig;
        legConfiguration = legsConfig;
        /*
        if(partConfiguration != 1 && partConfiguration != 3) arms = player.transform.Find(playerScript.armString).gameObject;
        legConfiguration = legsConfig;
        if(partConfiguration != 1 && partConfiguration != 2) legs = player.transform.Find(playerScript.legString).gameObject;
        currentLevel = SceneManager.GetActiveScene().buildIndex;
        */
    }
}
