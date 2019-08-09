using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gyre_WeakSpot : MonoBehaviour
{
    public GameObject Station;

    public void OnTriggerEnter2D(Collider2D col)
    {
        {
            if (col.gameObject.CompareTag("Station"))
            {
                Station = col.gameObject;
                Debug.Log("collided");
            }
        }
        if (col.gameObject.CompareTag("Player"))
        {
            Debug.Log("Gyre died");
            if (Station != null)
            {
                Station.GetComponent<Gyre_StationScript>().StartSpawn();
            }
            Destroy(transform.parent.gameObject);
        }
    }
}
