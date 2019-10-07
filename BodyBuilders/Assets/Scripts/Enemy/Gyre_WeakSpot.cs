using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gyre_WeakSpot : MonoBehaviour
{
    Rigidbody2D colRb; // the collider of this object
    GameObject player; // the player gameObject
    [Tooltip("The strength of the bounce off the gyre.")]
    public float bounceStrength; // standard jump force applied

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Rigidbody2D rb2d = col.gameObject.GetComponent<Rigidbody2D>();
            Vector2 velocity = rb2d.velocity;
            velocity.y = bounceStrength;
            rb2d.velocity = velocity;

            Destroy(transform.parent.gameObject);
        }
    }
}