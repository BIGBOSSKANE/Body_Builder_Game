using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevatorScript : MonoBehaviour
{
    public float moveTime = 5f;
    public bool jumpBooster = false;
    public float launchForce = 3f;
    public LayerMask liftableObjects;
    public float moveTimer = 0f;
    public bool forwards = true;
    bool playerOnboard = false;
    Vector2 movePoint1;
    Vector2 movePoint2;
    GameObject player;
    GameObject holder;
    playerScript playerScript;
    BoxCollider2D pickupPoint;
    
    void Start()
    {
        movePoint1 = gameObject.transform.Find("movePoint1").gameObject.transform.position;
        movePoint2 = gameObject.transform.Find("movePoint2").gameObject.transform.position;
        pickupPoint = gameObject.GetComponent<BoxCollider2D>();
        transform.position = movePoint1;
        moveTimer = 0f;
        forwards = true;
        playerOnboard = false;
        player = GameObject.Find("Player").gameObject;
        holder = gameObject.transform.parent.gameObject.transform.Find("ElevatorHolder").gameObject;
        playerScript = player.GetComponent<playerScript>();
    }

    void FixedUpdate()
    {
        holder.transform.position = transform.position;
        if(!jumpBooster)
        {
            if(moveTimer >= 1f)
            {
                forwards = !forwards;
                moveTimer = 0f;
            }
            if(forwards)
            {
                transform.position = new Vector2(Mathf.SmoothStep(movePoint1.x , movePoint2.x , moveTimer) , Mathf.SmoothStep(movePoint1.y , movePoint2.y , moveTimer));
            }
            else
            {
                transform.position = new Vector2(Mathf.SmoothStep(movePoint2.x , movePoint1.x , moveTimer) , Mathf.SmoothStep(movePoint2.y , movePoint1.y , moveTimer));
            }
            moveTimer += Time.fixedDeltaTime / moveTime;
        }
        else
        {
            if(playerOnboard && moveTimer >= (moveTime - 0.5f))
            {
                player.GetComponent<Rigidbody2D>().AddForce(transform.up * launchForce , ForceMode2D.Impulse);
            }
        }
    }


    void Update()
    {
        if(Input.GetKey("space"))
        {
            pickupPoint.enabled = false;
            pickupPoint.enabled = true;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag != "Untagged" || col.tag != "PassThroughPlatform" || col.tag != "Environment")
        {
            if(col.tag == "Player")
            {
                playerOnboard = true;
                
                if(playerScript.partConfiguration != 1)
                {
                    player.transform.parent = holder.transform; // causing wierd scaling issues for head, talk to brad
                }
                
                player.transform.parent = holder.transform;
                playerScript.UpdateParts();
            }
            else
            {
                if(col.gameObject.transform.parent == null)
                {
                    col.gameObject.transform.parent = holder.transform;
                }
                else if(col.gameObject.transform.parent.gameObject.transform.parent == null)
                {
                    col.gameObject.transform.parent.gameObject.transform.parent = holder.transform;
                }
                else if(col.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent == null)
                {
                    col.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent = holder.transform;
                }
                else if(col.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent == null)
                {
                    col.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent = holder.transform;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Exiting(col);
    }

    void Exiting(Collider2D col)
    {
        if(col.tag != "Untagged" || col.tag != "PassThroughPlatform")
        {
            if(col.tag == "Player")
            {
                var OriginalConstraints = player.GetComponent<Rigidbody2D>().constraints;
                playerOnboard = false;
                player.transform.parent = null;
                player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                player.GetComponent<Rigidbody2D>().constraints = OriginalConstraints;
            }
            else
            {
                if(col.gameObject.transform.parent == holder)
                {
                    col.gameObject.transform.parent = null;
                }
                else if(col.gameObject.transform.parent.gameObject.transform.parent == holder)
                {
                    col.gameObject.transform.parent.gameObject.transform.parent = null;
                }
                else if(col.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent == holder)
                {
                    col.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent = null;
                }
            }
        }

        if(col.tag == "Player")
        {
            playerOnboard = false;
        }
    }
}