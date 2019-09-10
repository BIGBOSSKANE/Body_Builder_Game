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


    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        startPos = gameObject.transform.position;
        playerScript = GameObject.Find("Player").gameObject.GetComponent<playerScript>(); // we can swap this out for the scene manager once it has been added
    }

    // Update is called once per frame
    void Update()
    {
        if (chasePlayer)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, startPos, speed * Time.deltaTime);
        }
    }


    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerScript.Respawn(0.2f);
        }
    }
}
