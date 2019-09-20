using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevator3 : activate
{
    [Tooltip("How long does the elevator take to move normally?")] [Range (0.1f , 20f)] public float moveTime = 5f; // how long between positions?
    [Tooltip("How long does the elevator take to slam upwards?")] [Range (0.1f , 10f)] public float slamUpTime = 0.8f; // how long does the elevator take to slam upwards?
    // public bool overcharge = false; // will the elevator slam upwards?         ***Make sure the elevator is completely vertical for this to work***
    [Tooltip("How much launch force does the elevator provide at the apex of an upwards slam?")] [Range (0f , 10f)] public float launchForce = 3f; // force applied at the movement apex when slamming up
    [Tooltip("How much futher is the player launched if they jump at the apex of the slam?")] [Range (0f , 10f)] public float jumpLaunchForce = 6f; // if the player jumps at the apex of a slam, they get launched further
    [Tooltip("What is the window in which the player can jump to get the boosted launch force?")] [Range (0f , 2f)] public float jumpTimeOffset = 0.2f; // how much time does the player have from the peak of a slam, to get the jump boost?
    [Tooltip("Disables the childing collider for this long before hitting the bottom (avoids jostling the player)")] [Range (0f , 1f)] public float childDisableTime = 1f; // how much time does the elevator have from the bottom of the movement to disable the childing trigger collider (prevents jittering)
    [Tooltip("What layers can the elevator lift?")] public LayerMask liftableObjects; // what objects does the elevator interact with?
    float moveTimer = 0f; // the current time of the elevator's movement
    bool ascending = true; // is the elevator moving up?
    float triggerCheckTime = 0.3f; // intervals at which the trigger collider will automatically check for objects
    bool playerOnboard = false; // is the player on board?
    bool slam; // interval at which the player can jump to get the slam bonus
    [Tooltip("The high/end point of the elevator (if it slams, it will be towards this)")] public Vector2 highPoint; // the position of the elevator's endpoint (the elevator slams towards this)
    [Tooltip("The low/starting point of the elevator (It will start here)")] public Vector2 lowPoint; // the position of the elevator's starting point
    Vector2 topPosition;
    Vector2 botttomPosition;
    Vector2 slamDirection; // the vector direction of the elevator slam
    bool jumpSlam; // did the player add vertical input within a short window of the slam summit?


    GameObject player;
    GameObject holder;
    playerScript playerScript;
    public List<GameObject> onBoard = new List<GameObject>(); // objects currently onboard


    bool canPickup = true;

    void Start()
    {
        topPosition = highPoint + (Vector2)transform.position;
        botttomPosition = lowPoint + (Vector2)transform.position;
        holder = gameObject.transform.parent.transform.Find("ElevatorHolder").gameObject;
        player = GameObject.Find("Player").gameObject;
        playerScript = player.GetComponent<playerScript>();
        slamDirection = (topPosition - botttomPosition).normalized;
        Debug.Log(slamDirection);
    }

    void FixedUpdate()
    {
        if(!activated) // if there is no power, stop working
        {
            return;
        }

        holder.transform.position = gameObject.transform.position;

        if(moveTimer >= 1f) // reverse direction when movement is complete
        {
            if(ascending && overcharge) // call for the slam force effect when at the peak of a slam
            {
                Debug.Log("Slammed");
                SummitSlam();
            }
            ascending = !ascending;
            moveTimer = 0f;
        }

        if(overcharge && ascending) // when slamming up, prevent the player from jumping, then add a force at the end
        {
            if(moveTimer >= (1f - jumpTimeOffset) && overcharge) // when nearing the apex, the elevator gives the player a brief period to time to jump and use the boost
            {
                slam = true;
                if(Input.GetAxisRaw("Vertical") > 0.5) // if the player is holding up, increase the launch force
                {
                    jumpSlam = true;
                }
            }
            else
            {
                jumpSlam = false;
            }

            if(playerOnboard) // if the elevator is ascending with a slam, prevent the player from jumping (until right at the end)
            {
                playerScript.jumpBan = true;
                playerScript.remainingJumps = playerScript.maximumJumps;
                Debug.Log("Jump Banned");
            }
        }
        else // the player can jump normally and the slam force will not be applied
        {
            playerScript.jumpBan = false;
            slam = false;
            jumpSlam = false;
        }

        if(ascending)
        {
            //pickupPoint.enabled = true;
            if(overcharge) // move timer speeds up
            {
                moveTimer += Time.fixedDeltaTime / slamUpTime;
            }
            else // move timer slows down
            {   
                moveTimer += Time.fixedDeltaTime / moveTime;
            }
            // smoothly move upwards based on the move timer
            transform.position = new Vector2(Mathf.SmoothStep(botttomPosition.x , topPosition.x , moveTimer) , Mathf.SmoothStep(botttomPosition.y , topPosition.y , moveTimer));
        }
        else
        {
            moveTimer += Time.fixedDeltaTime / moveTime; // cannot slam down, only moves at normal speed

            if(moveTimer > (1 - childDisableTime) && !playerOnboard)
            {
                //pickupPoint.enabled = false;
            }

            // smoothly move downwards based on the move timer
            transform.position = new Vector2(Mathf.SmoothStep(topPosition.x , botttomPosition.x , moveTimer) , Mathf.SmoothStep(topPosition.y , botttomPosition.y , moveTimer));
        }

        // prevent elevator crush

        if(Input.GetKey("space") && playerOnboard == true) // refresh pickup point when the player detaches, to make sure all parts are accounted for
        {
            canPickup = false;
            canPickup = true;
        }
    }



    // NOTE - ON TRIGGER AND ON COLLISION OCCUR IN THE FIXED UPDATE CYCLE, AFTER FIXED UPDATE







    /// <summary>On Collision with the Elevator's solid Collider.</summary>
    void OnCollisionEnter2D(Collision2D col)
    {
        Vector3 collisionNormal = col.contacts[0].normal;
        if(collisionNormal.y < 0.55f)
        {
            Adopt(col.gameObject);
            Debug.Log(col.gameObject.name + " has boarded");
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Orphan(col.gameObject);
            Debug.Log(col.gameObject.name + " has disembarked");
    }


    /// <summary>Make the object a child of the elevator.</summary>
    void Adopt(GameObject child)
    {
        if(!onBoard.Contains(child))
        {
            onBoard.Add(child.transform.root.gameObject); // add parent gameObject
            child.transform.root.parent = holder.transform;
        }
    }


    /// <summary>Detach the child from the elevator.</summary>
    void Orphan(GameObject child)
    {
        if(onBoard.Contains(child))
        {
            onBoard.Remove(child);
            child.transform.parent = null;
        }
    }


    /// <summary> Called on the fixed update at which the elevator reaches the end of a slam</summary>
    void SummitSlam()
    {
        foreach (GameObject child in onBoard)
        {
            float slamLaunchForce = 0f;
            if(child.tag == "Player" && jumpSlam) // set jump boost force for player
            {
                slamLaunchForce = jumpLaunchForce;
                playerScript.remainingJumps = playerScript.maximumJumps - 1;
            }
            else
            {
                if(child.tag == "Player") // set standard force on player
                {
                    slamLaunchForce = launchForce * 1f;
                }
                else if(child.tag == "Legs" || child.tag == "Arms") // set force on legs and arms
                {
                    slamLaunchForce = launchForce * 1f;
                }
                else if(child.tag == "Box") // set force to be applied to box
                {
                    slamLaunchForce = launchForce * 1f;
                }
                else if(child.tag == "powerCell") // set force to be applied to powercell
                {
                    slamLaunchForce = launchForce * 1f;
                }
            }

            child.GetComponent<Rigidbody2D>().AddForce(slamDirection * slamLaunchForce , ForceMode2D.Impulse);
            Orphan(child);
        }
    }


//===========================//==========================//======================//===================//


    /// <summary>Indicate the points, and path of the elevator.</summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float locationIdentifier = 0.3f;
        
        // if in game, the waypoints don't move, so use the world positions

        if(Application.isPlaying)
        {
            Gizmos.DrawLine(topPosition - Vector2.up * locationIdentifier , topPosition + Vector2.up * locationIdentifier);
            Gizmos.DrawLine(topPosition - Vector2.left * locationIdentifier , topPosition + Vector2.left * locationIdentifier);
            Gizmos.DrawLine(botttomPosition - Vector2.up * locationIdentifier , botttomPosition + Vector2.up * locationIdentifier);
            Gizmos.DrawLine(botttomPosition - Vector2.left * locationIdentifier , botttomPosition + Vector2.left * locationIdentifier);
            Gizmos.DrawLine(topPosition, botttomPosition); // draw a line so we can see where the elevator moves
        }
        else
        {
            topPosition = highPoint + (Vector2)transform.position;
            botttomPosition = lowPoint + (Vector2)transform.position;
            Gizmos.DrawLine(topPosition - Vector2.up * locationIdentifier , topPosition + Vector2.up * locationIdentifier);
            Gizmos.DrawLine(topPosition - Vector2.left * locationIdentifier , topPosition + Vector2.left * locationIdentifier);
            Gizmos.DrawLine(botttomPosition - Vector2.up * locationIdentifier , botttomPosition + Vector2.up * locationIdentifier);
            Gizmos.DrawLine(botttomPosition - Vector2.left * locationIdentifier , botttomPosition + Vector2.left * locationIdentifier);
            Gizmos.DrawLine(topPosition, botttomPosition); // draw a line so we can see where the elevator moves
        }
    }
}
