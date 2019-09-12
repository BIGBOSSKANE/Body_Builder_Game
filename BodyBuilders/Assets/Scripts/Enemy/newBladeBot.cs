using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// enemy is like a multi directional Thwomp that patrols
public class newBladeBot : MonoBehaviour
{
    public Vector2 startPos;
    public bool playerDetect;
    playerScript playerScript;
    public LineRenderer lR;
    public bool verticalDetect;
    public bool horizontalDetect;
    string direction;
    Rigidbody2D rb2D;

    // Start is called before the first frame update
    void Start()
    {
        startPos = gameObject.transform.position;
        playerScript = GameObject.Find("Player").gameObject.GetComponent<playerScript>(); // we can swap this out for the scene manager once it has been added
        lR = gameObject.GetComponent<LineRenderer>();
        rb2D = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (horizontalDetect)
        {
            RaycastHit2D hitR = Physics2D.Raycast(startPos, Vector2.right);
            RaycastHit2D hitL = Physics2D.Raycast(startPos, Vector2.left);

            if (hitR.collider.tag == "Player")
            {
                playerDetect = true;
                direction = "right";
            }

            if (hitL.collider.tag == "Player")
            {
                playerDetect = true;
                direction = "left";
            }
        }

        if (verticalDetect)
        {
            RaycastHit2D hitU = Physics2D.Raycast(startPos, Vector2.up);
            RaycastHit2D hitD = Physics2D.Raycast(startPos, Vector2.down);

            if (hitU.collider.tag == "Player")
            {
                playerDetect = true;
                direction = "up";
            }

            if (hitU.collider.tag == "Player")
            {
                playerDetect = true;
                direction = "down";
            }
        }

    }

    void OnCollisionEnter2d(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerScript.Die(0.2f);
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("Groundbreakable"))
        {
            Destroy(col.gameObject);
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            // stop for a moment then return to starting position
            // set direction to null
        }
    }
}