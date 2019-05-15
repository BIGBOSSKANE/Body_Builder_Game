/* By Kane Girvan
 * Body_Builders
 * Quits and restarts game
 * 13/05/19
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart_and_Quit : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape"))
        { //when you press escape
            Debug.Log("Quit the game");
            Application.Quit(); // close the game
        }
        if (Input.GetKey("r"))
        { //when you press the R key
            Debug.Log("Restart the game");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reset the game
        }
    }
}