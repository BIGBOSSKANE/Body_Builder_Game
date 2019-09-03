/*
Creator: Daniel
Created 09/08/2019
Last Edited by: Daniel
Last Edit 31/08/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumpPad : activate
{
    public bool jumpBooster = true;
    public float normalJumpForce; // standard bounce force applied
    public float overchargeJumpForce; // boosted bounce force applied
    public float jumpForceMultiplier = 1.2f; // increase the amount of bounce force applied if the player is pressing up
    float jumpForce; // the actual force applied (one of the ones above)
    public bool forceReflection; // does the player bounce higher when they fall from higher?
    PhysicsMaterial2D bouncyMat; // the physics material allowing objects to add their own velocity to bounces
    Rigidbody2D colRb; // the collider of this object
    GameObject player; // the player gameObject

    void Start()
    {
        player = GameObject.Find("Player").gameObject;
        if(!forceReflection) // disable the bouncy physics material to prevent velocity reflection if force refelction is off
        {
            gameObject.GetComponent<BoxCollider2D>().sharedMaterial = null;
        }
        if(!jumpBooster)
        {
            jumpForceMultiplier = 1f;
        }
    }

    void Update()
    {
        if(overcharge) // when overcharge is on, use the overcharge jump force, otherwise, use the normal one
        {
            jumpForce = overchargeJumpForce;
        }
        else
        {
            jumpForce = normalJumpForce;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(!activated) // if not activated, do nothing
        {
            return;
        }

        if(col.gameObject.tag == "Player")
        {
            player.GetComponent<playerScript>().forceSlaved = true;
            colRb = player.GetComponent<Rigidbody2D>();
            if(Input.GetAxis("Vertical") > 0f) // if the player is holding up, provide a larger jump boost
            {
                colRb.AddForce(Vector2.up * jumpForce * jumpForceMultiplier, ForceMode2D.Impulse);
            }
            else // apply a generic force upwards
            {
                colRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
        else // provide a generic force upwards
        {
            colRb = col.gameObject.GetComponent<Rigidbody2D>();
            colRb.AddForce(Vector2.up * jumpForce / 2f, ForceMode2D.Impulse);
        }
    }
}
