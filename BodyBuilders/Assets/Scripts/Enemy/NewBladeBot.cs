//https://www.youtube.com/watch?v=BePP-jtrs9U
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBladeBot : MonoBehaviour
{
    public float speed = 5.0f; 
    public GameObject BBArea;
    public bool chasePlayer = false;
    Transform startpos;


    // Start is called before the first frame update
    void Start()
    {
      // set initial starting point  
    }

    // Update is called once per frame
    void Update()
    {
        if (chasePlayer)
        {
            // follow player
        }
        else
        {
            // return to start position
        }
    }


    void OnCollisionEnter2D(Collider2D col)
    {
      // when collided with player
      // respawn player
    }
}
