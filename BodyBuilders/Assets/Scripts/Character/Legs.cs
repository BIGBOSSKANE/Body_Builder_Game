﻿/*
Creator: Daniel
Created: 09/04/2019
Laste Edited by: Daniel
Last Edit: 26/05/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legs : MonoBehaviour
{
    bool attached;
    public bool groundBreaker = false; // when ground breakers are dropped, they will continue to do their thing
    float unavailableTimer = 1f;
    public BoxCollider2D boxCol;
    public Rigidbody2D rb;

    public enum legIdentifier{ Basic, Afterburner, Groundbreaker} // sets up for the dropdown menu of options
    public legIdentifier legType;

    public GameObject player;
    public GameObject head;
    public playerScript playerScript;
    GameObject solidCollider;
    BoxCollider2D solidBoxCollider;
    timeSlow timeSlowScript;
    public float yCastOffset = -0.86f;
    public float raycastDistance = 0.17f;
    float maxHeight;
    float groundbreakerDistance;
    public LayerMask jumpLayer;
    bool groundBreakerReset;


    void Start()
    {
        boxCol = this.GetComponent<BoxCollider2D>();
        rb = this.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        timeSlowScript = player.GetComponent<timeSlow>();
        head = player.transform.Find("Head").gameObject;
        playerScript = player.GetComponent<playerScript>();
        solidCollider = transform.Find("solidCollider").gameObject;
        solidBoxCollider = solidCollider.GetComponent<BoxCollider2D>();
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
        boxCol.enabled = true;
        attached = false;
        unavailableTimer = 0f;
        rb.isKinematic = false;
        gameObject.layer = 18;
        if(gameObject.name == "GroundbreakerLegs")
        {
            groundBreaker = true;
        }
    }

    public void Attached()
    {
        attached = true;
        solidBoxCollider.enabled = false;
        boxCol.enabled = false;
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