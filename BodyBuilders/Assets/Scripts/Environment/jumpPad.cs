using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumpPad : MonoBehaviour
{
    public float jumpForce;
    public float jumpForceMultiplier;
    public bool forceReflection;
    PhysicsMaterial2D bouncyMat;
    Rigidbody2D colRb;
    GameObject player;

    void Start()
    {
        player = GameObject.Find("Player").gameObject;
        if(!forceReflection)
        {
            gameObject.GetComponent<BoxCollider2D>().sharedMaterial = null;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            colRb = player.GetComponent<Rigidbody2D>();
            if(Input.GetAxis("Vertical") > 0f)
            {
                colRb.AddForce(Vector2.up * jumpForce * jumpForceMultiplier, ForceMode2D.Impulse);
            }
            else
            {
                colRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
        else
        {
            colRb = col.gameObject.GetComponent<Rigidbody2D>();
            colRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
