using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheckerHead : MonoBehaviour
{
    public LayerMask groundLayer;
    float groundedDistance = 0.3f;

    void Update()
    {
        if(Input.GetKeyDown("b") == true)
        {
            Jump();
        }
        // Allow the below line of code to check the groundcheck distance
        //Debug.DrawRay(transform.position, Vector2.down * groundedDistance, Color.green);
    }

    void Jump()
    {
        if(!groundCheck())
        {
            return;
        }
        else
        {
            Debug.Log("hit");
        }
    }

    bool groundCheck()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        
        RaycastHit2D hit = Physics2D.Raycast(position, direction, groundedDistance, groundLayer);
        if(hit.collider != null)
        {
            return true;
        }
        return false;
    }
}
