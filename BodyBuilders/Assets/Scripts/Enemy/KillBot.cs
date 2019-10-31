using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBot : MonoBehaviour
{
    public float speed = 5.0f;
    public GameObject BBArea;
    public bool chasePlayer = false;
    playerScript playerScript;
    Vector2 startPos;
    Transform target;
    Rigidbody2D rb2D;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        startPos = gameObject.transform.position;
        playerScript = GameObject.Find("Player").gameObject.GetComponent<playerScript>(); // we can swap this out for the scene manager once it has been added
        rb2D = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (chasePlayer)
        {
            //rb2D.AddForce(target.position - transform.position * speed);
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        else
        {
            //rb2D.AddForce((startPos) - transform.position * speed);
            transform.position = Vector2.MoveTowards(transform.position, startPos, speed * Time.deltaTime);
        }
    }


    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerScript.Die(0.2f);
        }
    }
}
