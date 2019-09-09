/*
Creator: Daniel
Created: 09/04/2019
Last Edited by: Daniel
Last Edit: 07/09/2019
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arms : MonoBehaviour
{
    bool attached = false;
    float unavailableTimer = 1f;
    BoxCollider2D boxCol; // player pickup collider
    PlatformEffector2D platEffect;
    float boxColTimer;
    Rigidbody2D rb;
    public enum armIdentifier{ Basic, Lifter, Shield} // sets up for the dropdown menu of options
    [Tooltip("What type of arms are these?")] public armIdentifier armType;

    GameObject player;
    GameObject head;
    playerScript playerScript;
    GameObject solidCollider;
    BoxCollider2D solidBoxCollider; // solid collider on the non-player layer

    void Start()
    {
        player = GameObject.Find("Player");
        head = player.transform.Find("Head").gameObject;
        playerScript = player.GetComponent<playerScript>();
        boxCol = gameObject.GetComponent<BoxCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        solidCollider = transform.Find("solidCollider").gameObject;
        solidBoxCollider = solidCollider.GetComponent<BoxCollider2D>();
        platEffect = gameObject.GetComponent<PlatformEffector2D>();
        solidBoxCollider.enabled = true;
        this.name = armType + "Arms";
        CheckForParent();
        unavailableTimer = 0f;
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
    }

    void CheckForParent()
    {
        if(transform.parent != null && transform.parent.tag == "Player")
        {
            Attached();
        }
        else
        {
            Detached();
        }
    }

    public void Detached()
    {
        transform.parent = null;
        solidBoxCollider.enabled = true;
        platEffect.enabled = true;
        attached = false;
        unavailableTimer = 0f;
        rb.isKinematic = false;
        boxColTimer = (playerScript.isGrounded)? 0.2f : 0.4f;
        boxCol.enabled = false;
        gameObject.layer = 13;
    }

    public void Attached()
    {
        attached = true;
        boxCol.enabled = false;
        solidBoxCollider.enabled = false;
        platEffect.enabled = false;
        rb.isKinematic = true;
        gameObject.layer = 0; // switch physics layers so that the player raycast doesn't think it's ground
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Vector2 thisPos = gameObject.transform.position;
        if(col.gameObject.tag == "Player" && (!playerScript.isGrounded || playerScript.partConfiguration == 3) && playerScript.partConfiguration != 2 && playerScript.partConfiguration != 4) // if the player has just legs, snap anyway
        {
            playerScript.armString = this.name;
            int playerParts = playerScript.partConfiguration;
            if(attached == false && playerParts != 2 && playerParts != 4 && unavailableTimer > 0.3f)
            {
                col.gameObject.transform.rotation = Quaternion.identity;
                if(playerParts == 1) // this one needs changing
                {
                    thisPos.y += 0.01f;
                    col.gameObject.transform.position = thisPos;                   
                }
                Attached();
                this.gameObject.transform.parent = col.transform;
                this.gameObject.transform.position = col.transform.position;
                playerScript.UpdateParts();
            }
        }
        else if(col.gameObject.tag == "Player" && playerScript.isGrounded == true)
        // this resets the collider, so that if the player is pushing against it and then jumps, they can still connect
        {
            boxCol.enabled = false;
            Debug.Log("Why");
            boxCol.enabled = true;
        }
        else if(col.gameObject.tag == "Legs")
        {
            // add options for legs to attach here
        }
    }
}