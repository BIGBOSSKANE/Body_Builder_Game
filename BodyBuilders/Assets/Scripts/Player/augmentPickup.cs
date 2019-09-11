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
    public enum headIdentifier{ Basic, Scaler, Hookshot , Headbanger}
    [Tooltip("What type of head augment is this?")] public headIdentifier headType;
    string headString;
    private GameObject player;
    private GameObject head;
    private playerScript playerScript;
    private Vector2 headPos;
    private GameObject secondSprite;
    private float playerDistance;

    void Start()
    {
        rigi = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        head = player.transform.Find("Head").gameObject;
        playerScript = player.GetComponent<playerScript>();
        headString = headType + "Head";
        this.name = headString;
        taken = false;
        timer = 0f;

        if(gameObject.transform.Find("SecondSprite") != null)
        {
            secondSprite = gameObject.transform.Find("SecondSprite").gameObject;
        }
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
                playerScript.UpdateParts();
                Destroy(gameObject);
            }
        }
        if(headString != "HookshotHead")
        {
            gameObject.transform.Rotate(Vector3.forward * 30f * Time.deltaTime);
        }

        if(secondSprite != null)
        {
            secondSprite.transform.Rotate(Vector3.forward * 60f * -Time.deltaTime);
        }
    }
}