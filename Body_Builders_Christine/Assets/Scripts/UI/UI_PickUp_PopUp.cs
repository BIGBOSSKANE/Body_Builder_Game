using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PickUp_PopUp : MonoBehaviour
{
    public Text popUp;
    public Image popIm;
    public bool hasArms = false;
    public bool boxHere = false;
    public GameObject uiLocat;

    void Start()
    {
        popUp.enabled = false;
        popIm.enabled = false;
    }

    void Update()
    {
        if (hasArms && boxHere == true)
        {
            popUp.enabled = true;
            popIm.enabled = true;
            Vector2 poppos = Camera.main.WorldToScreenPoint(uiLocat.transform.position);
            popUp.transform.position = poppos;
        }
        else
        {
            popUp.enabled = false;
            popIm.enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Arms")
        {
            hasArms = true;
        }
        if (other.gameObject.tag == "Box")
        {
            boxHere = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Arms")
        {
            hasArms = false;
        }
        if (other.gameObject.tag == "Box")
        {
            boxHere = false;
        }
    }
    }