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
    /// SINGLETON //================================================================
        if(GameManager == null)
        {
            GameManager = this;
        }
        else if(GameManager != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

    /// GET PLAYER SPAWNER //=======================================================
        if(GameObject.Find("PlayerSpawner") != null)
        {
            playerSpawner = GameObject.Find("PlayerSpawner").gameObject;
            playerSpawnerScript = playerSpawner.GetComponent<playerSpawner>();
        }
    }

    public void Initialise()
    {
        if(GameObject.Find("PlayerSpawner").gameObject != null)
        {
            if(currentLevel != SceneManager.GetActiveScene().buildIndex)
            {
                playerSpawner = GameObject.Find("PlayerSpawner").gameObject;
                playerSpawnerScript = playerSpawner.GetComponent<playerSpawner>();
                playerSpawnerScript.preview = true;
                playerSpawnerScript.Preview();
                xPosition = playerSpawner.transform.position.x;
                yPosition = playerSpawner.transform.position.y;
                partConfiguration = playerSpawnerScript.partConfiguration;
                headConfiguration = playerSpawnerScript.headConfiguration;
                armConfiguration = playerSpawnerScript.armConfiguration;
                legConfiguration = playerSpawnerScript.legConfiguration;
                currentLevel = SceneManager.GetActiveScene().buildIndex;
            }
            
            playerSpawner = GameObject.Find("PlayerSpawner").gameObject;
            playerSpawnerScript = playerSpawner.GetComponent<playerSpawner>();
            playerSpawnerScript.OverrideSpawn(new Vector2(xPosition , yPosition) , partConfiguration , headConfiguration , armConfiguration , legConfiguration);
        }
    }

    public void SetCheckpoint(Vector2 checkpointPos , int bodyParts , int headParts , int armsConfig , int legsConfig) // set checkpoint when entering a checkpoint
    {
        xPosition = checkpointPos.x;
        yPosition = checkpointPos.y;
        partConfiguration = bodyParts;
        headConfiguration = headParts;
        armConfiguration = armsConfig;
        legConfiguration = legsConfig;
    }
}
