using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Animatior2 : MonoBehaviour
{
    public Animator anim; // find the Animator on this object, and name it "anim" for this script.

    void Awake() // on starting up...
    {
        Debug.Log("Closing");
        anim.Play("Closing"); // play the animation named "platform up" from the animtor assgined to the object that this script is on.
    }
}