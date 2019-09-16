/*
Creator: Daniel
Created: 09/04/2019
Laste Edited by: Daniel
Last Edit: 07/09/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class augmentPickup : MonoBehaviour
{
    private bool taken = false;
    private float timer = 0f;
    private Vector2 initialPosition;
    private Vector2 targetPosition;
    private Rigidbody2D rigi;
    public enum augmentIdentifier{Scaler, Hookshot}
    [Tooltip("What type of head augment is this?")] public augmentIdentifier augmentType;
    augmentIdentifier previousaugmentIdentifier;
    [Tooltip("Assign a unique number for each instance of an augment, starting at 1")] public int instance;
    string headString;
    private GameObject player;
    private GameObject head;
    private playerScript playerScript;
    private Vector2 headPos;
    private GameObject secondSprite;
    private float playerDistance;
    private SpriteRenderer spriteRenderer;

    [Header("Sprites:")]
    [Tooltip("Scaler Augment")] public Sprite scalerSprite;
    public float scalerSize = 0.2246535f;
    [Tooltip("Hookshot Augment")] public Sprite hookshotSprite;
    public float hookshotSize = 0.30431f;

    void Start()
    {
        rigi = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        player = GameObject.Find("Player");
        head = player.transform.Find("Head").gameObject;
        playerScript = player.GetComponent<playerScript>();
        headString = augmentType + "Head";
        this.name = headString;
        taken = false;
        timer = 0f;
        SetType();

        if(gameObject.transform.Find("SecondSprite") != null)
        {
            secondSprite = gameObject.transform.Find("SecondSprite").gameObject;
        }
    }

    public void SetType()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        if(augmentType == augmentIdentifier.Scaler)
        {
            spriteRenderer.sprite = scalerSprite;
            if(gameObject.transform.Find("SecondSprite") != null) Destroy(gameObject.transform.Find("SecondSprite").gameObject);
        }
        else if(augmentType == augmentIdentifier.Hookshot)
        {
            spriteRenderer.sprite = hookshotSprite;
        }
        this.name = augmentType + "Head";
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            taken = true;
        }
    }

    void Update()
    {
        if(taken == true)
        {
            headPos = head.transform.position;
            playerDistance = Vector2.Distance(headPos , transform.position);
            timer += Time.deltaTime;
            transform.position = Vector2.Lerp(transform.position, headPos, timer * 2.5f);
            if(timer > 0.4f || playerDistance < 0.1f)
            {
                playerScript.headString = headString;
                if(augmentType == augmentIdentifier.Scaler) playerScript.augmentScalerIdentifier = instance;
                if(augmentType == augmentIdentifier.Hookshot) playerScript.augmentHookshotIdentifier = instance;
                playerScript.UpdateParts();
                Destroy(gameObject);
            }
        }
        if(augmentType == augmentIdentifier.Scaler)
        {
            gameObject.transform.Rotate(Vector3.forward * 30f * Time.deltaTime);
        }
        else if(augmentType == augmentIdentifier.Hookshot)
        {
            secondSprite.transform.Rotate(Vector3.forward * 60f * -Time.deltaTime);
        }
    }

    void OnDrawGizmos()
    {
        if(previousaugmentIdentifier != augmentType)
        {
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if(augmentType == augmentIdentifier.Scaler)
            {
                spriteRenderer.sprite = scalerSprite;
                gameObject.transform.localScale = new Vector3(scalerSize , scalerSize , scalerSize);
            }
            else if(augmentType == augmentIdentifier.Hookshot)
            {
                spriteRenderer.sprite = hookshotSprite;
                gameObject.transform.localScale = new Vector3(hookshotSize , hookshotSize , hookshotSize);
            }
        }

        previousaugmentIdentifier = augmentType;
    }
}