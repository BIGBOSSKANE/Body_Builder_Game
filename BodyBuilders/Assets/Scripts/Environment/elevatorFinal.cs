using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevatorFinal : activate
{
    [Tooltip("How long does the elevator take to move normally?")] [Range (0.1f , 20f)] public float moveTime = 5f; // how long between positions?
    [Tooltip("How long does the elevator take to slam upwards?")] [Range (0.1f , 5f)] public float slamUpTime = 0.8f; // how long does the elevator take to slam upwards?
    // public bool overcharge = false; // will the elevator slam upwards?         ***Make sure the elevator is completely vertical for this to work***
    [Tooltip("How much launch force does the elevator provide at the apex of an upwards slam?")] [Range (0f , 20f)] public float launchForce = 3f; // force applied at the movement apex when slamming up
    [Tooltip("How much futher is the player launched if they jump at the apex of the slam?")] [Range (0f , 20f)] public float jumpLaunchForce = 6f; // if the player jumps at the apex of a slam, they get launched further
    [Tooltip("What is the window in which the player can jump to get the boosted launch force?")] [Range (0f , 2f)] public float jumpTimeOffset = 0.2f; // how much time does the player have from the peak of a slam, to get the jump boost?
    [Tooltip("Disables the childing collider for this long before hitting the bottom (avoids jostling the player)")] [Range (0f , 1f)] public float childDisableTime = 1f; // how much time does the elevator have from the bottom of the movement to disable the childing trigger collider (prevents jittering)
    [Tooltip("What layers can the elevator lift?")] public LayerMask liftableObjects; // what objects does the elevator interact with?
    float moveTimer = 0f; // the current time of the elevator's movement
    bool ascending = true; // is the elevator moving up?
    bool playerOnboard = false; // is the player on board?
    bool slam; // interval at which the player can jump to get the slam bonus
    bool jumpSlam; // did the player add vertical input within a short window of the slam summit?
    [Tooltip("The high/end point of the elevator (if it slams, it will be towards this)")] public Vector2 highPoint; // the position of the elevator's endpoint (the elevator slams towards this)
    [Tooltip("The low/starting point of the elevator (It will start here)")] public Vector2 lowPoint; // the position of the elevator's starting point
     [Range (1f , 20f)] [Tooltip("Use to set the vertical force when slamming horizontally")] public float verticalForceOverride = 3f;
    Vector2 slamDirection; // the vector direction of the elevator slam
    GameObject player; // the player gameObject
    GameObject holder; // the holder gameObject to which elevator childed objects are assigned
    GameObject elevatorPlatform; // the solid elevator platform
    playerScript playerScript; // the player's script, used to reference part configuration, and prevent jumping during slam up
    BoxCollider2D pickupPoint; // the collider used to check for objects
    Vector2 topPosition;
    Vector2 botttomPosition;
    Vector2 motion;
    float pickupInitialOffset;
    bool jumpBan;
    float jumpCutTimer = 3f;
    public List<GameObject> onBoard = new List<GameObject>(); // objects currently onboard
    bool horizontal;

//===========================//==========================//======================//===================//

    void Start()
    {
        pickupPoint = gameObject.GetComponent<BoxCollider2D>();
        topPosition = highPoint + (Vector2)transform.position;
        botttomPosition = lowPoint + (Vector2)transform.position;
        transform.position = botttomPosition;
        moveTimer = 0f;
        ascending = true;
        playerOnboard = false;
        player = GameObject.Find("Player").gameObject;
        holder = gameObject.transform.parent.gameObject.transform.Find("ElevatorHolder").gameObject;
        elevatorPlatform = gameObject.transform.Find("ElevatorSolid").gameObject;
        holder.transform.position = Vector3.zero;
        holder.transform.localScale = new Vector3(1f , 1f, 1f);
        holder.transform.rotation = Quaternion.Euler(0f , 0f , 0f);
        playerScript = player.GetComponent<playerScript>();
        slamDirection = topPosition - botttomPosition; // get the direction of the slam movement and normalize it
        slamDirection = slamDirection.normalized; // note that the player can only have a vertical force applied
        pickupInitialOffset = pickupPoint.offset.y;
        horizontal = (Mathf.Abs(slamDirection.x) > Mathf.Abs(slamDirection.y))? true : false;
    }

//===========================//==========================//======================//===================//

    void FixedUpdate()
    {
        if(!activated)
        {
            return;
        }

        if(onBoard.Contains(gameObject)) onBoard.Remove(gameObject);

        ElevatorMovement(); // the elevator moves
        holder.transform.position = transform.position; // set the holder's position
    }

//===========================//==========================//======================//===================//

    /// <summary>Move the Elevator.</summary>
    void ElevatorMovement()
    {
        if(moveTimer >= 1f) // reverse direction when movement is complete
        {
            if(ascending && overcharge) // call for the slam force effect when at the peak of a slam
            {
                SummitSlam();
                onBoard.Clear();
                jumpCutTimer = 0f; // sets jumpCut within the limits
            }
            ascending = !ascending;
            moveTimer = 0f;
        }

        if(jumpCutTimer > -1f && jumpCutTimer < 1f) // while within the limits, increase in value to 0.1, cutting off the player's remaining jumps by 1 to offset the ground check on the elevator
        {
            jumpCutTimer += Time.fixedDeltaTime;
            if(jumpCutTimer > 0.1f)
            {
                playerScript.remainingJumps = playerScript.maximumJumps-1;
                jumpCutTimer = 3f;
            }
        }

        bool wasJumpBanned = jumpBan;

        if(overcharge && ascending)
        {
            elevatorPlatform.layer = 25; // dynamic environment is a non-jumpable layer

            if(playerOnboard) // if the elevator is ascending with a slam, prevent the player from jumping (until right at the end)
            {
                jumpBan = true;
                playerScript.remainingJumps = playerScript.maximumJumps;
            }
        }
        else // the player can jump normally and the slam force will not be applied
        {
            elevatorPlatform.layer = 23; // elevator layer is a jumpable layer

            if(overcharge && !ascending && moveTimer > 0.8f && playerOnboard) jumpBan = true;
            else jumpBan = false;
            
            slam = false;
            if(Input.GetAxisRaw("Vertical") > 0.5f && !slam)
            {
                if(onBoard.Contains(player))
                {
                    if(ascending) motion = (topPosition - botttomPosition) / moveTime;
                    else motion = (botttomPosition - topPosition) / moveTime;

                    playerScript.Jump();
                    Unparent(player);
                }
            }
        }

        if(jumpBan != wasJumpBanned)
        {
            if(jumpBan) playerScript.jumpBan = true;
            else playerScript.jumpBan = false;
        }

        if(ascending)
        {
            pickupPoint.offset = new Vector2(0 , pickupInitialOffset);
            if(overcharge) // move timer speeds up
            {
                moveTimer += Time.fixedDeltaTime / slamUpTime;
                // smoothly move upwards based on the move timer
                transform.position = new Vector2(Mathf.Lerp(botttomPosition.x , topPosition.x , moveTimer) , Mathf.Lerp(botttomPosition.y , topPosition.y , moveTimer));
            }
            else // move timer slows down
            {   
                moveTimer += Time.fixedDeltaTime / moveTime;
                // smoothly move upwards based on the move timer
                transform.position = new Vector2(Mathf.SmoothStep(botttomPosition.x , topPosition.x , moveTimer) , Mathf.SmoothStep(botttomPosition.y , topPosition.y , moveTimer));
            }
        }
        else
        {
            moveTimer += Time.fixedDeltaTime / moveTime; // cannot slam down, only moves at normal speed

            if(moveTimer > 0.2f && !ascending && overcharge)
            {
                elevatorPlatform.layer = 23;
            }

            if(moveTimer > (1 - childDisableTime) && !playerOnboard)
            {
                pickupPoint.offset = new Vector2(0 , 5000f);
            }
            else
            {
                pickupPoint.offset = new Vector2(0 , pickupInitialOffset);
            }

            // smoothly move downwards based on the move timer
            transform.position = new Vector2(Mathf.SmoothStep(topPosition.x , botttomPosition.x , moveTimer) , Mathf.SmoothStep(topPosition.y , botttomPosition.y , moveTimer));
        }
    }

//===========================//==========================//======================//===================//

    /// <summary>Apply a force to all child objects at the top of a slam.</summary>
    void SummitSlam()
    {
        slam = true;
        foreach (GameObject child in onBoard)
        {
            float slamLaunchForce = 0f;
            if(child.tag == "Player" && Input.GetAxisRaw("Vertical") > 0.1f || (horizontal &&  Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")) == Mathf.RoundToInt(slamDirection.x))) // set jump boost force for player
            {
                slamLaunchForce = jumpLaunchForce;
                jumpBan = false;
                playerScript.jumpBan = false;
                playerScript.jumpGateTimer = 0f;
                playerScript.coyoteTimeLimit = 0.4f;
                playerScript.forceSlaved = true;
                playerScript.remainingJumps = playerScript.maximumJumps - 1;
                elevatorPlatform.layer = 25;
            }
            else
            {
                if(child.tag == "Player") // set standard force on player
                {
                    slamLaunchForce = launchForce * 1f;
                    if(horizontal)
                    {
                        jumpBan = false;
                        playerScript.jumpBan = false;
                        playerScript.jumpGateTimer = 0f;
                        playerScript.coyoteTimeLimit = 0.4f;
                        playerScript.forceSlaved = true;
                        playerScript.remainingJumps = playerScript.maximumJumps - 1;

                        elevatorPlatform.layer = 25;
                    }
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

            Vector2 slamForce = slamDirection * slamLaunchForce;
            if(slamForce.y < verticalForceOverride) slamForce.y = verticalForceOverride;
            child.GetComponent<Rigidbody2D>().AddForce(slamForce , ForceMode2D.Impulse);
            Unparent(child);
        }
    }

//===========================//==========================//======================//===================//

    /// <summary>Add the game object to the onboard list and the transform holder.</summary>
    void Parent(GameObject child)
    {
        if(!onBoard.Contains(child) && child.tag != "Untagged" || child.tag != "PassThroughPlatform" || child.tag != "Environment")
        {
            if(child.tag == "Player") // if the player is not on board already, add them
            {
                if(playerScript.partConfiguration == 1) // prevent unity issues where ghost colliders would activate
                {
                    Destroy(player.GetComponent<BoxCollider2D>());
                }
                playerOnboard = true; // the player is now on board
                if(!onBoard.Contains(player)) onBoard.Add(player); // add parent gameObject
                player.transform.parent = holder.transform;
                playerScript.jumpBan = true;
                jumpBan = true; // reset jumpBan just to make sure it is not overwritten
            }
            else
            {
                Debug.Log("Non player");
                onBoard.Add(child); // add parent gameObject
                child.transform.root.parent = holder.transform;
            }
        }
    }

//===========================//==========================//======================//===================//

    /// <summary>Remove the game object from the onboard list and the transform holder.</summary>
    void Unparent(GameObject child)
    {
        if(child.tag == "Player")
        {            
            if(playerScript.partConfiguration == 1) // destroy colliders to prevent unity collider ghosting error
            {
                Destroy(player.GetComponent<BoxCollider2D>());
            }
            playerOnboard = false; // player is no longer on board
            if(!slam) onBoard.Remove(player);
            jumpBan = false;
            playerScript.jumpBan = false;
            player.transform.parent = null; // the player is no longer a child of the holder
        }
        else // set the leg or arm full game object so that it is no longer the child of the holder
        {
            if(!slam) onBoard.Remove(child);
            child.transform.parent = null;
        }
    }

//===========================//==========================//======================//===================//

    /// <summary>When the player collides with the top of the elevator platform.</summary>
    void OnCollisionEnter2D(Collision2D col)
    {
        Vector3 collisionNormal = col.contacts[0].normal;
        if(collisionNormal.y < 0.55f && col.transform.position.y >= holder.transform.position.y)
        {
            Parent(col.gameObject);
        }
    }

//===========================//==========================//======================//===================//

    /// <summary>When the player leaves the trigger area.</summary>
    void OnTriggerExit2D(Collider2D col) // when exiting, execute the unparent function
    {
        if(col.tag != "Untagged" && col.tag != "PassThroughPlatform" && col.tag != "Environment")
        {
            Unparent(col.gameObject);
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
