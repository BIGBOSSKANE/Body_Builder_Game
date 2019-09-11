/*Author Kane
 * 15/05/19
 * Loads the next level on trigger
 */

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // import SceneMangement namespaces

public class StageExit : MonoBehaviour
{
    private int SceneNumber;

    private void Start()
    {
        SceneNumber = SceneManager.GetActiveScene().buildIndex + 1; // The SceneName is equaled to the currently active scene number + 1
    }

    private void OnTriggerEnter2D(Collider2D other)
    {        
        if (other.gameObject.tag == "Player") // If an other object that has the tag Player collides with this objects 2D Collider
        {
            Debug.Log("Win! load next level");
            if (PlayerPrefs.GetInt("levelReached") < SceneNumber)
            {
                PlayerPrefs.SetInt("levelReached", (SceneNumber));
            }
            SceneManager.LoadScene(SceneNumber); //In scene manager, Load scene number equalled to SceneName
        }
    }
}