using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Detach_PopUp : MonoBehaviour
{
    public Text popUp;
    public Image popIm;
    public GameObject uiLocat;
    int playerParts;

    void Start()
    {
        popUp.enabled = false;
        popIm.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            playerParts = col.GetComponent<Player_Controller>().partConfiguration;
        }

        if (playerParts > 2)
        {
            popUp.enabled = true;
            popIm.enabled = true;
            Vector2 poppos = Camera.main.WorldToScreenPoint(uiLocat.transform.position);
            popUp.transform.position = poppos;
            StartCoroutine("PopUpActive");
        }
        else
        {
            popUp.enabled = false;
            popIm.enabled = false;
        }
    }
    private IEnumerator PopUpActive()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.5f); // wait a selected amount of time
            Debug.Log("UI popped up");
            Destroy(popUp);
            Destroy(popIm);
            yield break;
        }
    }
}