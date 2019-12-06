/*
Creator: Daniel
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallOnCollision : MonoBehaviour
{
    public bool falling;
    public Vector2 fallPoint;
    public float fallAcceleration;
    float fallSpeed;

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            falling = true;
            Destroy(GetComponent<BoxCollider2D>());
        }
    }

    void Update()
    {
        if(falling)
        {
            fallSpeed += fallAcceleration * Time.deltaTime;
            transform.position -= new Vector3(0 , fallSpeed * Time.deltaTime , 0);
            if(transform.position.y <= fallPoint.y)
            {
                transform.position = fallPoint;
                falling = false;
            }
        }
    }
}
