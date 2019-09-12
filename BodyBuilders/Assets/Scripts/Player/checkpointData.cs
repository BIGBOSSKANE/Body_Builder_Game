using UnityEngine;
using UnityEngine.SceneManagement;

public class checkpointData : MonoBehaviour
{
    [Tooltip("Prevent checkpoint save between loads. Turn this off when building")] public bool preventSave = false;
    float xPosition;
    float yPosition;
    int partConfiguration;
    int headConfiguration;
    string armString = "BasicArms";
    string legString = "BasicLegs";
    playerScript playerScript;

    void Awake()
    {
        playerScript = gameObject.GetComponent<playerScript>();
    }

    public void CheckpointStartCheck() // called from player start
    {
        if(PlayerPrefs.GetInt("currentLevel") != SceneManager.GetActiveScene().buildIndex || PlayerPrefs.GetInt("preventSave") == 0)//PlayerPrefs.GetInt("savePrevented") == 1) // set new respawn values at the start point
        {
            xPosition = transform.position.x;
            PlayerPrefs.SetFloat("xPosition" , xPosition);
            yPosition = transform.position.y;
            PlayerPrefs.SetFloat("yPosition" , yPosition);
            partConfiguration = playerScript.partConfiguration;
            PlayerPrefs.SetInt("partConfig" , partConfiguration);
            headConfiguration = playerScript.headConfiguration;
            PlayerPrefs.SetInt("headConfig" , headConfiguration);
            armString = playerScript.armString;
            PlayerPrefs.SetString("armString" , armString);
            legString = playerScript.legString;
            PlayerPrefs.SetString("legString" , legString);
            PlayerPrefs.SetInt("preventSave" , 1);
        }
        else // use existing respawn conditions
        {
            xPosition = PlayerPrefs.GetFloat("xPosition");
            yPosition = PlayerPrefs.GetFloat("yPosition");
            partConfiguration = PlayerPrefs.GetInt("partConfig");
            headConfiguration = PlayerPrefs.GetInt("headConfig");
        }

        PlayerPrefs.SetInt("currentLevel" , SceneManager.GetActiveScene().buildIndex);

        //playerScript.Respawn(new Vector2(xPosition , yPosition) , partConfiguration , headConfiguration , armConfig , legConfig);
    }

    public void SetCheckpoint(Vector2 checkpointPos , int bodyParts , int headParts , string armsString , string legsString) // set checkpoint when entering a checkpoint
    {
        PlayerPrefs.SetFloat("xPosition" , checkpointPos.x);
        PlayerPrefs.SetFloat("yPosition" , checkpointPos.y);
        PlayerPrefs.SetInt("partConfig" , bodyParts);
        PlayerPrefs.SetInt("headConfig" , headParts);
        PlayerPrefs.SetString("armString" , armsString);
        PlayerPrefs.SetString("legString" , legsString);
        PlayerPrefs.SetInt("currentLevel" , SceneManager.GetActiveScene().buildIndex);
    }

    void OnDrawGizmos() // reset the save so you can load in to a new level from the editor
    {
        if(!Application.isPlaying)
        {
            if(preventSave) PlayerPrefs.SetInt("preventSave" , 0);
            else PlayerPrefs.SetInt("preventSave" , 1);
        }
    }
}
