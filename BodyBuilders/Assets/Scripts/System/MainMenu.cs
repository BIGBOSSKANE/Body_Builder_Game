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
    public GameObject Main; // put a panel for the Main Menu here
    public GameObject levelSelect; // put a panel for the Level Selection Menu here
    bool islvlslct = false; // a private(cannot be changed outside the script) on off switch called isLvlSlct

    void Update()
    {
        if (islvlslct) // and the bool islvlslct is set to true
        {
            Main.SetActive(false); // disable the Main Menu panel
            levelSelect.SetActive(true); // activate the Level Select menu panel
        }

        else // else ( the opposite of the previous if statement), if the bool islvlslct is set to false
        {
            Main.SetActive(true); // activate the Main Menu panel
            levelSelect.SetActive(false); // disable the Level Select menu panel
        }
    }

    // used for the New Game Button
    public void NewGameButton() // a selectabel(public) function called NewGameButton
    {
        Debug.Log("Start game"); // in the console type Start game
        SceneManager.LoadScene(1); // in the SceneManager (build settings) load the scene numbered 1 in the index
    }
    // used for the Level Selection button
    public void LevelSelectionButton() // a selectable(public) function called LevelSelectionButton
    {
        islvlslct = true; // set the bool islvlslct is set to true
    }
    // used for the Return Button
    public void ReturnButton() // a selectable(public) function called ReturnButton
    {
        islvlslct = false; // set the bool islvlslct is set to false
    }
    // used for the buttons to select levels
    public void LevelButton(int level) // a selectable(public) function called LevelButton, when setting up the button write the number of that scenes index
    {
        SceneManager.LoadScene(level); // in the SceneManager (build settings) load the scene number writen in the onclick () in the index
    } 

public void QuitGameButton() // a selectable(public) function called QuitGameButton
    {
        Debug.Log("Quited the game"); // in the console type Quited the game
        Application.Quit(); // close the game
    }
}