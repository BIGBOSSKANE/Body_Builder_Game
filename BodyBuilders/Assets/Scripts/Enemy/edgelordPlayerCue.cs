using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class edgelordPlayerCue : MonoBehaviour
{
    public GameObject edgelord;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            edgelord.GetComponent<edgelord>().waitingForPlayer = false;
            edgelord.SetActive(true);
            Destroy(gameObject);
        }
    }
}
