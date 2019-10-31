/*Author Kane Girvan
 * 15/8/19
 * Script to navigate the main menu and level selector
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // this is allows you to use certain classes from SceneManagement (it's used to navigate different scenes in this game project(that is set in the build settings)

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu; // put a panel for the main Menu here
    public GameObject levelSelect; // put a panel for the Level Selection Menu here
    public GameObject settings;

    void Start()
    {
        mainMenu.gameObject.SetActive(true); 
        levelSelect.gameObject.SetActive(false);
        settings.gameObject.SetActive(false);
    }

    // used for the New Game Button
    public void NewGameButton() // a selectabel(public) function called NewGameButton
    {
        SceneManager.LoadScene(1); // in the SceneManager (build settings) load the scene numbered 1 in the index
    }

    // used for the Level Selection button
    public void LevelSelectionButton() // a selectable(public) function called LevelSelectionButton
    {
        mainMenu.gameObject.SetActive(true);
        levelSelect.gameObject.SetActive(false);
        settings.gameObject.SetActive(false);
    }

    // used for the Return Button
    public void ReturnButton() // a selectable(public) function called ReturnButton
    {
        mainMenu.gameObject.SetActive(true);
        levelSelect.gameObject.SetActive(false);
        settings.gameObject.SetActive(false);
    }

    public void SettingsButton() // a selectable(public) function called SettingsButton
    {
        mainMenu.gameObject.SetActive(false);
        levelSelect.gameObject.SetActive(false);
        settings.gameObject.SetActive(true);
    }
    // used for the buttons to select levels
    public void LevelButton(int level) // a selectable(public) function called LevelButton, when setting up the button write the number of that scenes index
    {
        SceneManager.LoadScene(level); // in the SceneManager (build settings) load the scene number writen in the onclick () in the index
    } 

    public void QuitGameButton() // a selectable(public) function called QuitGameButton
    {
        Application.Quit(); // close the game
    }
}