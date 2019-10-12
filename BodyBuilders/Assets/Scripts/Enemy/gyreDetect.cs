using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gyreDetect : MonoBehaviour
{
    public GameObject patrol;
    public GameObject alert;

    NewGyre nG;
    gyreAlerted nA;

[Tooltip("tick if it's patroling, if not it's alerted")]
    public bool isPatroling; //if true, then it is set to a normal patrol Gyre, if false it is set to the alert Gyre

    public float time = 3.5f;
    float resetTime = 3.5f;

    public bool colliding; // is the player colliding with the collider, this is to detect the player

    // Start is called before the first frame update
    void Start()
    {
        isPatroling = true; // patrol gyre is active on start

        colliding = false; // the player is not colliding on start

        resetTime = time; // the reset time will be the same as time

        alert.gameObject.SetActive(false); // alert gameobject is set to unactive
        patrol.gameObject.SetActive(true); // patrol gameobject is set to active

        nG = patrol.gameObject.GetComponent<NewGyre>(); // nG is the NewGyre script in the patrol gyre
        nA = alert.gameObject.GetComponent<gyreAlerted>(); // nA is the gyreAlerted script in the alerted gyre
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPatroling) // if alerted
        {
            if (!colliding) //colliding bool is set false
            {
                time -= Time.deltaTime; // count down time
                if (time < 0) // if time is less than 0...
                {
                    time = 0; // time will equal 0
                }

                if (time == 0) // if time equals 0..
                {
                StartCoroutine(Processing()); // start the IEnumerator Processing function
                }
            }
        }

        if (isPatroling) // if true
        {
            alert.gameObject.SetActive(false); // alert gameobject is set to unactive
            patrol.gameObject.SetActive(true); // patrol gameobject is set to active
        }
        else //if false
        {
            patrol.gameObject.SetActive(false); // patrol gameobject is set to unactive
            alert.gameObject.SetActive(true); // alert gameobject is set to active
        }
    }

    public IEnumerator Processing() // a function to prepare the transform
    {
        if (!isPatroling) // if alerted
        {
            nA.patrolling = false; // in the alerted script, set patrolling to false, to stop moving
        }
        else // if patroling
        {
            nG.patrolling = false; // in the patroling script, set patrolling to false, to stop moving
        }
        yield return new WaitForSeconds(1); // wait for 1 second
        Transforming(); // start the Transforming function
    }

    public void Transforming() // thisfunction transform the gyre form alert to patrol and vice versa
    {
        if (!isPatroling) // if alerted
        {
            Debug.Log("transformed");
            alert.gameObject.SetActive(false); // alert gameobject is set to unactive
            patrol.gameObject.SetActive(true); // patrol gameobject is set to active
            isPatroling = true; // and is now set to patrol
        }
        else // if patroling
        {
            Debug.Log("transformed");
            patrol.gameObject.SetActive(false); // patrol gameobject is set to unactive
            alert.gameObject.SetActive(true); // alert gameobject is set to active
            isPatroling = false; // and is now set to alert
        }
    }

    public void OnTriggerExit2D(Collider2D col) // if a gameobject has left the trigger,
    {
        if (!isPatroling) // the gyre is alerted...
        {
            if (col.tag == "Player") // and if the gameobject is tagged as Player,
            {
                Debug.Log("player Missing");
                colliding = false; // the player is no longer colliding 
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D col) // if a gameobject has entered the trigger,
    {
        if (col.tag == "Player") // the gameobject is tagged as Player,
        {
            colliding = true; // the player is colliding 
            if (isPatroling) // the gyre is patroling
            {
                Debug.Log("player detected");
                StartCoroutine(Processing()); // start the IEnumerator Processing function
            }
            else // if alerted
            {
                Debug.Log("player found");
                time = resetTime; // set the time to the same number as reset time
            }
        }
    }
}