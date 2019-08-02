using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gyre_WeakSpot : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            Destroy(transform.parent.gameObject);
    }
}
