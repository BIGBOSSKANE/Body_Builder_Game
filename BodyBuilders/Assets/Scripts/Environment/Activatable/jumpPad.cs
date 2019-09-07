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
    [Tooltip("Does the jump pad give extra boost if the player is jumping")] public bool jumpBooster = true;
    [Tooltip("Normal bounce force")] public float normalBounceForce; // standard bounce force applied
    [Tooltip("Bounce force while overcharged")] public float overchargeBounceForce; // boosted bounce force applied
    [Tooltip("If jump boost is on, how much higher does the player go?")] public float jumpForceMultiplier = 1.2f; // increase the amount of bounce force applied if the player is pressing up
    float jumpForce; // the actual force applied (one of the ones above)
    [Tooltip("Does the bounce pad reflect the player's vertical velocity?")] public bool forceReflection; // does the player bounce higher when they fall from higher?
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
            jumpForce = overchargeBounceForce;
        }
        else
        {
            jumpForce = normalBounceForce;
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
