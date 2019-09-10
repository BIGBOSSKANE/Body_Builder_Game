/*
Creator: Daniel
Created 02/09/2019
Last Edited by: Daniel
Last Edit 02/09/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraWaypoint : MonoBehaviour
{
    [Tooltip("Destroys itself after first cycle. Otherwise it retriggers whenever the player re-enters")] public bool onlyOnce = true;
    [Tooltip("Locks player view to first location and size, until player leaves the area")] public bool lockView = true;
    bool alreadyDone = false;
    [Tooltip("Locks the player while looping between waypoints")] public bool playerLock; // does the waypoint lock the player (don't use if the camera is supposed to zoom out and stay locked on an area)
    [Tooltip("Unlocks the player if they apply input after a short duration while moving between waypoints")] public bool unlockOnInput = false; // does the waypoint disable when the player moves vertically or horizontally
    [Tooltip("Duration taken to resize between waypoints")] public float waypointResizeDuration = 2f; // time for camera zoom out
    int waypointCount; // number of waypoints
    int currentWaypoint = 0; // current waypoint
    Camera2DFollow cameraScript;
    Vector2 previousPosition; // used for drawing lines between locations
    Vector2 playerPos;

    [SerializeField] private SubClass[] waypointCycle;

    public SubClass GetValue (int index)
    {

        return waypointCycle[index];
    }

    void OnDrawGizmos() // show all of the camera waypoints
    {
        Gizmos.color = Color.yellow;
        float locationIdentifier = 0.3f;

        for(int i = 0 ; i < waypointCycle.Length ; i++)
        {
            Gizmos.DrawLine(waypointCycle[i].waypointPos - Vector2.up * locationIdentifier , waypointCycle[i].waypointPos + Vector2.up * locationIdentifier);
            Gizmos.DrawLine(waypointCycle[i].waypointPos - Vector2.left * locationIdentifier , waypointCycle[i].waypointPos + Vector2.left * locationIdentifier);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(waypointCycle[i].waypointPos , previousPosition);
            if(i == 0)
            {
                if(alreadyDone)
                {
                    previousPosition = waypointCycle[waypointCount + 1].waypointPos;
                }
                else
                {
                    previousPosition = playerPos;
                }
            }
            else
            {
                previousPosition = waypointCycle[i].waypointPos;
            }
        }
    }

    void Start()
    {
        currentWaypoint = 0;
        waypointCount = waypointCycle.Length;
        cameraScript = Camera.main.GetComponent<Camera2DFollow>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player" && ((onlyOnce && !alreadyDone) || !onlyOnce))
        {
            cameraScript.unlockOnInput = unlockOnInput;
            cameraScript.wayPointResizeDuration = waypointResizeDuration;
            playerPos = col.gameObject.transform.position;
            NextCoordinate();
        }
    }

    public void NextCoordinate()
    {
        if(currentWaypoint < waypointCount)
        {
            if(currentWaypoint == 0)
            {
                previousPosition = playerPos;
            }
            else
            {
                previousPosition = waypointCycle[currentWaypoint - 1].waypointPos;
            }
            cameraScript.waypointCycling = true;
            cameraScript.WayPointCycle(waypointCycle[currentWaypoint].waypointPos , previousPosition , waypointCycle[currentWaypoint].waypointPauseTime , waypointCycle[currentWaypoint].waypointSize , playerLock , lockView ,unlockOnInput, waypointCycle[currentWaypoint].waypointMoveTime , this , waypointCount);
            currentWaypoint ++;
        }
        else
        {
            cameraScript.waypointCycling = false;
            cameraScript.EndCycle();
            //cameraScript.WayPointCycle(playerPos , previousPosition , 0f , 0f , false , 0f , this);
            currentWaypoint = 0;
        }
    }

    [System.Serializable]
    public class SubClass
    {
        public Vector2 waypointPos;
        public float waypointPauseTime;
        public float waypointSize;
        public float waypointMoveTime = 1f; // time for camera movement
    }
}