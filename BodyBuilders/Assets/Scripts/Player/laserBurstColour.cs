using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserBurstColour : MonoBehaviour
{
    public GameObject blue;
    public GameObject purple;
    public GameObject red;

    public void ColourChange(string colour)
    {
        if(colour == "blue")
        {
            blue.SetActive(true);
            purple.SetActive(false);
            red.SetActive(false);
        }
        else if(colour == "purple" )
        {
            blue.SetActive(false);
            purple.SetActive(true);
            red.SetActive(false);
        }
        else if(colour == "red")
        {
            blue.SetActive(false);
            purple.SetActive(false);
            red.SetActive(true);
        }
        if(colour == "none")
        {
            blue.SetActive(false);
            purple.SetActive(false);
            red.SetActive(false);
        }
    }
}
