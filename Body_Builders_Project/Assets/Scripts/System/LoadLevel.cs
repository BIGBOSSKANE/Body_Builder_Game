/*Author Kane
 * 15/05/19
 * Loads a level on trigger
 */
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // import SceneMangement namespaces

public class LoadLevel : MonoBehaviour
{
    private int SceneName;

    private void Start()
    {
        SceneName = SceneManager.GetActiveScene().buildIndex + 1;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(SceneName); //In scene manager, Load scene (insert Scene here)
        }
    }
}