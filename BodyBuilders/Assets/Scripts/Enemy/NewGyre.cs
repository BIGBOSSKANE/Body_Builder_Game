﻿//Created by Kane Girvan
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGyre : MonoBehaviour
{
    public float velocity = 2.5f;

    public Transform sightStart;
    public Transform sightEnd;

    public LayerMask DetectWhat;

    public bool colliding;
    bool collided = false;

    Rigidbody2D rb2d;
    playerScript playerScript;

    public GameObject transformation;

    public float range = 10f;

    public Vector3 pos;

    public bool patrolling = true;

    public float distance;

    public Transform groundDetection;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.Find("Player").gameObject.GetComponent<playerScript>(); // we can swap this out for the scene manager once it has been added
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        patrolling = true;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distance);

        if (patrolling)
        {
            rb2d.velocity = new Vector2(velocity, rb2d.velocity.y);
        }

        colliding = Physics2D.Linecast(sightStart.position, sightEnd.position, DetectWhat);

        if (colliding || groundInfo.collider == false)
        {
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            velocity *= -1;
        }

        if (collided)
        {
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            velocity *= -1;
            collided = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerScript.Die(0.2f);
        }

        if (col.gameObject.tag == "Legs" || col.gameObject.tag == "Arms" || col.gameObject.tag == "Box")
        {
            collided = true;
        }
    }
}