using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boxTracker : MonoBehaviour
{
    public LayerMask objectLayerMask;

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Collisions();
    }

    void Collisions()
    {
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position , transform.localScale / 2 , Quaternion.identity , objectLayerMask);
        int i = 0;
        while (i < hitColliders.Length)
        {
            Debug.Log("Hit : " + hitColliders[i].name + i);
            i++; // increase the number of boxes in the array
        }

        //foreach()
    }
}
