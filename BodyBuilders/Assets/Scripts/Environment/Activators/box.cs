using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class box : MonoBehaviour
{
    playerSound playerSound;
    bool started = false;
    public bool grounded = true;
    public float airTimer = 0f;

    void Start()
    {
        playerSound = GameObject.Find("Player").GetComponent<playerSound>();
    }

    void Update()
    {
        if(!grounded)
        {
            airTimer += Time.deltaTime;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(!grounded && started && airTimer > 0.3f && col.gameObject.tag != "Player")
        {
            playerSound.DropPlay();
        }
        airTimer = 0f;
        started = true;
        grounded = true;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if(col.gameObject.tag != "Player")
        {
            grounded = false;   
        }
    }
}
