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
    PlatformEffector2D effector;
    BoxCollider2D boxCol;
    float waitTime;
    float colWaitTime;

    void Start()
    {
        effector = gameObject.GetComponent<PlatformEffector2D>();
        boxCol = gameObject.GetComponent<BoxCollider2D>();
        waitTime = 0.1f;
        colWaitTime = 0.1f;
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            waitTime = 0.1f;
        }

        if(Input.GetKey(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
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
}
