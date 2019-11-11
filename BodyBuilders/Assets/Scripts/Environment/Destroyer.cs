/*
Created by: Daniel
Date Created: 15/05/2019
Last Edited by: Daniel
Last Edited Date: 16/10/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            GameObject.Find("Player").GetComponent<playerScript>().Die(0.2f);
            GetComponent<Collider2D>().enabled = false;
        }

        if(col.gameObject.tag == "Groundbreakable")
        {
            col.gameObject.GetComponent<groundbreakable>().Groundbreak();
        }
    }
}
