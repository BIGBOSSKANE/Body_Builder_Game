/* Kane
 * 28/05/19
 * Loads a specific level
 * can be used in a menu, or reworked into a trigger
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //uses the scene manager

public class Load_Specific_Level : MonoBehaviour
{
    public int lvlNumber; // pick a number from the build settings

    public void loadlevel(string level)
    {
        SceneManager.LoadScene(lvlNumber); // loads the scene equalled to lvlNumber
    }
}
