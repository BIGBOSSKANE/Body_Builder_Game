using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head_Scaler : MonoBehaviour
{
    public CircleCollider2D cirCol;
    private bool taken = false;
    private float timer = 0f;
    private Vector2 initialPosition;
    private Vector2 targetPosition;
    public string identifierHeadString = "Scaler"; // This is used to change what arms the player controller thinks are connected

    void Start()
    {
        cirCol = this.GetComponent<CircleCollider2D>();
        taken = false;
        timer = 0f;
        initialPosition = transform.position;
    }

    void OnCollisionEnter2D(Collision2D player)
    {
        Vector2 thisPos = gameObject.transform.position;
        if(player.gameObject.tag == "Player")
        {
            Player_Controller playerScript = player.gameObject.GetComponent<Player_Controller>();
            targetPosition = player.gameObject.transform.position;
            playerScript.headString = identifierHeadString;
            player.gameObject.GetComponent<Player_Controller>().UpdateParts();
            taken = true;
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if(taken == true)
        {
            timer += Time.deltaTime;
            transform.position = Vector2.Lerp(initialPosition, targetPosition, timer);
            if(timer > 1f)
            {
                Destroy(gameObject);
            }
        }
    }
}