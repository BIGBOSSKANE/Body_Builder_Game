using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gyre_StationScript : MonoBehaviour
{
    public GameObject gyrePrefab;
    public GameObject gyre;
    public GameObject spwnPnt;
    public float spwnTim = 3f;
    public bool isGyreAlive = true;

    public void Start()
    {
        isGyreAlive = true;

        gyrePrefab = gyre;

        if (!isGyreAlive)
        {
            StartCoroutine(SpawnWait());
        }
    }

    public void Update()
    {
        if(gyre != null)
        {
            isGyreAlive = true;
        }
        else
        {
            isGyreAlive = false;
            StartCoroutine( SpawnWait() );
        }
    }

    IEnumerator SpawnWait()
    {
        if (!isGyreAlive)
        {
            yield return new WaitForSeconds(spwnTim);
            spawnGyre();
            yield break;
        }
    }

    void spawnGyre()
    {
        Vector2 position1 = spwnPnt.transform.position;
        Instantiate(gyrePrefab, position1, Quaternion.identity);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Grye")
        {
            gyre = col.gameObject;
        }
    }
}