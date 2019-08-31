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
    [Range (0.1f , 20f)] public float moveTime = 5f; // how long between positions?
    [Range (0.1f , 10f)] public float slamUpTime = 0.8f; // how long does the elevator take to slam upwards?
    // public bool overcharge = false; // will the elevator slam upwards?         ***Make sure the elevator is completely vertical for this to work***
    [Range (0f , 10f)] public float launchForce = 3f; // force applied at the movement apex when slamming up
    [Range (0f , 10f)] public float jumpLaunchForce = 6f; // if the player jumps at the apex of a slam, they get launched further
    [Range (0f , 2f)] public float jumpTimeOffset = 0.2f; // how much time does the player have from the peak of a slam, to get the jump boost?
    public LayerMask liftableObjects; // what objects does the elevator interact with?
    float moveTimer = 0f; // the current time of the elevator's movement
    bool ascending = true; // is the elevator moving up?
    float triggerCheckTime = 0.3f; // intervals at which the trigger collider will automatically check for objects
    bool playerOnboard = false; // is the player on board?
    bool slam; // interval at which the player can jump to get the slam bonus
    Vector2 lowPoint; // the position of the elevator's starting point
    Vector2 highPoint; // the position of the elevator's endpoint (the elevator slams towards this)
    Vector2 slamDirection; // the vector direction of the elevator slam
    GameObject player; // the player gameObject
    GameObject holder; // the holder gameObject to which elevator childed objects are assigned
    playerScript playerScript; // the player's script, used to reference part configuration, and prevent jumping during slam up
    BoxCollider2D pickupPoint; // the collider used to check for objects
    
    void Start()
    {
        lowPoint = gameObject.transform.Find("lowPoint").gameObject.transform.position;
        highPoint = gameObject.transform.Find("highPoint").gameObject.transform.position;
        pickupPoint = gameObject.GetComponent<BoxCollider2D>();
        transform.position = lowPoint;
        moveTimer = 0f;
        ascending = true;
        playerOnboard = false;
        player = GameObject.Find("Player").gameObject;
        holder = gameObject.transform.parent.gameObject.transform.Find("ElevatorHolder").gameObject;
        holder.transform.position = Vector3.zero;
        holder.transform.localScale = new Vector3(1f , 1f, 1f);
        holder.transform.rotation = Quaternion.Euler(0f , 0f , 0f);
        playerScript = player.GetComponent<playerScript>();

        slamDirection = highPoint - lowPoint; // get the direction of the slam movement and normalize it
        slamDirection = slamDirection.normalized; // note that the player can only have a vertical force applied
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(lowPoint, highPoint); // draw a line so we can see where the elevator moves
    }

    void FixedUpdate()
    {
        if(!activated) // if there is no power, stop working
        {
            return;
        }

        holder.transform.position = transform.position; // set the holder to the elevator's position to keep all childed game objects in sync


        if(moveTimer >= 1f) // reverse direction when movement is complete
        {
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
            }
        }
        else // the player can jump normally and the slam force will not be applied
        {
            playerScript.jumpBan = false;
            slam = false;
        }

        if(ascending)
        {
            if(overcharge) // move timer speeds up
            {
                moveTimer += Time.fixedDeltaTime / slamUpTime;
            }
            else // move timer slows down
            {   
                moveTimer += Time.fixedDeltaTime / moveTime;
            }
            // smoothly move upwards based on the move timer
            transform.position = new Vector2(Mathf.SmoothStep(lowPoint.x , highPoint.x , moveTimer) , Mathf.SmoothStep(lowPoint.y , highPoint.y , moveTimer));
        }
        else
        {
            moveTimer += Time.fixedDeltaTime / moveTime; // cannot slam down, only moves at normal speed
            // smoothly move downwards based on the move timer
            transform.position = new Vector2(Mathf.SmoothStep(highPoint.x , lowPoint.x , moveTimer) , Mathf.SmoothStep(highPoint.y , lowPoint.y , moveTimer));
        }
        
        holder.transform.position = transform.position; // set the holder's position again for late frame check
    }

    void Update()
    {
        if(Input.GetKey("space") && playerOnboard == true) // refresh pickup point when the player detaches
        {
            pickupPoint.enabled = false;
            pickupPoint.enabled = true;
        }
    }

    void OnTriggerEnter2D(Collider2D col) // When entering, apply the Parent Function
    {
        Parent(col);
    }

        void OnTriggerStay2D(Collider2D col)
    {
        if(slam == true) // while at the apex of an up slam
        {
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

            if(col.transform.position.y <= holder.transform.position.y) // prevent objects from falling through while moving quickly
            {
                col.transform.position = new Vector2(col.transform.position.x , holder.transform.position.y);
            }
        }

        // check the trigger stay periodically, in case something has slipped past the OnTriggerEnter check
        if(moveTimer % triggerCheckTime <= 0.0005f && moveTimer % triggerCheckTime >= -0.0005f)
        {
            Parent(col);
        }
    }

    void OnTriggerExit2D(Collider2D col) // when exiting, execute the unparent function
    {
        if(col.tag != "Untagged" && col.tag != "PassThroughPlatform" && col.tag != "Environment")
        {
            Unparent(col);
        }
    }

    void Parent(Collider2D col)
    {
        // while not jumping, and if the object does not have an incorrect tag, parent it
        if(Input.GetAxisRaw("Vertical") <= 0.1f && (col.tag != "Untagged" || col.tag != "PassThroughPlatform" || col.tag != "Environment"))
        {
            if(col.tag == "Player" && playerOnboard == false) // if the player is not on board already, add them
            {
                player.transform.parent = holder.transform;
                
                if(playerScript.partConfiguration == 1) // prevent unity issues where ghost colliders would activate
                {
                    Destroy(player.GetComponent<BoxCollider2D>());
                }
                playerOnboard = true; // the player is now on board
            }
            else if(col.tag == "Arms" || col.tag == "Legs") // reference the correct transform for legs and arms (they use multiple childed game objects)
            {
                col.gameObject.transform.parent.parent = holder.transform;
            }
            else if(col.tag == "Box" || col.tag == "Powercell") // child Boxes and Powercells normally
            {
                col.gameObject.transform.parent = holder.transform;
            }
            else // generic check for parent, this will find whatever object is highest in the parent hierarchy, and make it the child of the holder
            {
                if(col.gameObject.transform.parent == null)
                {
                    col.gameObject.transform.SetParent(holder.transform);
                }
                else if(col.gameObject.transform.parent.parent == null)
                {
                    col.gameObject.transform.parent.SetParent(holder.transform);
                }
                else if(col.gameObject.transform.parent.parent.parent == null)
                {
                    col.gameObject.transform.parent.parent.SetParent(holder.transform);
                }
                else if(col.gameObject.transform.parent.parent.parent.parent == null)
                {
                    col.gameObject.transform.parent.parent.parent.SetParent(holder.transform);
                }
            }
        }
    }

    void Unparent(Collider2D col)
    {
        if(col.tag == "Player")
        {                
            player.transform.parent = null; // the player is no longer a child of the holder
            
            if(playerScript.partConfiguration == 1) // destroy colliders to prevent unity collider ghosting error
            {
                Destroy(player.GetComponent<BoxCollider2D>());
            }
            playerOnboard = false; // player is no longer on board
        }
        else if(col.tag == "Arms" || col.tag == "Legs") // set the leg or arm full game object so that it is no longer the child of the holder
        {
            if(col.gameObject.transform.parent != null)
            {
                if(col.gameObject.transform.parent.parent != null)
                {
                    col.gameObject.transform.parent.parent = null;
                }
            }
        }
        else if(col.tag == "Box" || col.tag == "Powercell") // unchild boxes or powercells
        {
            col.gameObject.transform.gameObject.transform.parent = null;
        }
        else // generic check for which parent in the hierarchy is the holder, then make that one null to unchild the object
        {
            if(col.gameObject.transform.parent != null && col.gameObject.transform.parent == holder)
            {
                col.gameObject.transform.parent = null;
            }
            else if(col.gameObject.transform.parent.parent != null && col.gameObject.transform.parent.parent == holder)
            {
                col.gameObject.transform.parent.gameObject.transform.parent = null;
            }
        }
    }
}