/*
Creator: Daniel
Created: 09/04/2019
Last Edited by: Daniel
Last Edit: 07/09/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arms : MonoBehaviour
{
    bool attached = false;
    float unavailableTimer = 1f;
    BoxCollider2D playerBoxCol; // player pickup collider
    GameObject playerColliderObject;
    float boxColTimer;
    Rigidbody2D rb;
    public enum armIdentifier{Basic, Lifter, Shield} // sets up for the dropdown menu of options
    [Tooltip("What type of arms are these?")] public armIdentifier armType;
    armIdentifier previousArmIdentifier;

    [Tooltip("Make sure each instance of arms in the level has a different number, starting at 1.")] public int instance = 1;
    GameObject player;
    GameObject head;
    playerScript playerScript;
    BoxCollider2D solidBoxCollider; // solid collider on the non-player layer

    [Header("Sprites:")]
    [Tooltip("Basic Arms Sprite")] public Sprite BasicSprite;
    [Tooltip("Lifter Arms Sprite")] public Sprite LifterSprite;
    [Tooltip("Shield Arms Sprite")] public Sprite ShieldSprite;

    SpriteRenderer spriteRenderer;

    void Start()
    {
        player = GameObject.Find("Player");
        head = player.transform.Find("Head").gameObject;
        playerScript = player.GetComponent<playerScript>();
        solidBoxCollider = gameObject.GetComponent<BoxCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        playerColliderObject = gameObject.transform.Find("playerCollider").gameObject;
        playerBoxCol = playerColliderObject.GetComponent<BoxCollider2D>();
        playerBoxCol.enabled = true;

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        SetType();
        CheckForParent();
        playerBoxCol.enabled = true;
        unavailableTimer = 1f;
    }

    public void SetType()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        if(armType == armIdentifier.Basic)
        {
            spriteRenderer.sprite = BasicSprite;
            if(gameObject.transform.Find("shieldBubble") != null) Destroy(gameObject.transform.Find("shieldBubble").gameObject);
            if(gameObject.transform.Find("collisionEffectPosition") != null) Destroy(gameObject.transform.Find("collisionEffectPosition").gameObject);
        }
        else if(armType == armIdentifier.Lifter)
        {
            spriteRenderer.sprite = LifterSprite;
            if(gameObject.transform.Find("shieldBubble") != null) Destroy(gameObject.transform.Find("shieldBubble").gameObject);
            if(gameObject.transform.Find("collisionEffectPosition") != null) Destroy(gameObject.transform.Find("collisionEffectPosition").gameObject);
        }
        else if(armType == armIdentifier.Shield)
        {
            spriteRenderer.sprite = ShieldSprite;
        }

        this.name = armType + "Arms";
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
    }

    void CheckForParent()
    {
        if(transform.parent != null && transform.parent.tag == "Player")
        {
            Attached();
        }
        else
        {
            Detached();
        }
    }

    public void Detached()
    {
        transform.parent = null;
        solidBoxCollider.enabled = true;
        attached = false;
        unavailableTimer = 0f;
        rb.isKinematic = false;
        playerColliderObject.layer = 13;
        boxColTimer = (playerScript.isGrounded)? 0.4f : 0.6f;
        playerBoxCol.enabled = false;
        gameObject.layer = 13;
        playerColliderObject.SetActive(true);
    }

    public void Attached()
    {
        attached = true;
        playerBoxCol.enabled = false;
        solidBoxCollider.enabled = false;
        rb.isKinematic = true;
        gameObject.layer = 0; // switch physics layers so that the player raycast doesn't think it's ground
        playerColliderObject.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Vector2 thisPos = gameObject.transform.position;
        if(col.gameObject.tag == "Player" && (!playerScript.isGrounded || playerScript.partConfiguration == 3) && playerScript.partConfiguration != 2 && playerScript.partConfiguration != 4) // if the player has just legs, snap anyway
        {
            playerScript.armString = this.name;
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
        else if(col.gameObject.tag == "Player" && playerScript.isGrounded == true)
        // this resets the collider, so that if the player is pushing against it and then jumps, they can still connect
        {
            playerBoxCol.enabled = false;
            playerBoxCol.enabled = true;
        }
        else if(col.gameObject.tag == "Legs")
        {
            // add options for legs to attach here
        }
    }



    void OnDrawGizmos()
    {
        if(previousArmIdentifier != armType)
        {
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if(armType == armIdentifier.Basic)
            {
                spriteRenderer.sprite = BasicSprite;
            }
            else if(armType == armIdentifier.Lifter)
            {
                spriteRenderer.sprite = LifterSprite;
            }
            else if(armType == armIdentifier.Shield)
            {
                spriteRenderer.sprite = ShieldSprite;
            }
        }

        previousArmIdentifier = armType;
    }
}