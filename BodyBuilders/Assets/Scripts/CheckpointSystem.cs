using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    private GameObject _currentSpawnPoint;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Respawn")
        {
            _currentSpawnPoint = col.gameObject;
        }
        else if(col.tag == "Death")
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        if (_currentSpawnPoint != null)
        {
            transform.position = _currentSpawnPoint.transform.position;
        }
    }
}