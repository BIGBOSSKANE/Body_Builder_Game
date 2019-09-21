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
    public int armConfiguration;
    public int legConfiguration;
    GameObject scalerAugment;
    GameObject hookshotAugment;

    GameObject player;
    GameObject storedSavedPlayer;
    playerScript playerScript;

    GameObject playerSpawner;
    playerSpawner playerSpawnerScript;
    
    private GameObject[] arms;
    int armIdentifier;
    private GameObject[] legs;
    int legIdentifier;
    private GameObject[] augments;
    public int augmentScalerIdentifier; // scaler
    int augmentHookshotIdentifier; // hookshot

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
        if(GameObject.Find("PlayerSpawner").gameObject != null) /// CULL PARTS AND AUGMENTS ON THE MAP THAT THE PLAYER CURRENTLY HAS EQUIPPED //
        {
            if(currentLevel == SceneManager.GetActiveScene().buildIndex)
            {
                if(partConfiguration != 1 & partConfiguration != 3)
                {
                    GameObject[] arms = GameObject.FindGameObjectsWithTag("Arms");
                    if(arms.Length != 0)
                    {
                        for(int i = 0; i < arms.Length; i++)
                        {
                            if(arms[i].name != "playerCollider" && arms[i].GetComponent<Arms>().instance == armIdentifier)
                            {
                                Destroy(arms[i]);
                                break;
                            }
                        }
                    }
                }

                if(partConfiguration >= 3)
                {
                    GameObject[] legs = GameObject.FindGameObjectsWithTag("Legs");
                    if(legs.Length != 0)
                    {
                        for(int i = 0; i < legs.Length; i++)
                        {
                            if(legs[i].name != "playerCollider" && legs[i].GetComponent<Legs>().instance == legIdentifier)
                            {
                                Destroy(legs[i]);
                                break;
                            }
                        }
                    }
                }

                GameObject[] augments = GameObject.FindGameObjectsWithTag("HeadAugment");
                if(augments.Length != 0)
                {
                    for(int i = 0; i < augments.Length; i++)
                    {
                        int foundIdentifier = augments[i].GetComponent<augmentPickup>().instance;
                        if((foundIdentifier == augmentScalerIdentifier || foundIdentifier == augmentHookshotIdentifier))
                        {
                            Destroy(augments[i]);
                        }
                    }
                }
            }

            if(currentLevel != SceneManager.GetActiveScene().buildIndex)
            {
                playerSpawner = GameObject.Find("PlayerSpawner").gameObject;
                playerSpawnerScript = playerSpawner.GetComponent<playerSpawner>();
                playerSpawnerScript.preview = true;
                playerSpawnerScript.Preview();
                xPosition = playerSpawnerScript.editorSpawnPos.x;
                yPosition = playerSpawnerScript.editorSpawnPos.y;
                partConfiguration = playerSpawnerScript.partConfiguration;
                headConfiguration = playerSpawnerScript.headConfiguration;
                armConfiguration = playerSpawnerScript.armConfiguration;
                legConfiguration = playerSpawnerScript.legConfiguration;
                armIdentifier = 0;
                legIdentifier = 0;
                augmentScalerIdentifier = 0;
                augmentHookshotIdentifier = 0;
                currentLevel = SceneManager.GetActiveScene().buildIndex;
            }

            Vector2 spawnPos = new Vector2(xPosition , yPosition);
            playerSpawner = GameObject.Find("PlayerSpawner").gameObject;
            playerSpawnerScript = playerSpawner.GetComponent<playerSpawner>();
            playerSpawnerScript.OverrideSpawn(spawnPos , partConfiguration , headConfiguration , armConfiguration , legConfiguration,
                                              armIdentifier , legIdentifier , augmentScalerIdentifier , augmentHookshotIdentifier);
        }
    }

    public void SetCheckpoint(Vector2 checkpointPos , int bodyParts , int headParts , int armsConfig , int legsConfig , int armsIdenti , int legsIdenti , int augmentScalerIdenti , int augmentHookshotIdenti) // set checkpoint when entering a checkpoint
    {
        xPosition = checkpointPos.x;
        yPosition = checkpointPos.y;
        partConfiguration = bodyParts;
        headConfiguration = headParts;
        armConfiguration = armsConfig;
        legConfiguration = legsConfig;
        armIdentifier = armsIdenti;
        legIdentifier = legsIdenti;
        augmentScalerIdentifier = augmentScalerIdenti;
        augmentHookshotIdentifier = augmentHookshotIdenti;
    }
}
