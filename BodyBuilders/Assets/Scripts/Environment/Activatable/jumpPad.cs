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
    // [Tooltip("Does the bounce pad reflect the player's vertical velocity?")] public bool forceReflection; // does the player bounce higher when they fall from higher?
    Rigidbody2D colRb; // the collider of this object
    GameObject player; // the player gameObject
    playerScript playerScript;

    void Start()
    {
        player = GameObject.Find("Player").gameObject;
        playerScript = player.GetComponent<playerScript>();

        /* if(!forceReflection) */ gameObject.GetComponent<BoxCollider2D>().sharedMaterial = null;

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

        Vector3 collisionNormal = col.contacts[0].normal;

        Vector2 bounceDirection;

        if(collisionNormal.y > 0.5f || collisionNormal.y < -0.5f)
        {
            bounceDirection = new Vector2( 0f , -Mathf.Sign(collisionNormal.y));
        }
        else
        {
            return;
        }

        if(col.gameObject.tag == "Player")
        {
            colRb = col.rigidbody;
            colRb.velocity = new Vector2(colRb.velocity.x * 0.9f , colRb.velocity.y);
            playerScript.forceSlaved = true;

            if(Input.GetAxis("Vertical") > 0f) // if the player is holding up, provide a larger jump boost
            {
                colRb.AddForce(bounceDirection * jumpForce * jumpForceMultiplier, ForceMode2D.Impulse);
            }
            else // apply a generic force upwards
            {
                colRb.AddForce(bounceDirection * jumpForce, ForceMode2D.Impulse);
            }

            if(bounceDirection.y > 0)
            {
                playerScript.jumpGate = true;
                playerScript.jumpGateTimer = 0f;
                playerScript.remainingJumps = playerScript.maximumJumps - 1;
            }
        }
        else if(col.rigidbody != null) // provide a generic force upwards
        {
            colRb = col.rigidbody;
            colRb.velocity = new Vector2(colRb.velocity.x * 0.9f , colRb.velocity.y);
            colRb.AddForce(bounceDirection * jumpForce / 2f, ForceMode2D.Impulse);
        }
    }
}
