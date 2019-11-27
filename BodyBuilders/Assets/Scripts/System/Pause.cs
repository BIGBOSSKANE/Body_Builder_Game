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
    public GameObject settingsPanel; // put a panel for the settings panel here
    public GameObject restartPanel; // put a panel for the restart clarification menu here
    float pausedTimeScale;
    timeSlow timeSlow;

    void Start() // on start..
    {
        timeSlow = GameObject.Find("Player").GetComponent<timeSlow>(); // set timeSlow to be the timeSlow component on the player
        pausePanel.SetActive(false); // disable the pause screen
        exitPanel.SetActive(false); // disable the Exit menu
        settingsPanel.SetActive(false); // disable the settings menu
        restartPanel.SetActive(false); // disable the restart panel menu
    }

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
    
    void Paused () // a hidden(private) function called pause
    {
        // Added soundtrack change
        AkSoundEngine.SetState("Mode" , "PauseMenu");
        AkSoundEngine.PostEvent("ExitFan" , gameObject);
        //
        pausePanel.SetActive(true); // activate the Pause menu
        pausedTimeScale = Time.timeScale;
        Time.timeScale = 0; // set time to stop (0)
        isPaused = true; // set the bool isPaused is set to true
        TimeMaster(pausedTimeScale);
    }

    public void Resume () // a selectable(public) function called resume
    {// this function is set to a button and when the player presses the 'Esc' key
        // Added soundtrack change
        AkSoundEngine.SetState("Mode" , "Gameplay");
        //
        pausePanel.SetActive(false); // disable the Pause menu
        exitPanel.SetActive(false); // disable the Exit menu
        settingsPanel.SetActive(false); // disable the settings menu
        restartPanel.SetActive(false); // disable the restart panel menu
        Time.timeScale = 1; // set time to normal (1)
        isPaused = false; // set the bool isPaused is set to false
        TimeMaster(pausedTimeScale);
    }

    public void MainMenuPanel() // a selectable(public) function called MainMenu
    {
        exitPanel.SetActive(true); // activate the Exit menu
        pausePanel.SetActive(false); // disables the Pause menu
    }

    public void SettingsPanel() // a selectable(public) function called MainMenu
    {
        settingsPanel.SetActive(true); // activate the Exit menu
        pausePanel.SetActive(false); // disables the Pause menu
    }

    public void RestartPanel() // a selectable(public) function called MainMenu
    {
        restartPanel.SetActive(true); // activate the restart menu
        pausePanel.SetActive(false); // disables the Pause menu
    }

    public void ReturnButton() // a selectable(public) function called Return
    {
        pausePanel.SetActive(true); // activate the Pause menu
        exitPanel.SetActive(false); // disable the Exit menu
        settingsPanel.SetActive(false); // disable the settings menu
        restartPanel.SetActive(false); // disable the restart panel menu
    }

    public void ExitButton() // a selectable(public) function called Exit
    {
        // Added soundtrack change
        AkSoundEngine.SetState("Mode" , "MainMenu");
        //
        SceneManager.LoadScene(0); // in the SceneManager (build settings) load the scene numbered 0 in the index
        timeSlow.TimeSlave(pausedTimeScale, !isPaused);
    } // you can have it use the name of the scene instead of the number it's indexed as "SceneManger.LoadScene(MainMenu);" if it you have put that scene in the build setting

    public void RestartButton() // a selectable(public) function called Exit
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // In the SceneManager, load the active scene (restart)
        timeSlow.TimeSlave(pausedTimeScale, !isPaused);
    }

    void TimeMaster(float pausedTimeScale) // references other time-adjusting scripts and takes complete control of time
    {
        timeSlow.TimeSlave(pausedTimeScale , isPaused);
    }
}