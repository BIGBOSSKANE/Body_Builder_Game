/*Author Kane Girvan
 * 15/08/19
 * Pause system
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // this is allows you to use certain classes from SceneManagement (it's used to navigate different scenes in this game project(that is set in the build settings)

public class Pause : MonoBehaviour
{ // fields  V  can be set to private or public to be seen in the inspector, on the script. (you don't need to write private, you can even have it as "bool isPaused = true;" and it will automatically be private)
    private bool isPaused = false; // a private(cannot be changed outside the script) on off switch called isPaused
    public GameObject pausePanel; // put a panel for the pause menu here
    public GameObject exitPanel; // put a panel for the exit clarification menu here

    void Update() //on everyframe..
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // if you press the escape key..
        {
            if (isPaused) // and the bool isPaused is set to true
            {
                Resume(); // start the function Resume
            }

            else // else ( the opposite of the previous if statement), if the bool isPaused is set to false
            {
                Paused(); // start the function Paused
            }
        }
    }
    // just like setting up the field up top, you can have to write 'private' before 'void Paused() instead "private void Paused ()"
    void Paused () // a hidden(private) function called pause
    { 
        pausePanel.SetActive(true); // activate the Pause menu
        Time.timeScale = 0; // set time to stop (0)
        isPaused = true; // set the bool isPaused is set to true
    }

    public void Resume () // a selectable(public) function called resume
    {// this function is set to a button and when the player presses the 'Esc' key
        pausePanel.SetActive(false); // disable the Pause menu
        Time.timeScale = 1; // set time to normal (1)
        isPaused = false; // set the bool isPaused is set to false
        exitPanel.SetActive(false); // disable the Exit menu
    }

    public void MainMenu() // a selectable(public) function called MainMenu
    {
        exitPanel.SetActive(true); // activate the Exit menu
    }

    public void CancelExit() // a selectable(public) function called CancelExit
    {
        exitPanel.SetActive(false); // disable the Exit menu
    }

    public void Exit() // a selectable(public) function called Exit
    {
        SceneManager.LoadScene(0); // in the SceneManager (build settings) load the scene numbered 0 in the index
    } // you can have it use the name of the scene instaed of the number it's indexed as "SceneManger.LoadScene(MainMenu);" if it you have put that scene in the build setting
}