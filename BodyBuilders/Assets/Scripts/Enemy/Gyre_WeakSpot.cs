using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gyre_WeakSpot : MonoBehaviour
{
    Rigidbody2D colRb; // the collider of this object

    GameObject player; // the player gameObject
    public GameObject parent;

    [Tooltip("The strength of the bounce off the gyre.")]
    public float bounceStrength; // standard jump force applied

    public GameObject station;
    bool isStationed;

    void Start()
    {
        isStationed = false;
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Station")
        {
            station = col.gameObject;
            isStationed = true;
        }

        if (col.gameObject.tag == "Player")
        {
            Rigidbody2D rb2d = col.attachedRigidbody;
            Vector2 velocity = rb2d.velocity;
            velocity.y = bounceStrength;
            rb2d.velocity = velocity;
        }
    }

    public void Destruct()
    {
        Debug.Log("Destruct");
        if (isStationed)
        {
            station.GetComponent<Gyre_StationScript>().isGyreAlive = false;
        }

        Destroy(parent);
    }
}