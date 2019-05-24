using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arms : MonoBehaviour
{
    // int movementSpeed = 10;
    // int jumpForce = 10;

    bool attached = false;
    float unavailableTimer = 1f;
    public BoxCollider2D boxCol;
    public Rigidbody2D rb;
    public string identifierArmString = "Basic"; // This is used to change what arms the player controller thinks are connected

    public GameObject player;
    public GameObject head;
    public Player_Controller playerScript;
    public Vector2 headPos;
    float playerDistance;

    Vector2 snapPoint;
    float snappingToLegsTimer = 0f;
    float snappingToLegsDistance;
    bool snapToLegs = false;
    bool snappingToLegs = false;

    void Start()
    {
        player = GameObject.Find("Player");
        head = player.transform.Find("Head").gameObject;
        playerScript = player.GetComponent<Player_Controller>();
        boxCol = this.GetComponent<BoxCollider2D>();
        rb = this.GetComponent<Rigidbody2D>();
        CheckForParent();
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

    void OnCollisionEnter2D(Collision2D col) // try to change it to OnTriggerEnter2D
    {
        Vector2 thisPos = gameObject.transform.position;
        if(col.gameObject.tag == "Player" && playerScript.isGrounded == false)
        {
            playerScript.armString = identifierArmString;
            int playerParts = playerScript.partConfiguration;
            if(attached == false && playerParts != 2 && playerParts != 4 && unavailableTimer > 0.3f)
            {
                col.gameObject.transform.rotation = Quaternion.identity;
                if(playerParts == 1) // this one needs changing
                {
                    thisPos.y += 0.01f;
                    col.gameObject.transform.position = thisPos;                   
                }
                Attached();
                this.gameObject.transform.parent = col.transform;
                this.gameObject.transform.position = col.transform.position;
                playerScript.UpdateParts();
            }
        }
        else if(col.gameObject.tag == "Legs")
        {

        }
    }
}

/*

void Update()
{
    if(unavailableTimer < 1f)
    {
        unavailableTimer += Time.deltaTime;
    }


    if(snappingToLegs == true && snappingToLegsTimer < 0.5f && snappingToLegsDistance > 0.2f)
    {
        snappingToLegsTimer += Time.deltaTime;
        snappingToLegsDistance = Vector2.Distance(transform.position , snapPoint);
        transform.position = Vector2.Lerp(transform.position , snapPoint , snappingToLegsTimer * 4f);
    }
    else 
    {
        transform.position = snapPoint;
        snapToLegs = true;
a
}


void OnTriggerEnter2D(Collider2D col)
{
    if(col.gameObject.tag == "Player)"
    {
        playerScript.armString = identifierArmString;
        if(attached == false && playerParts != 2 && playerParts != 4 && unavailableTimer > 0.3f)
    }
    else if(col.gameObject.tag == "legs")
    {
        snapPoint = col.transform.Find("ArmLock").gameObject.transform.position;
        snappingToLegs = true;
        snapToLegs = true;
        snappingToLegsTimer = 0f;
    }
}


*/