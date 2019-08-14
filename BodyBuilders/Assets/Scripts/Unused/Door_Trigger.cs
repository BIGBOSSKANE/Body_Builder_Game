using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Trigger : MonoBehaviour
{
    public GameObject door;
    public Animator anim; // find the Animator on this object, and name it "anim" for this script.


    // Update is called once 
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject)
        {
            Debug.Log("Opening");
            anim.Play("Opening"); // play the animation named "platform up" from the animtor assgined to the object that this script is on.
        }
    }
}
