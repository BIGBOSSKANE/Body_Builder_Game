using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sawblade : MonoBehaviour
{
    public float spinSpeed = 200f;

    void Update()
    {
        transform.Rotate(0f , 0f , spinSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player") col.attachedRigidbody.gameObject.GetComponent<playerScript>().Die(0.75f);
    }
}
