using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Pressure_Pad : MonoBehaviour
{
    public GameObject door;
    public string animationName = "Opening";


    // Update is called once 
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject)
        {
            {
                Debug.Log("collided with button");
                Animator a = door.GetComponent<Animator>();
                if (a != null)
                {
                    a.Play(animationName);
                }
            }
        }
    }
}