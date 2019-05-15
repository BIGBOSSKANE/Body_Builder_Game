using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Pressure_Pad_Timed : MonoBehaviour
{
    public GameObject door;
    public string animationName = "Opening";
    public string animationName2 = "Closing";
    public float timer = 5f;

    // Update is called once 
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject)
        {
            {
                Debug.Log("collided with button");
                Animator a = door.GetComponent<Animator>();
                if (a != null)
                {
                    a.Play(animationName);
                    StartCoroutine("ClosingDoor");
                    Debug.Log("I'm looping");
                }
            }
        }
    }
    private IEnumerator ClosingDoor()
    {
        while (true)
        {
            yield return new WaitForSeconds(timer); // wait a selected amount of time
            Debug.Log("Time's up, Door down");
                Animator a = door.GetComponent<Animator>();
                if (a != null)
                {
                a.Play(animationName2);
                }
            }
        }
    }
