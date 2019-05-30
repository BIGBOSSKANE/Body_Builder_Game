/*
Creator: Daniel
Created: 09/04/2019
Laste Edited by: Daniel
Last Edit: 24/05/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head_Scaler : MonoBehaviour
{
    private bool taken = false;
    private float timer = 0f;
    private Vector2 initialPosition;
    private Vector2 targetPosition;
    private Rigidbody2D rigi;
    public string identifierHeadString = "Scaler"; // This is used to change what arms the player controller thinks are connected
    public GameObject player;
    public GameObject head;
    public Player_Controller playerScript;
    public Vector2 headPos;
    float playerDistance;

    void Start()
    {
        rigi = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        head = player.transform.Find("Head").gameObject;
        playerScript = player.GetComponent<Player_Controller>();
        taken = false;
        timer = 0f;
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
                playerScript.headString = identifierHeadString;
                playerScript.UpdateParts();
                Destroy(gameObject);
            }
        }
    }
}