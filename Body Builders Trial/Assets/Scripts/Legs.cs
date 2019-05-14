using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legs : MonoBehaviour
{
    // int movementSpeed = 10;
    // int jumpForce = 10;

    bool attached;
    float unavailableTimer = 1f;
    public BoxCollider2D boxCol;
    public Rigidbody2D rb;

    void Start()
    {
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
        rb.mass = 2;
        rb.isKinematic = false;
    }

    public void Attached()
    {
        attached = true;
        boxCol.enabled = false;
        Destroy(rb);
    }

    void OnCollisionEnter2D(Collision2D player)
    {
        Vector2 thisPos = gameObject.transform.position;
        if(player.gameObject.tag == "Player")
        {
            Player_Controller playerScript = player.gameObject.GetComponent<Player_Controller>();
            int playerParts = playerScript.partConfiguration;
            if(attached == false && playerParts != 3 && playerParts != 4 && unavailableTimer > 0.3f)
            {
                player.gameObject.transform.rotation = Quaternion.identity;
                if(playerParts == 1) // this one needs changing
                {
                    thisPos.y += 0.014f;
                    player.gameObject.transform.position = thisPos;                   
                }
                else if(playerParts == 2)
                {
                    thisPos.y += 0.014f;
                    player.gameObject.transform.position = thisPos;
                }
                Attached();
                this.gameObject.transform.parent = player.transform;
                player.gameObject.GetComponent<Player_Controller>().UpdateParts();
            }
        }
    }

    public void Special()
    {

    }
}