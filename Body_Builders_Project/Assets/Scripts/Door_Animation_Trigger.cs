using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Animation_Trigger : MonoBehaviour
{
    public GameObject Door;
    public Door_Animation_Catcher doorAnimCatch;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject)
        {
            Debug.Log("collided with button");
           // Animation = Door.GetComponent();
        }
    }
}
