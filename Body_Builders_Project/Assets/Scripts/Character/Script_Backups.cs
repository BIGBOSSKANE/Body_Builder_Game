/*      Legs
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
    CapsuleCollider2D playerCol;
    int playerParts;
    bool taken;
    float headConnectTimer;
    float torsoConnectTimer;

    void Start()
    {
        boxCol = this.GetComponent<BoxCollider2D>();
        rb = this.GetComponent<Rigidbody2D>();
        CheckForParent();
        player = GameObject.Find("Player");
        playerCol = player.GetComponent<CapsuleCollider2D>();
        head = player.transform.Find("Head").gameObject;
        playerScript = player.GetComponent<Player_Controller>();
        playerParts = playerScript.partConfiguration;
        headConnectTimer = 0f;
        torsoConnectTimer = 0f;
    }

    void Update()
    {
        if(unavailableTimer < 1f)
        {
            unavailableTimer += Time.deltaTime;
        }

        if(taken == true) //
        {
            Vector2 targetLoc = new Vector2(transform.position.x , transform.position.y + 0.014f);
            playerCol.enabled = false;
            if(playerParts == 1)
            {
                float headDistance = Vector2.Distance(head.transform.position , targetLoc);
                headConnectTimer += Time.deltaTime;
                player.transform.position = Vector2.Lerp(player.transform.position , targetLoc , headConnectTimer * 2.5f);
                if(headConnectTimer > 0.4f || headDistance < 0.1f)
                {
                    playerScript.legString = identifierLegString;
                    Attached();
                    this.gameObject.transform.parent = player.transform;
                    playerScript.UpdateParts();
                    headConnectTimer = 0f;
                    taken = false;
                    playerCol.enabled = true;
                }            
            }
            else if(playerParts == 2)
            {
                float torsoDistance = Vector2.Distance(player.transform.position , targetLoc);
                torsoConnectTimer += Time.deltaTime;
                player.transform.position = Vector2.Lerp(player.transform.position , targetLoc , headConnectTimer * 2.5f);
                if(torsoConnectTimer > 0.4f || torsoDistance < 0.1f)
                {
                    playerScript.legString = identifierLegString;
                    Attached();
                    this.gameObject.transform.parent = player.transform;
                    playerScript.UpdateParts();
                    torsoConnectTimer = 0f;
                    taken = false;
                    playerCol.enabled = true;
                }
            }
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
        if(col.gameObject.tag == "Player" && playerScript.isGrounded == false)
        {
            if(attached == false && playerParts != 3 && playerParts != 4 && unavailableTimer > 0.3f)
            {
                col.gameObject.transform.rotation = Quaternion.identity;
                taken = true;
                //
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
                playerScript.legString = identifierLegString;
                Attached();
                this.gameObject.transform.parent = col.transform;
                playerScript.UpdateParts();
                //
            }
        }
    }
}

//create a child object with a capsule collider that is on a layer which can't collide with the player
//set this one as a trigger collider
*/