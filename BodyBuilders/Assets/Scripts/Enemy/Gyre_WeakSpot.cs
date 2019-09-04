using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gyre_WeakSpot : MonoBehaviour
{
    public GameObject Station;
    public float normalJumpForce; // standard bounce force applied
    Rigidbody2D colRb; // the collider of this object
    GameObject player; // the player gameObject
    public float jumpForceMultiplier = 1.2f; // increase the amount of bounce force applied if the player is pressing up
    float jumpForce; // the actual force applied (one of the ones above)

    void Start()
    {
        player = GameObject.Find("Player").gameObject;
    }

    void Update()
    {
        jumpForce = normalJumpForce;
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
     
        if (col.gameObject.CompareTag("Station"))
        {
            Station = col.gameObject;
        }
     
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("bounce");
            Rigidbody2D rb2d = col.gameObject.GetComponent<Rigidbody2D>();
            Vector2 velocity = rb2d.velocity;
            velocity.y = jumpForce;
            rb2d.velocity = velocity;
            if (Station != null)
            {
                Station.GetComponent<Gyre_StationScript>().StartSpawn();
            }
            Destroy(transform.parent.gameObject);
        }
    }
}
