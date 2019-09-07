/*
Creator: Daniel
Created 29/08/2019
Last Edited by: Daniel
Last Edit 29/08/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door : activate
{
    [Tooltip("How long does the door take to move?")] public float moveTimeTotal = 1f;
    Vector2 originalPosition;
    [Tooltip("Move to be centred at this position")] public Vector2 moveTo;
    Vector2 targetPosition;
    float moveTime;
    bool previousState;
    float xBounds;
    float yBounds;

    void Start()
    {
        originalPosition = gameObject.transform.position;
        moveTime = 0f;
        targetPosition = moveTo + (Vector2)transform.position;
    }

    void Update()
    {
        if(activated)
        {
            moveTime += Time.deltaTime;
            if(moveTime > moveTimeTotal)
            {
                moveTime = moveTimeTotal;
            }
            gameObject.transform.position = Vector2.Lerp(originalPosition , targetPosition , moveTime/moveTimeTotal);

        }
        else if(!activated)
        {
            moveTime -= Time.deltaTime;
            if(moveTime < 0f)
            {
                moveTime = 0f;
            }
            gameObject.transform.position = Vector2.Lerp(originalPosition , targetPosition , moveTime/moveTimeTotal);
        }

        if(activated != previousState) // play sound effect on a state change
        {
            AkSoundEngine.PostEvent("DoorMoves" , gameObject);
        }
        previousState = activated;
    }

    void OnDrawGizmos() // shows the waypoints in both editor and in game
    {
        Gizmos.color = Color.red;
        float locationIdentifier = 0.3f;
        
        // if in game, the waypoints don't move, so use the world positions

        if(Application.isPlaying)
        {
            Gizmos.DrawLine(targetPosition - Vector2.up * locationIdentifier , targetPosition + Vector2.up * locationIdentifier);
            Gizmos.DrawLine(targetPosition - Vector2.left * locationIdentifier , targetPosition + Vector2.left * locationIdentifier);
        }
        else
        {
            targetPosition = moveTo + (Vector2)transform.position;
            Gizmos.DrawLine(targetPosition - Vector2.up * locationIdentifier , targetPosition + Vector2.up * locationIdentifier);
            Gizmos.DrawLine(targetPosition - Vector2.left * locationIdentifier , targetPosition + Vector2.left * locationIdentifier);
        }
    }
}