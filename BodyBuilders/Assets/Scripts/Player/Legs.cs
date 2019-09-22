/*
Creator: Daniel
Created: 09/04/2019
Laste Edited by: Daniel
Last Edit: 07/09/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legs : MonoBehaviour
{
    bool attached;
    [HideInInspector] public bool groundBreaker = false; // when ground breakers are dropped, they will continue to do their thing
    float unavailableTimer = 1f;
    BoxCollider2D solidBoxCol;
    Rigidbody2D rb;

    public enum legIdentifier{ Basic, Groundbreaker, Afterburner} // sets up for the dropdown menu of options
    [Tooltip("What type of legs are these?")] public legIdentifier legType;
    legIdentifier previousLegIdentifier;

    [Tooltip("Make sure each instance of legs in the level has a different number, starting at 1.")] public int instance = 1;

    GameObject player;
    GameObject head;
    playerScript playerScript;
    GameObject playerColliderObject;
    BoxCollider2D playerBoxCol;
    timeSlow timeSlowScript;
    float yCastOffset = -0.86f;
    float raycastDistance = 0.17f;
    float maxHeight;
    float groundbreakerDistance;
    LayerMask jumpLayer;
    bool groundBreakerReset;
    float boxColTimer;

    [Header("Sprites:")]
    [Tooltip("Basic Legs Sprite")] public Sprite BasicSprite;
    [Tooltip("Groundbreaker Legs Sprite")] public Sprite GroundbreakerSprite;
    [Tooltip("Afterburner Legs Sprite")] public Sprite AfterburnerSprite;

    SpriteRenderer spriteRenderer;

    void Start()
    {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<playerScript>();
        solidBoxCol = gameObject.GetComponent<BoxCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        timeSlowScript = player.GetComponent<timeSlow>();
        head = player.transform.Find("Head").gameObject;
        jumpLayer = playerScript.jumpLayer;
        playerColliderObject = transform.Find("playerCollider").gameObject;
        playerBoxCol = playerColliderObject.GetComponent<BoxCollider2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        SetType();
        CheckForParent();
        playerBoxCol.enabled = true;
        unavailableTimer = 1f;
    }

    public void SetType()
    {
        if(legType == legIdentifier.Basic)
        {
            spriteRenderer.sprite = BasicSprite;
            if(gameObject.transform.Find("BoostSprites") != null) Destroy(gameObject.transform.Find("BoostSprites").gameObject);
        }
        else if(legType == legIdentifier.Groundbreaker)
        {
            spriteRenderer.sprite = GroundbreakerSprite;
            if(gameObject.transform.Find("BoostSprites") != null) Destroy(gameObject.transform.Find("BoostSprites").gameObject);
        }
        else if(legType == legIdentifier.Afterburner)
        {
            spriteRenderer.sprite = AfterburnerSprite;
        }

        this.name = legType + "Legs";
    }

    void Update()
    {
        if(unavailableTimer < 1f)
        {
            unavailableTimer += Time.deltaTime;
        }

        if(boxColTimer > 0f)
        {
            boxColTimer -= Time.deltaTime;
        }
        else if(!attached)
        {
            playerBoxCol.enabled = true;
            boxColTimer = 0f;
            playerColliderObject.layer = 18;
        }

        if(transform.position.y <= (maxHeight - groundbreakerDistance))
        {
            if(groundBreaker && !groundBreakerReset)
            {
                timeSlowScript.TimeSlow();
                groundBreakerReset = true;
            }
        }

        if(groundBreaker == true)
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + yCastOffset), Vector2.down, raycastDistance, jumpLayer);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + yCastOffset), Vector2.down * raycastDistance, Color.red);

            if(hit.collider != null)
            {
                if(transform.position.y <= (maxHeight - groundbreakerDistance))
                {
                    if(hit.collider.gameObject.tag == "Groundbreakable")
                    {
                        hit.collider.gameObject.GetComponent<Groundbreakable_Script>().Groundbreak();
                    }
                }
                else
                {
                    maxHeight = transform.position.y;
                    groundBreakerReset = false;
                    groundBreaker = false;
                }
                timeSlowScript.TimeNormal();
            }
        }
    }

    void CheckForParent()
    {
        if(transform.parent != null && transform.parent.tag == "Player")
        {
            Attached();
        }
        else
        {
            Detached(0f , 0f);
        }
    }

    public void Detached(float maxHeightCalled , float groundbreakerDistanceCalled)
    {
        maxHeight = maxHeightCalled;
        groundbreakerDistance = groundbreakerDistanceCalled;
        transform.parent = null;
        solidBoxCol.enabled = true;
        playerBoxCol.enabled = true;
        attached = false;
        unavailableTimer = 0f;
        rb.isKinematic = false;
        gameObject.layer = 13;
        playerColliderObject.SetActive(true);
        if(gameObject.name == "GroundbreakerLegs")
        {
            groundBreaker = true;
        }
    }

    public void Attached()
    {
        attached = true;
        solidBoxCol.enabled = false;
        playerBoxCol.enabled = false;
        rb.isKinematic = true;
        playerColliderObject.SetActive(false);
        gameObject.layer = 0; // switch physics layers so the player raycast doesn't think it's ground
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Vector2 thisPos = gameObject.transform.position;
        if(col.gameObject.tag == "Player" && playerScript.partConfiguration != 2 && playerScript.isGrounded == true) // reset the collider if the player is not jumping
        {
            playerBoxCol.enabled = false;
            playerBoxCol.enabled = true;
        }
        else if(col.gameObject.tag == "Player" && (!playerScript.isGrounded || playerScript.partConfiguration == 2) && playerScript.partConfiguration != 3 && playerScript.partConfiguration != 4)
        {
            playerScript.legString = this.name;
            int playerParts = playerScript.partConfiguration;
            if(attached == false && playerParts != 3 && playerParts != 4 && unavailableTimer > 0.3f)
            {
                col.gameObject.transform.rotation = Quaternion.identity;
                if(playerParts == 1)
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

    void OnDrawGizmos()
    {
        if(previousLegIdentifier != legType)
        {
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if(legType == legIdentifier.Basic)
            {
                spriteRenderer.sprite = BasicSprite;
            }
            else if(legType == legIdentifier.Groundbreaker)
            {
                spriteRenderer.sprite = GroundbreakerSprite;
            }
            else if(legType == legIdentifier.Afterburner)
            {
                spriteRenderer.sprite = AfterburnerSprite;
            }
        }

        previousLegIdentifier = legType;
    }
}