using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animController : MonoBehaviour
{
    public Animator anim; // find the Animator on this object, and name it "anim" for this script.
    
    void Start() // on starting up...
    {   anim.Play("Animation"); // play the animation named "platform up" from the animtor assgined to the object that this script is on.
    }
}