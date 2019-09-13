/*
Creator: Daniel
Created: 13/04/2019
Laste Edited by: Daniel
Last Edit: 13/09/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnerHeadSprite : MonoBehaviour
{
    public void SetSprite(int configuration)
    {
        if(configuration == 1)
        {
            gameObject.transform.Find("Scaler").gameObject.SetActive(false);
            gameObject.transform.Find("Hookshot").gameObject.SetActive(false);
        }
        else if(configuration == 2)
        {
            gameObject.transform.Find("Scaler").gameObject.SetActive(true);
            gameObject.transform.Find("Hookshot").gameObject.SetActive(false);
        }
        else if(configuration == 3)
        {
            gameObject.transform.Find("Scaler").gameObject.SetActive(false);
            gameObject.transform.Find("Hookshot").gameObject.SetActive(true);
        }
        else if(configuration == 4)
        {
            gameObject.transform.Find("Scaler").gameObject.SetActive(true);
            gameObject.transform.Find("Hookshot").gameObject.SetActive(true);
        }
    }
}
