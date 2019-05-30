using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeExample : MonoBehaviour
{
    int playerParts;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            playerParts = col.GetComponent<Player_Controller>().partConfiguration;
        }

        if(playerParts == 2 || playerParts == 4)
        {
            // do arm thing here
        }
    }
}
