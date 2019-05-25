﻿/*
Creator: Daniel
Created: 09/04/2019
Laste Edited by: Daniel
Last Edit: 25/05/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legs : MonoBehaviour
{
    bool attached;
    float unavailableTimer = 1f;
    public BoxCollider2D boxCol;
    public Rigidbody2D rb;
    public string identifierLegString = "Basic";
    public GameObject player;
    public GameObject head;
    public Player_Controller playerScript;
    GameObject solidCollider;
    BoxCollider2D solidBoxCollider;

    void Start()
    {
        boxCol = this.GetComponent<BoxCollider2D>();
        rb = this.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        head = player.transform.Find("Head").gameObject;
        playerScript = player.GetComponent<Player_Controller>();
        solidCollider = transform.Find("Legs_Solid_Collider").gameObject;
        solidBoxCollider = solidCollider.GetComponent<BoxCollider2D>();
        solidBoxCollider.enabled = true;
        CheckForParent();
    }

    void Update()
    {
        if(unavailableTimer < 1f)
        {
            unavailableTimer += Time.deltaTime;
        }
    }

    void CheckForParent()
    {
        if(transform.parent == null)
        {
            Detached();
        }
        else
        {
            Attached();
        }
    }

    public void Detached()
    {
        transform.parent = null;
        solidBoxCollider.enabled = true;
        boxCol.enabled = true;
        attached = false;
        unavailableTimer = 0f;
        rb.isKinematic = false;
    }

    public void Attached()
    {
        attached = true;
        solidBoxCollider.enabled = false;
        boxCol.enabled = false;
        rb.isKinematic = true;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Vector2 thisPos = gameObject.transform.position;
        if(col.gameObject.tag == "Player" && playerScript.isGrounded == true) // reset the collider if the player is not jumping
        {
            boxCol.enabled = false;
            boxCol.enabled = true;
        }
        else if(col.gameObject.tag == "Player" && (playerScript.isGrounded == false || playerScript.partConfiguration == 2))
        {
            playerScript.legString = identifierLegString;
            int playerParts = playerScript.partConfiguration;
            if(attached == false && playerParts != 3 && playerParts != 4 && unavailableTimer > 0.3f)
            {
                col.gameObject.transform.rotation = Quaternion.identity;
                if(playerParts == 1) // this one needs changing
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

//create a child object with a capsule collider that is on a layer which can't collide with the player
//set this one as a trigger collider


//alternatively, create a layermask for collisions with the legs, that references when the player is grounded, when not, the layermask allows the player to come in contact