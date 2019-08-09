using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gyre_StationScript : MonoBehaviour
{
    public GameObject gyrePrefab;
    public GameObject spwnPnt;
    public float spwnTim = 3f;

    public void StartSpawn()
    {
        Debug.Log("spawning");
        StartCoroutine(SpawnWait());
    }

    IEnumerator SpawnWait()
    {
        yield return new WaitForSeconds(spwnTim);
        spawnGyre();
        yield break;
    }

    void spawnGyre()
    {
        Vector2 position1 = spwnPnt.transform.position;
        Instantiate(gyrePrefab, position1, Quaternion.identity);
        Debug.Log("spawned");
    }
}