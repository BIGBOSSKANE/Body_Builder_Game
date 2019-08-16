using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fanZone : MonoBehaviour
{
    public float fanForce;
    float lockHeight;
    float forceAngle;

    void Start()
    {
        lockHeight = transform.position.y + (0.5f * transform.localScale.y);
        forceAngle = transform.parent.transform.rotation.z;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if(col.transform.position.y <= (lockHeight))
        {
            // col.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0f; // didn't work
            col.gameObject.transform.position = new Vector2(col.gameObject.transform.position.x , col.gameObject.transform.position.y + Time.deltaTime);
        }
    }
}