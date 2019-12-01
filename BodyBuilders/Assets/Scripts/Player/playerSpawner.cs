/*
Creator: Daniel
Created: 13/04/2019
Laste Edited by: Daniel
Last Edit: 13/09/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSpawner : MonoBehaviour
{
    public bool spaceForJump;
    public bool preview;

    [Header("Configuration:")]
    [Tooltip("1 is head, 2 is head and arms, 3 is head and legs, 4 is full body")] [Range (1 , 4)] public int partConfiguration = 1;
    int previousPartConfig = 1;
    [Tooltip("1 is basic head, 2 is scaler, 3 is hookshot, 4 is all augments")] [Range (1 , 4)] public int headConfiguration = 1;
    int previousHeadConfig = 1;
    [Tooltip("1 is basic, 2 is LifterArms, 3 is DeflectorArms")] [Range (1 , 3)] public int armConfiguration = 1;
    int previousArmConfig = 1;
    [Tooltip("1 is basic, 2 is groundbreakers, 3 is afterburners")] [Range (1 , 3)] public int legConfiguration = 1;
    int previousLegConfig = 1;

    
    [Header("Details (Not Editable:)")]
    public string bodyConfiguration = "Head";
    public string headAugment = "Basic Head";
    public string armType = "Basic Arms";
    public string legType = "Basic Legs";

    GameObject player;
    playerScript playerScript;

    Vector2 spawnPos;
    [HideInInspector] public Vector2 editorSpawnPos;
    float verticalOffset;
    
    int armIdentifier;
    int legIdentifier;
    int augmentScalerIdentifier;
    int augmentHookshotIdentifier;


    void Awake()
    {
        GameObject.Find("GameManager").gameObject.GetComponent<gameManager>().Initialise();
        spawnPos = editorSpawnPos + Vector2.up * verticalOffset;
    }

    public void OverrideSpawn(Vector2 spawnPosition, int parts , int head , int arms , int legs , int armIdenti , int legIdenti , int scalerIdenti , int hookshotIdenti) // called by the GameManagerScript
    {
        spawnPos = spawnPosition + Vector2.up * verticalOffset;
        partConfiguration = parts;
        headConfiguration = head;
        armConfiguration = arms;
        legConfiguration = legs;
        armIdentifier = armIdenti;
        legIdentifier = legIdenti;
        augmentScalerIdentifier = scalerIdenti;
        augmentHookshotIdentifier = hookshotIdenti;
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        spawnerPrefabHolder prefabHolder = gameObject.transform.Find("PrefabHolder").gameObject.GetComponent<spawnerPrefabHolder>();

        if(partConfiguration == 1)
        {
            player = Instantiate(prefabHolder.Head, spawnPos, Quaternion.identity);
            playerScript = player.GetComponent<playerScript>();
            player.name = "Player";
        }
        else if(partConfiguration == 2)
        {
            Arms Arms = prefabHolder.HeadAndArms.transform.Find("BasicArms").GetComponent<Arms>();
            if(armConfiguration == 1)
            {
                Arms.armType = Arms.armIdentifier.Basic;
            }
            else if(armConfiguration == 2)
            {
                Arms.armType = Arms.armIdentifier.Lifter;
            }
            else if(armConfiguration == 3)
            {
                Arms.armType = Arms.armIdentifier.Shield;
            }
            
            Arms.instance = armIdentifier;
            player = Instantiate(prefabHolder.HeadAndArms, spawnPos, Quaternion.identity);
            playerScript = player.GetComponent<playerScript>();
            player.name = "Player";
        }
        else if(partConfiguration == 3)
        {
            Legs Legs = prefabHolder.HeadAndLegs.transform.Find("BasicLegs").GetComponent<Legs>();
            if(legConfiguration == 1)
            {
                Legs.legType = Legs.legIdentifier.Basic;
            }
            else if(legConfiguration == 2)
            {
                Legs.legType = Legs.legIdentifier.Groundbreaker;
            }
            else if(legConfiguration == 3)
            {
                Legs.legType = Legs.legIdentifier.Afterburner;
            }

            Legs.instance = legIdentifier;
            player = Instantiate(prefabHolder.HeadAndLegs, spawnPos, Quaternion.identity);
            playerScript = player.GetComponent<playerScript>();
            player.name = "Player";
        }
        else if(partConfiguration == 4)
        {
            Arms Arms = prefabHolder.Fullbody.transform.Find("BasicArms").GetComponent<Arms>();
            if(armConfiguration == 1)
            {
                Arms.armType = Arms.armIdentifier.Basic;
            }
            else if(armConfiguration == 2)
            {
                Arms.armType = Arms.armIdentifier.Lifter;
            }
            else if(armConfiguration == 3)
            {
                Arms.armType = Arms.armIdentifier.Shield;
            }

            Legs Legs = prefabHolder.Fullbody.transform.Find("BasicLegs").GetComponent<Legs>();
            if(legConfiguration == 1)
            {
                Legs.legType = Legs.legIdentifier.Basic;
            }
            else if(legConfiguration == 2)
            {
                Legs.legType = Legs.legIdentifier.Groundbreaker;
            }
            else if(legConfiguration == 3)
            {
                Legs.legType = Legs.legIdentifier.Afterburner;
            }

            Arms.instance = armIdentifier;
            Legs.instance = legIdentifier;
            player = Instantiate(prefabHolder.Fullbody, spawnPos, Quaternion.identity);
            playerScript = player.GetComponent<playerScript>();
            player.name = "Player";
        }

        if(headConfiguration == 1) // basic
        {
            playerScript.scaler = false;
            playerScript.hookShot = false;
            playerScript.augmentScalerIdentifier = 0;
            playerScript.augmentHookshotIdentifier = 0;
        }
        else if(headConfiguration == 2) // scaler
        {
            playerScript.scaler = true;
            playerScript.hookShot = false;
            playerScript.augmentScalerIdentifier = augmentScalerIdentifier;
            playerScript.augmentHookshotIdentifier = 0;
        }
        else if(headConfiguration == 3) // hookshot
        {
            playerScript.scaler = false;
            playerScript.hookShot = true;
            playerScript.augmentScalerIdentifier = 0;
            playerScript.augmentHookshotIdentifier = augmentHookshotIdentifier;
        }
        else if(headConfiguration == 4) // all augments
        {
            playerScript.scaler = true;
            playerScript.hookShot = true;
            playerScript.augmentScalerIdentifier = augmentScalerIdentifier;
            playerScript.augmentHookshotIdentifier = augmentHookshotIdentifier;
        }
        
        playerScript.spaceForJump = spaceForJump;

    }

    void Update()
    {
        playerScript.UpdateParts(); // get player to update parts, recognising the new ones
        gameObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Preview(); // always make sure the placement is correct
        if(preview) // preview part configuration
        {
            if(partConfiguration != previousPartConfig)
            {
                if(partConfiguration == 1)
                {
                    bodyConfiguration = "Head";
                    gameObject.transform.Find("HeadIndicator").gameObject.transform.position = editorSpawnPos + Vector2.up * verticalOffset;
                }
                else if(partConfiguration == 2)
                {
                    bodyConfiguration = "Head and Torso";
                    gameObject.transform.Find("HeadIndicator").gameObject.transform.position = new Vector2(editorSpawnPos.x , editorSpawnPos.y + 0.5500002f + verticalOffset);
                    gameObject.transform.Find("ArmIndicator").gameObject.transform.position = new Vector2(editorSpawnPos.x , editorSpawnPos.y - 0.2550001f + verticalOffset);
                    gameObject.transform.Find("LegIndicator").gameObject.transform.position = new Vector2(editorSpawnPos.x , editorSpawnPos.y + verticalOffset);
                }
                else if(partConfiguration == 3)
                {
                    bodyConfiguration = "Head and Legs";
                    gameObject.transform.Find("HeadIndicator").gameObject.transform.position = new Vector2(editorSpawnPos.x , editorSpawnPos.y + 0.1550002f + verticalOffset);
                    gameObject.transform.Find("ArmIndicator").gameObject.transform.position = new Vector2(editorSpawnPos.x , editorSpawnPos.y + verticalOffset);
                    gameObject.transform.Find("LegIndicator").gameObject.transform.position = new Vector2(editorSpawnPos.x , editorSpawnPos.y - 0.01399994f + verticalOffset);
                }
                else if(partConfiguration == 4)
                {
                    bodyConfiguration = "Full Body";
                    gameObject.transform.Find("HeadIndicator").gameObject.transform.position = new Vector2(editorSpawnPos.x , editorSpawnPos.y + 0.7550001f + verticalOffset);
                    gameObject.transform.Find("ArmIndicator").gameObject.transform.position = new Vector2(editorSpawnPos.x , editorSpawnPos.y + verticalOffset);
                    gameObject.transform.Find("LegIndicator").gameObject.transform.position = new Vector2(editorSpawnPos.x , editorSpawnPos.y - 0.01399994f + verticalOffset);
                }

                gameObject.transform.Find("LegIndicator").gameObject.GetComponent<spawnerSprites>().SetSprite(legConfiguration , partConfiguration);
                gameObject.transform.Find("ArmIndicator").gameObject.GetComponent<spawnerSprites>().SetSprite(armConfiguration , partConfiguration);
                previousPartConfig = partConfiguration;
            }

            if(headConfiguration != previousHeadConfig)
            {
                if(headConfiguration == 1)
                {
                    headAugment = "Basic Head";
                }
                else if(headConfiguration == 2)
                {
                    headAugment = "Scaler Head";
                }
                else if(headConfiguration == 3)
                {
                    headAugment = "Hookshot Head";
                }
                else if(headConfiguration == 4)
                {
                    headAugment = "All Augments";
                }
                gameObject.transform.Find("HeadIndicator").gameObject.GetComponent<spawnerHeadSprite>().SetSprite(headConfiguration);
                previousHeadConfig = headConfiguration;
            }

            if(legConfiguration != previousLegConfig)
            {
                if(legConfiguration == 1)
                {
                    legType = "Basic Legs";
                }
                else if(legConfiguration == 2)
                {
                    legType = "Groundbreaker Legs";
                }
                else if(legConfiguration == 3)
                {
                    legType = "Afterburner Legs";
                }
                gameObject.transform.Find("LegIndicator").gameObject.GetComponent<spawnerSprites>().SetSprite(legConfiguration , partConfiguration);
                previousLegConfig = legConfiguration;
            }

            if(armConfiguration != previousArmConfig)
            {
                if(armConfiguration == 1)
                {
                    armType = "Basic Arms";
                }
                else if(armConfiguration == 2)
                {
                    armType = "Lifter Arms";
                }
                else if(armConfiguration == 3)
                {
                    armType = "Shield Arms";
                }
                gameObject.transform.Find("ArmIndicator").gameObject.GetComponent<spawnerSprites>().SetSprite(armConfiguration , partConfiguration);
                previousArmConfig = armConfiguration;
            }
        }
    }

    public void Preview()
    {
        if(partConfiguration == 1)
        {
            editorSpawnPos = (Vector2)gameObject.transform.position;
            verticalOffset = 0.27f;
        }
        else if(partConfiguration == 2)
        {
            editorSpawnPos = (Vector2)gameObject.transform.position + Vector2.up;
            verticalOffset = -0.198f;
        }
        else if(partConfiguration == 3)
        {
            editorSpawnPos = (Vector2)gameObject.transform.position + Vector2.up;
            verticalOffset = -0.01f;
        }
        else if(partConfiguration == 4)
        {
            editorSpawnPos = (Vector2)gameObject.transform.position + Vector2.up;
            verticalOffset = -0.042f;
        }
    }
}
