//Created by Kane Girvan
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
        Vector2 spwnPos = spwnPnt.transform.position;
        Instantiate(gyrePrefab, spwnPos, Quaternion.identity);
    }
}