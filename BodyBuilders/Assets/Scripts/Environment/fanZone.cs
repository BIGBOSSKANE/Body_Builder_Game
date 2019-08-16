using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fanZone : MonoBehaviour
{
    public float fanForce;
    public float lockHeight;
    public float posY;
    Rigidbody2D colRb;
    GameObject player;

    void Start()
    {
        //lockHeight = transform.position.y + (0.5f * transform.localScale.y);
        player = GameObject.Find("Player").gameObject;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            colRb = player.GetComponent<Rigidbody2D>();

            if(colRb.velocity.y < 0f)
            {
                //colRb.velocity = new Vector2(colRb.velocity.x , Mathf.Clamp((colRb.velocity.y + (Time.deltaTime * 80f)) , 0f , 5f)); // * -colRb.velocity.y));
            }

            if((player.transform.position.y < lockHeight + 0.2f && player.transform.position.y > lockHeight - 0.2f)
                && (colRb.velocity.y < 0.1f && colRb.velocity.y > -0.1f)
                && !(Input.GetAxis("Vertical") > 0f))
            {
                player.transform.position = new Vector2(player.transform.position.x , lockHeight);
            }
            else if(col.transform.position.y > 0.5f * lockHeight)
            {
                colRb.AddForce(Vector2.up * fanForce * 0.5f * (col.transform.position.y / lockHeight));
            }
            else
            {
                colRb.AddForce(Vector2.up * fanForce);
            }
        }
        else
        {
            colRb = col.gameObject.GetComponent<Rigidbody2D>();
            
            if(colRb.velocity.y < 0f)
            {
                colRb.velocity = new Vector2(colRb.velocity.x , Mathf.Clamp((colRb.velocity.y + (Time.deltaTime * 80f)) , 0f , 5f)); // * -colRb.velocity.y));
            }

            if(col.transform.position.y < lockHeight + 0.2f || col.transform.position.y > lockHeight - 0.2f && colRb.velocity.y < 2.5f && colRb.velocity.y > -2.5f)
            {
                col.transform.position = new Vector2(col.transform.position.x , lockHeight);
            }
            else
            {
                colRb.AddForce(Vector2.up * fanForce);
            }
        }

        /*
        Debug.Log(col.transform.position.y);
        if(col.transform.position.y <= (lockHeight))
        {
            Rigidbody2D colRb = col.gameObject.GetComponent<Rigidbody2D>();
            if(colRb.velocity.y < 0.1f && col.transform.position.y > lockHeight - 1f)
            {
                colRb.AddForce(transform.up * fanForce * (col.transform.position.y / lockHeight) * -2f * colRb.velocity.y);
            }
            else
            {
                colRb.AddForce(transform.up * fanForce * (col.transform.position.y / lockHeight));
            }
        }
        */
    }
}