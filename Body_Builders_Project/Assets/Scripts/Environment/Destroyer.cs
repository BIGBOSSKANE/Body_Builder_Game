/*
Created by: Daniel
Date Created: 15/05/2019
Last Edited by: Daniel
Last Edited Date: 17/05/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
