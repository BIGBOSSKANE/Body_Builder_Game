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
    float waitTime = 0.1f;
    float waitTimer;
    float resetTime = 0.5f;
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
        if(playerScript.lockController) return;
        
        if(playerScript.isSwinging) gameObject.layer = 13;
        else gameObject.layer = 14;

        if(Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow) || playerAbove == false)
        {
            waitTimer = waitTime;
        }

        if(playerAbove == true && InputManager.JoystickLeftVerticalUnclamped() <= -0.85f)
        {
            if(waitTimer <= 0f)
            {
                effector.rotationalOffset = 180f;
                //AkSoundEngine.PostEvent("PassThroughPlatform" , gameObject);
                waitTimer = waitTime;
                resetTimer = resetTime;
            }
            else
            {
                waitTimer -= Time.deltaTime;
            }
        }

        if(!playerAbove || resetTimer <= 0f)
        {
            effector.rotationalOffset = 0f;
            resetTimer = resetTime;
            Debug.Log("Did it");
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
