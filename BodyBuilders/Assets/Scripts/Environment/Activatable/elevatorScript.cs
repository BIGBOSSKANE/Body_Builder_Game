/*
Creator: Daniel
Created 09/08/2019
Last Edited by: Daniel
Last Edit 29/08/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevatorScript : activate
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
    bool jumpSlam; // did the player add vertical input within a short window of the slam summit?
    [Tooltip("The high/end point of the elevator (if it slams, it will be towards this)")] public Vector2 highPoint; // the position of the elevator's endpoint (the elevator slams towards this)
    [Tooltip("The low/starting point of the elevator (It will start here)")] public Vector2 lowPoint; // the position of the elevator's starting point
    Vector2 slamDirection; // the vector direction of the elevator slam
    GameObject player; // the player gameObject
    GameObject holder; // the holder gameObject to which elevator childed objects are assigned
    playerScript playerScript; // the player's script, used to reference part configuration, and prevent jumping during slam up
    BoxCollider2D pickupPoint; // the collider used to check for objects
    Vector2 topPosition;
    Vector2 botttomPosition;
    public List<GameObject> onBoard = new List<GameObject>(); // objects currently onboard

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
        holder.transform.position = Vector3.zero;
        holder.transform.localScale = new Vector3(1f , 1f, 1f);
        holder.transform.rotation = Quaternion.Euler(0f , 0f , 0f);
        playerScript = player.GetComponent<playerScript>();
        slamDirection = topPosition - botttomPosition; // get the direction of the slam movement and normalize it
        slamDirection = slamDirection.normalized; // note that the player can only have a vertical force applied
        jumpSlam = true;
    }


    void FixedUpdate()
    {
        if(!activated) // if there is no power, stop working
        {
            return;
        }

        if(Input.GetKey("space") && playerOnboard == true) // refresh pickup point when the player detaches
        {
            pickupPoint.enabled = false;
            pickupPoint.enabled = true;
        }

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
        
        if(overcharge && ascending)
        {
            if(moveTimer >= (1f - jumpTimeOffset) && overcharge) // when nearing the apex, the elevator gives the player a brief period to time to jump and use the boost
            {
                slam = true;
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

        holder.transform.position = transform.position; // set the holder's position

        // prevent elevator crush
    }

    void SummitSlam()
    {
        Debug.Log(onBoard.Count);
        for (int i = 0; i < onBoard.Count; i++)
        {
            float slamLaunchForce = 0f;
            if(onBoard[i].tag == "Player" && jumpSlam) // set jump boost force for player
            {
                slamLaunchForce = jumpLaunchForce;
                playerScript.remainingJumps = playerScript.maximumJumps - 1;
                playerScript.forceSlaved = true;
            }
            else
            {
                if(onBoard[i].tag == "Player") // set standard force on player
                {
                    slamLaunchForce = launchForce * 1f;
                    playerScript.forceSlaved = true;
                }
                else if(onBoard[i].tag == "Legs" || onBoard[i].tag == "Arms") // set force on legs and arms
                {
                    slamLaunchForce = launchForce * 1f;
                }
                else if(onBoard[i].tag == "Box") // set force to be applied to box
                {
                    slamLaunchForce = launchForce * 1f;
                }
                else if(onBoard[i].tag == "powerCell") // set force to be applied to powercell
                {
                    slamLaunchForce = launchForce * 1f;
                }
            }            

            onBoard[i].GetComponent<Rigidbody2D>().AddForce(slamDirection * slamLaunchForce , ForceMode2D.Impulse);
            Unparent(onBoard[i]);
            Debug.Log("Force Applied to " + onBoard[i].name);
        }

        onBoard.Clear();
    }

    /// <summary>On Collision with the Elevator's solid Collider.</summary>
    void OnCollisionEnter2D(Collision2D col)
    {
        Vector3 collisionNormal = col.contacts[0].normal;
        if(collisionNormal.y < 0.55f)
        {
            Parent(col.gameObject);
            Debug.Log(col.gameObject.name + " has boarded");
        }
    }

/*
    void OnTriggerEnter2D(Collider2D col) // When entering, apply the Parent Function
    {
        if(ascending || (moveTimer > (1 - childDisableTime) && !playerOnboard))
        {
            Parent(col.gameObject);
        }
        Debug.Log("Parented");
    }
 */

        void OnTriggerStay2D(Collider2D col)
    {
        if(slam == true) // while at the apex of an up slam
        {
            /*
            if(col.tag == "Player")
            {
                playerScript.forceSlaved = true;
                playerScript.forceSlavedTimer = 0f;
                playerScript.facingDirection = Mathf.RoundToInt(Mathf.Sign(slamDirection.x));

                if(Input.GetAxisRaw("Vertical") > 0f) // if the player is holding up, apply extra force, otherwise apply normal force
                {
                    player.GetComponent<Rigidbody2D>().AddForce(slamDirection * jumpLaunchForce , ForceMode2D.Impulse);
                }
                else
                {
                    player.GetComponent<Rigidbody2D>().AddForce(slamDirection * launchForce , ForceMode2D.Impulse);
                }
            }
            else if(col.gameObject.tag == "Legs" || col.gameObject.tag == "Arms") // apply normal force to legs and arms
            {
                if(col.gameObject.GetComponent<Rigidbody2D>() != null)
                {
                    col.gameObject.GetComponent<Rigidbody2D>().AddForce(slamDirection * launchForce , ForceMode2D.Impulse);
                }
                else
                {
                    col.transform.parent.gameObject.GetComponent<Rigidbody2D>().AddForce(slamDirection * launchForce , ForceMode2D.Impulse);
                }
            }
            else if(col.gameObject.tag == "Box" || col.gameObject.tag == "powerCell") // apply normal force to boxes and power cells
            {
                col.gameObject.GetComponent<Rigidbody2D>().AddForce(slamDirection * launchForce , ForceMode2D.Impulse);
            }

            */

            if(col.transform.position.y <= holder.transform.position.y) // prevent objects from falling through while moving quickly
            {
                col.transform.position = new Vector2(col.transform.position.x , holder.transform.position.y);
            }
        }

        // check the trigger stay periodically, in case something has slipped past the OnTriggerEnter check
        if(moveTimer % triggerCheckTime <= 0.0005f && moveTimer % triggerCheckTime >= -0.0005f)
        {
            if(ascending || (moveTimer > (1 - childDisableTime) && !playerOnboard))
            {
                Parent(col.gameObject);
            }
        }


        if(!slam && playerOnboard && Input.GetAxisRaw("Vertical") > 0)
        {
            pickupPoint.enabled = false;
            if(col.gameObject == player)
            {
                Unparent(player); 
                if(onBoard.Contains(player))
                {
                    onBoard.Remove(player);
                }
            }
        }
        else if((moveTimer > (1 - childDisableTime) && !ascending))
        {
            pickupPoint.enabled = true;
            Debug.Log("Doing it");
        }

    }

    void OnTriggerExit2D(Collider2D col) // when exiting, execute the unparent function
    {
        if(col.tag != "Untagged" && col.tag != "PassThroughPlatform" && col.tag != "Environment")
        {
            Unparent(col.gameObject);
        }

        if(onBoard.Contains(col.gameObject))
        {
            onBoard.Remove(col.gameObject);
        }
    }

    void Parent(GameObject child)
    {
        if(!onBoard.Contains(child))
        {
            onBoard.Add(child.transform.root.gameObject); // add parent gameObject

            // while not jumping, and if the object does not have an incorrect tag, parent it
            if(Input.GetAxisRaw("Vertical") <= 0.1f && (child.tag != "Untagged" || child.tag != "PassThroughPlatform" || child.tag != "Environment"))
            {
                if(child.tag == "Player" && playerOnboard == false) // if the player is not on board already, add them
                {
                    player.transform.parent = holder.transform;
                    
                    if(playerScript.partConfiguration == 1) // prevent unity issues where ghost colliders would activate
                    {
                        Destroy(player.GetComponent<BoxCollider2D>());
                    }
                    playerOnboard = true; // the player is now on board
                }
                else // reference the correct transform for legs and arms (they use multiple childed game objects)
                {
                    child.transform.root.parent = holder.transform;
                }
            }
        }
        else
        {
            Debug.Log("Oops");
        }
    }

    void Unparent(GameObject child)
    {
        child.transform.parent = null;

        if(child.tag == "Player")
        {                
            player.transform.parent = null; // the player is no longer a child of the holder
            
            if(playerScript.partConfiguration == 1) // destroy colliders to prevent unity collider ghosting error
            {
                Destroy(player.GetComponent<BoxCollider2D>());
            }
            playerOnboard = false; // player is no longer on board
        }
        else if(child.tag == "Arms" || child.tag == "Legs") // set the leg or arm full game object so that it is no longer the child of the holder
        {
            if(child.gameObject.transform.parent != null)
            {
                if(child.gameObject.transform.parent.parent != null)
                {
                    child.gameObject.transform.parent.parent = null;
                }
            }
        }
        else if(child.tag == "Box" || child.tag == "Powercell") // unchild boxes or powercells
        {
            child.gameObject.transform.gameObject.transform.parent = null;
        }
        else // generic check for which parent in the hierarchy is the holder, then make that one null to unchild the object
        {
            if(child.transform.parent != null && child.transform.parent == holder)
            {
                child.transform.parent = null;
            }
            else if(child.transform.parent.parent != null && child.transform.parent.parent == holder)
            {
                child.transform.parent.gameObject.transform.parent = null;
            }
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