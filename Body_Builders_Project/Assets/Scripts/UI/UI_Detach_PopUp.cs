using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Detach_PopUp : MonoBehaviour
{
    public Text popUp;
    public Image popIm;
    private bool mIPopped = false;

    void Start()
    {
        popUp.enabled = false;
        popIm.enabled = false;
    }

    void Update()
    {
        if (mIPopped == true)
        {
            popUp.enabled = true;
            popIm.enabled = true;
            Vector2 poppos = Camera.main.WorldToScreenPoint(this.transform.position);
            popUp.transform.position = poppos;
            StartCoroutine("PopUpActive");
        }
        if (mIPopped == false)
        {
            popUp.enabled = false;
            popIm.enabled = false;
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Legs")
        {
            mIPopped = true;
        }
        if (other.gameObject.tag == "Arms")
        {
            mIPopped = true;
        }
    }
    private IEnumerator PopUpActive()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.5f); // wait a selected amount of time
            Debug.Log("UI popped up");
            mIPopped = false;
            yield break;
        }
    }
}