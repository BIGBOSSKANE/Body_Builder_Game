using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    public bool grabbed;
    RaycastHit2D hit;
    public float distance = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            if(!grabbed)
            {
                //grab
                //may need to reduce y component of transform.position by dividing
                
                //Physics2D.raycastStartInColliders = false;
                hit = Physics2D.Raycast(transform.position, Vector2.right*transform.localScale.x, distance);
            }
            else
            {
                //throw
            }
        }
    }

/*
    void OnDrawGizoms()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position+Vector2.right*transform.localScale.x, distance);
    }
*/
}



/*
currently I have created a box trigger collider for the player controller
this should be set up to flip over the character controller bsed on their position
It should be used for wall climb and jump, and grabbing boxes
    Whilst in the air, it should shrink down a bit, as wall climb is lower range than grabbing
It should only turn on when the player has arms

While in that mode, the character can click the box to child it to their form

*/