
// currently there is still a lot of tuning to be done
    // potentially get each object that enters the area, store its information, and set its course to be a sine wave with shortening amplitude in each wave
    // this would require a loop

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
        player = GameObject.Find("Player").gameObject;
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            colRb = player.GetComponent<Rigidbody2D>();

            if(colRb.velocity.y < 0f)
            {
                colRb.velocity = new Vector2(colRb.velocity.x , Mathf.Clamp((colRb.velocity.y + (Time.deltaTime * 10f)) , 0f , 5f)); // * -colRb.velocity.y));
            }

            if((player.transform.position.y < lockHeight + 0.2f && player.transform.position.y > lockHeight - 0.2f)
                && (colRb.velocity.y < 0.2f && colRb.velocity.y > -0.2f)
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
            
            if(colRb != null && colRb.velocity.y < 0f)
            {
                colRb.velocity = new Vector2(colRb.velocity.x , Mathf.Clamp((colRb.velocity.y + (Time.deltaTime * 80f)) , 0f , 5f));
            }

            if(colRb != null)
            {
                if((col.transform.position.y < lockHeight + 0.2f || col.transform.position.y > lockHeight - 0.2f) && colRb.velocity.y < 2.5f && colRb.velocity.y > -2.5f)
                {
                    col.transform.position = new Vector2(col.transform.position.x , lockHeight);
                }
                else
                {
                    colRb.AddForce(Vector2.up * fanForce);
                }
            }
        }
    }
}