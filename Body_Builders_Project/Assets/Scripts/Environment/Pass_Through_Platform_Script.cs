/*
Creator: Daniel
Created: 09/04/2019
Laste Edited by: Daniel
Last Edit: 24/05/2019
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pass_Through_Platform_Script : MonoBehaviour
{
    public PlatformEffector2D effector;
    float waitTime;
    bool playerAbove;

    void Start()
    {
        effector = gameObject.GetComponent<PlatformEffector2D>();
        waitTime = 0.1f;
        playerAbove = false;
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow) || playerAbove == false)
        {
            waitTime = 0.1f;
        }

        if(playerAbove == true && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)))
        {
            if(waitTime <= 0f)
            {
                effector.rotationalOffset = 180f;
                waitTime = 0.1f;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }        
        }

        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
              effector.rotationalOffset = 0f;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
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
