using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boxTracker : MonoBehaviour
{
    /*
        Use a public callable function, which creates an array of things on the object layer with Physics2D.OverlapBox
        In the same function, reference the array and return each game object
        Compare their distance from the boxHoldPos (which is the transform of this object)
        The function then returns the closest to the player
        
        This way the box checker will only be referenced when the player goes to pick one up
    

    public GameObject boxAssigner()
    {
        
    }

    */

    /*
    public Transform boxHoldPos;
    public LayerMask objectLayerMask;
    public Vector3 boxPos;
    public string boxes;

    void Start()
    {
        
    }

    void Update()
    {
        Collisions();
    }

    void OnTriggerEnter2D()
    {
        Collider[] hitColliders = Physics2D.OverlapBox(gameObject.transform.position , transform.localScale / 2 , Quaternion.identity , objectLayerMask);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            Debug.Log("Hit : " + hitColliders[i].name + i);
            boxPos[i] = hitColliders[i].gameObject.transform.position;
            boxes[i] = hitColliders[i].name;
        }
    }
    */
}

/*
    create an array from on trigger enter
    an array list (called that in Java)
    remove specific ones from the array list using OnTriggerExit2D
    use a forloop on pickup call, and find which one is the closest
*/