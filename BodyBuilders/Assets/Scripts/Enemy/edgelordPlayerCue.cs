/*
Creator: Daniel
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class edgelordPlayerCue : MonoBehaviour
{
    public GameObject edgelord;
    public GameObject destroyPrevious;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            if(destroyPrevious != null) Destroy(destroyPrevious);
            edgelord.SetActive(true);
            edgelord.GetComponent<edgelord>().waitingForPlayer = false;
            edgelord.SetActive(true);
            Destroy(gameObject);
        }
    }
}
