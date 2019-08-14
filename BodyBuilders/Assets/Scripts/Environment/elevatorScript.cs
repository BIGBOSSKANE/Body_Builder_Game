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
    playerScript playerScript;
    
    void Start()
    {
        movePoint1 = gameObject.transform.Find("movePoint1").gameObject.transform.position;
        movePoint2 = gameObject.transform.Find("movePoint2").gameObject.transform.position;
        transform.position = movePoint1;
        moveTimer = 0f;
        forwards = true;
        playerOnboard = false;
        player = GameObject.Find("Player").gameObject;
        playerScript = player.GetComponent<playerScript>();
    }

    void FixedUpdate()
    {
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

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag != "Untagged" || col.tag != "PassThroughPlatform")
        {
            if(col.tag == "Player")
            {
                playerOnboard = true;
                if(playerScript.partConfiguration != 1)
                {
                    player.transform.parent = gameObject.transform; // causing wierd scaling issues for head, talk to brad
                }
            }
            else
            {
                col.gameObject.transform.parent = gameObject.transform;
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.tag != "Untagged" || col.tag != "PassThroughPlatform")
        {
            if(col.tag == "Player")
            {
                var OriginalConstraints = player.GetComponent<Rigidbody2D>().constraints;
                player.transform.parent = null;
                player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                player.GetComponent<Rigidbody2D>().constraints = OriginalConstraints;
            }
            else
            {
                col.gameObject.transform.parent = null;
            }
        }
    }
}