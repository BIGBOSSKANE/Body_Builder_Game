using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSpawner : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("1 is head, 2 is head and arms, 3 is head and legs, 4 is full body")] [Range (1 , 4)] public int partConfiguration = 1;
    int previousPartConfig = 1;
    [Tooltip("1 is basic head, 2 is scaler, 3 is hookshot, 4 is all augments")] [Range (1 , 4)] public int headConfiguration = 1;
    int previousHeadConfig = 1;
    [Tooltip("1 is basic, 2 is LifterArms, 3 is DeflectorArms")] [Range (1 , 3)] public int armConfiguration = 1;
    int previousArmConfig = 1;
    [Tooltip("1 is basic, 2 is groundbreakers, 3 is afterburners")] [Range (1 , 3)] public int legConfiguration = 1;
    int previousLegConfig = 1;

    
    [Header("Details (Not Editable)")]
    public string bodyConfiguration = "Head";
    public string headAugment = "Basic Head";
    public string armType = "Basic Arms";
    public string legType = "Basic Legs";


    [Header("Prefabs:")]
    public GameObject Head;
    public GameObject HeadAndArms;
    public GameObject HeadAndLegs;
    public GameObject Fullbody;

    GameObject player;
    playerScript playerScript;

    [HideInInspector] public Vector2 spawnPos;


    void Awake()
    {
        spawnPos = transform.position;
        SpawnPlayer();
    }

    public void OverrideVariables(Vector2 spawnPosition, int parts , int head , int arms , int legs) // called by the GameManagerScript
    {
        spawnPos = spawnPosition;
        partConfiguration = parts;
        headConfiguration = head;
        armConfiguration = arms;
        legConfiguration = legs;
    }

    void SpawnPlayer()
    {
        if(partConfiguration == 1)
        {
            player = Instantiate(Head, transform.position, Quaternion.identity);
            playerScript = player.GetComponent<playerScript>();
            player.name = "Player";
        }
        else if(partConfiguration == 2)
        {
            Arms Arms = HeadAndArms.transform.Find("BasicArms").GetComponent<Arms>();
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
            player = Instantiate(HeadAndArms, transform.position, Quaternion.identity);
            playerScript = player.GetComponent<playerScript>();
            player.name = "Player";
        }
        else if(partConfiguration == 3)
        {
            Legs Legs = HeadAndLegs.transform.Find("BasicLegs").GetComponent<Legs>();
            if(armConfiguration == 1)
            {
                Legs.legType = Legs.legIdentifier.Basic;
            }
            else if(armConfiguration == 2)
            {
                Legs.legType = Legs.legIdentifier.Groundbreaker;
            }
            else if(armConfiguration == 3)
            {
                Legs.legType = Legs.legIdentifier.Afterburner;
            }

            player = Instantiate(HeadAndLegs, transform.position, Quaternion.identity);
            playerScript = player.GetComponent<playerScript>();
            player.name = "Player";
        }
        else if(partConfiguration == 4)
        {
            Arms Arms = Fullbody.transform.Find("BasicArms").GetComponent<Arms>();
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

            Legs Legs = Fullbody.transform.Find("BasicLegs").GetComponent<Legs>();
            if(armConfiguration == 1)
            {
                Legs.legType = Legs.legIdentifier.Basic;
            }
            else if(armConfiguration == 2)
            {
                Legs.legType = Legs.legIdentifier.Groundbreaker;
            }
            else if(armConfiguration == 3)
            {
                Legs.legType = Legs.legIdentifier.Afterburner;
            }

            player = Instantiate(Fullbody, transform.position, Quaternion.identity);
            playerScript = player.GetComponent<playerScript>();
            player.name = "Player";
        }

        if(headConfiguration == 1) // basic
        {
            playerScript.scaler = false;
            playerScript.hookShot = false;
        }
        else if(headConfiguration == 2) // scaler
        {
            playerScript.scaler = true;
            playerScript.hookShot = false;
        }
        else if(headConfiguration == 3) // hookshot
        {
            playerScript.scaler = false;
            playerScript.hookShot = true;
        }
        else if(headConfiguration == 4) // all augments
        {
            playerScript.scaler = true;
            playerScript.hookShot = true;
        }

        // Reference a function in all scripts that reference the player and player script, and call for them to reassign that variable
    }

    void Update()
    {
        playerScript.UpdateParts(); // get player to update parts, recognising the new ones
        gameObject.SetActive(false);
    }


    void OnDrawGizmos()
    {
        if(partConfiguration != previousPartConfig)
        {
            if(partConfiguration == 1)
            {
                bodyConfiguration = "Head";
            }
            else if(partConfiguration == 2)
            {
                bodyConfiguration = "Head and Torso";
            }
            else if(partConfiguration == 3)
            {
                bodyConfiguration = "Head and Legs";
            }
            else if(partConfiguration == 4)
            {
                bodyConfiguration = "Full Body";
            }
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
            previousArmConfig = armConfiguration;
        }
    }
}
