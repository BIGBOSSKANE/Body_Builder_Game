using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gyre_StationScript : MonoBehaviour
{
    public GameObject gyrePrefab;

    public GameObject spwnPnt;

    public float spwnTim = 3f;

    public bool isGyreAlive = true;

    public void Start()
    {
        isGyreAlive = true;
    }

    public void Update()
    {
        if(!isGyreAlive)
        {
            StartCoroutine( SpawnWait() );
            isGyreAlive = true;
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
}