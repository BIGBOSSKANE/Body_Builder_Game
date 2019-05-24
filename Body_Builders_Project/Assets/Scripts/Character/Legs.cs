using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legs : MonoBehaviour
{
    bool attached;
    float unavailableTimer = 1f;
    public BoxCollider2D boxCol;
    public Rigidbody2D rb;
    public string identifierLegString = "Basic";
    public GameObject player;
    public GameObject head;
    public Player_Controller playerScript;

    void Start()
    {
        boxCol = this.GetComponent<BoxCollider2D>();
        rb = this.GetComponent<Rigidbody2D>();
        CheckForParent();
        player = GameObject.Find("Player");
        head = player.transform.Find("Head").gameObject;
        playerScript = player.GetComponent<Player_Controller>();
    }

    void Update()
    {
        if(unavailableTimer < 1f)
        {
            unavailableTimer += Time.deltaTime;
        }
    }

    void CheckForParent()
    {
        if(transform.parent == null)
        {
            Detached();
        }
        else
        {
            Attached();
        }
    }

    public void Detached()
    {
        attached = false;
        unavailableTimer = 0f;
        boxCol.enabled = true;
        if(rb == null)
        {
            Rigidbody2D Rigidbody2D = this.gameObject.AddComponent<Rigidbody2D>();
        }
        rb = this.GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;   
        rb.mass = 6f;
        rb.drag = 3f;
        rb.gravityScale = 3f;
        rb.isKinematic = false;
    }

    public void Attached()
    {
        attached = true;
        boxCol.enabled = false;
        Destroy(rb);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Vector2 thisPos = gameObject.transform.position;
        if(col.gameObject.tag == "Player" && playerScript.isGrounded == false)
        {
            playerScript.legString = identifierLegString;
            int playerParts = playerScript.partConfiguration;
            if(attached == false && playerParts != 3 && playerParts != 4 && unavailableTimer > 0.3f)
            {
                col.gameObject.transform.rotation = Quaternion.identity;
                if(playerParts == 1) // this one needs changing
                {
                    thisPos.y += 0.014f;
                    col.gameObject.transform.position = thisPos;                   
                }
                else if(playerParts == 2)
                {
                    thisPos.y += 0.014f;
                    col.gameObject.transform.position = thisPos;
                }
                Attached();
                this.gameObject.transform.parent = col.transform;
                playerScript.UpdateParts();
            }
        }
    }
}

//create a child object with a capsule collider that is on a layer which can't collide with the player
//set this one as a trigger collider

//