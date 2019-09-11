/*
Creator: Daniel
Created: 09/04/2019
Laste Edited by: Daniel
Last Edit: 07/09/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legs : MonoBehaviour
{
    bool attached;
    [HideInInspector] public bool groundBreaker = false; // when ground breakers are dropped, they will continue to do their thing
    float unavailableTimer = 1f;
    BoxCollider2D boxCol;
    Rigidbody2D rb;

    public enum legIdentifier{ Basic, Afterburner, Groundbreaker} // sets up for the dropdown menu of options
    [Tooltip("What type of legs are these?")] public legIdentifier legType;

    GameObject player;
    GameObject head;
    playerScript playerScript;
    GameObject solidCollider;
    BoxCollider2D solidBoxCollider;
    PlatformEffector2D platEffect;
    timeSlow timeSlowScript;
    float yCastOffset = -0.86f;
    float raycastDistance = 0.17f;
    float maxHeight;
    float groundbreakerDistance;
    LayerMask jumpLayer;
    bool groundBreakerReset;
    float boxColTimer;


    void Start()
    {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<playerScript>();
        boxCol = gameObject.GetComponent<BoxCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        timeSlowScript = player.GetComponent<timeSlow>();
        head = player.transform.Find("Head").gameObject;
        jumpLayer = playerScript.jumpLayer;
        solidCollider = transform.Find("solidCollider").gameObject;
        solidBoxCollider = solidCollider.GetComponent<BoxCollider2D>();
        platEffect = gameObject.GetComponent<PlatformEffector2D>();
        solidBoxCollider.enabled = true;
        this.name = legType + "Legs";
        CheckForParent();
        unavailableTimer = 1f;
    }

    void Update()
    {
        if(unavailableTimer < 1f)
        {
            unavailableTimer += Time.deltaTime;
        }

        if(boxColTimer > 0f)
        {
            boxColTimer -= Time.deltaTime;
        }
        else if(!attached)
        {
            boxCol.enabled = true;
            boxColTimer = 0f;
            gameObject.layer = 18;
        }

        if(transform.position.y <= (maxHeight - groundbreakerDistance))
        {
            if(groundBreaker && !groundBreakerReset)
            {
                timeSlowScript.TimeSlow();
                groundBreakerReset = true;
            }
        }

        if(groundBreaker == true)
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + yCastOffset), Vector2.down, raycastDistance, jumpLayer);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + yCastOffset), Vector2.down * raycastDistance, Color.red);

            if(hit.collider != null)
            {
                if(transform.position.y <= (maxHeight - groundbreakerDistance))
                {
                    if(hit.collider.gameObject.tag == "Groundbreakable")
                    {
                        hit.collider.gameObject.GetComponent<Groundbreakable_Script>().Groundbreak();
                    }
                }
                else
                {
                    maxHeight = transform.position.y;
                    groundBreakerReset = false;
                    groundBreaker = false;
                }
                timeSlowScript.TimeNormal();
            }
        }
    }

    void CheckForParent()
    {
        if(transform.parent != null && transform.parent.tag == "Player")
        {
            Attached();
        }
        else
        {
            Detached(0f , 0f);
        }
    }

    public void Detached(float maxHeightCalled , float groundbreakerDistanceCalled)
    {
        maxHeight = maxHeightCalled;
        groundbreakerDistance = groundbreakerDistanceCalled;
        transform.parent = null;
        solidBoxCollider.enabled = true;
        platEffect.enabled = true;
        boxCol.enabled = true;
        attached = false;
        unavailableTimer = 0f;
        rb.isKinematic = false;
        gameObject.layer = 13;
        if(gameObject.name == "GroundbreakerLegs")
        {
            groundBreaker = true;
        }
    }

    public void Attached()
    {
        attached = true;
        boxCol.enabled = false;
        solidBoxCollider.enabled = false;
        platEffect.enabled = false;
        rb.isKinematic = true;
        gameObject.layer = 0; // switch physics layers so the player raycast doesn't think it's ground
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Vector2 thisPos = gameObject.transform.position;
        if(col.gameObject.tag == "Player" && playerScript.partConfiguration != 2 && playerScript.isGrounded == true) // reset the collider if the player is not jumping
        {
            boxCol.enabled = false;
            boxCol.enabled = true;
        }
        else if(col.gameObject.tag == "Player" && (!playerScript.isGrounded || playerScript.partConfiguration == 2) && playerScript.partConfiguration != 3 && playerScript.partConfiguration != 4)
        {
            playerScript.legString = this.name;
            int playerParts = playerScript.partConfiguration;
            if(attached == false && playerParts != 3 && playerParts != 4 && unavailableTimer > 0.3f)
            {
                col.gameObject.transform.rotation = Quaternion.identity;
                if(playerParts == 1)
                {
                    thisPos.y += 0.014f;
                    col.gameObject.transform.position = thisPos;                   
                }
                else if(playerParts == 2)
                {
                    thisPos.y += 0.014f;
                    col.gameObject.transform.position = thisPos;
                }
                Attached();
                this.gameObject.transform.parent = col.transform;
                playerScript.UpdateParts();
            }
        }
    }
}