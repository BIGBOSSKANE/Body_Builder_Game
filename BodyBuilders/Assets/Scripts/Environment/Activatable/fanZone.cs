/*
Creator: Daniel
Created 09/08/2019
Last Edited by: Daniel
Last Edit 29/08/2019
*/

// potentially get each object that enters the area, store its information, and set its course to be a sine wave with shortening amplitude in each wave
// this would require a loop

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fanZone : activate
{
    public float standardFanForce = 20f; // force the fan applies when not overcharged
    public float overchargeFanForce = 100f; // force the fan applies when overcharged
    float fanForce; // force the fan applies
    public float lockHeight; // height at which objects settle at
    public float lockHeightBounds = 0.2f; // if the player stays within these bounds of the lock height, lock their height
    public float lockVelocityBounds = 0.25f; // if the player is moving beneath this speed, it can be locked
    Rigidbody2D colRb; // the rigidbodies of effected objects
    GameObject player; // the player game object

    void Start()
    {
        player = GameObject.Find("Player").gameObject;
    }
    
    void OnTriggerStay2D(Collider2D col)
    {
        if(!activated)
        {
            return;
        }

        fanForce = (overcharge)? overchargeFanForce : standardFanForce;

        if(col.tag == "Player")
        {
            colRb = player.GetComponent<Rigidbody2D>();

            if(colRb.velocity.y < 0f) // if moving down, reduce velocity
            {
                colRb.velocity = new Vector2(colRb.velocity.x , Mathf.Clamp((colRb.velocity.y + (Time.deltaTime * 10f)) , 0f , 5f));
            }

            if((player.transform.position.y < lockHeight + lockHeightBounds && player.transform.position.y > lockHeight - lockHeightBounds) // lock height if within range and moving slow enough
                && (colRb.velocity.y < lockVelocityBounds && colRb.velocity.y > -lockVelocityBounds)
                && !(Input.GetAxis("Vertical") > 0f))
            {
                player.transform.position = new Vector2(player.transform.position.x , lockHeight);
            }
            else if(col.transform.position.y > 0.5f * lockHeight) // reduce fanForce as the player approaches the lock height
            {
                colRb.AddForce(Vector2.up * fanForce * 0.5f * (col.transform.position.y / lockHeight));
            }
            else
            {
                colRb.AddForce(Vector2.up * fanForce); // fan force is strongest at the base
            }
        }
        else
        {
            colRb = col.gameObject.GetComponent<Rigidbody2D>();
            
            if(colRb != null && colRb.velocity.y < 0f) // if moving down, reduce velocity
            {
                colRb.velocity = new Vector2(colRb.velocity.x , Mathf.Clamp((colRb.velocity.y + (Time.deltaTime * 80f)) , 0f , 5f));
            }

            if(colRb != null) // apply force or lock if moving slow enough within the lock range
            {
                if((col.transform.position.y < lockHeight + lockHeightBounds || col.transform.position.y > lockHeight - lockHeightBounds) && colRb.velocity.y < lockVelocityBounds && colRb.velocity.y > lockVelocityBounds)
                {
                    col.transform.position = new Vector2(col.transform.position.x , lockHeight);
                }
                else
                {
                    colRb.AddForce(Vector2.up * fanForce);
                }
            }
        }
    }
}