/*
Creator: Daniel
Created: 09/04/2019
Laste Edited by: Daniel
Last Edit: 24/05/2019
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class passThroughPlatform : MonoBehaviour
{
    PlatformEffector2D effector;
    float waitTimer;
    float resetTimer;
    bool playerAbove;
    bool shiftHeld = false;
    playerScript playerScript;

    void Start()
    {
        playerScript = GameObject.Find("Player").gameObject.GetComponent<playerScript>();
        effector = gameObject.GetComponent<PlatformEffector2D>();
        waitTimer = 0.1f;
        resetTimer = 0.1f;
        playerAbove = false;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            shiftHeld = true;
        }

        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            shiftHeld = false;
        }

        if(playerScript.isSwinging == true)
        {
            gameObject.layer = 13;
        }
        else
        {
            gameObject.layer = 14;
        }


        if(!shiftHeld)
        {
            if(Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow) || playerAbove == false)
            {
                waitTimer = 0.1f;
            }

            if(playerAbove == true && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)))
            {
                if(waitTimer <= 0f)
                {
                    effector.rotationalOffset = 180f;
                    AkSoundEngine.PostEvent("PassThroughPlatform" , gameObject);
                    waitTimer = 0.1f;
                    resetTimer = 0.4f;
                }
                else
                {
                    waitTimer -= Time.deltaTime;
                }        
            }

            if(!playerAbove || resetTimer <= 0f)
            {
                effector.rotationalOffset = 0f;
                resetTimer = 0.6f;
            }
            else
            {
                resetTimer -= Time.deltaTime;
            }

            if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                effector.rotationalOffset = 0f;
            }   
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            playerAbove = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            playerAbove = false;
        }
    }
}
