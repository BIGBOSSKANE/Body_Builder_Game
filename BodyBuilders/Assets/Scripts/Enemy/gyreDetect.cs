using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gyreDetect : MonoBehaviour
{
    public GameObject patrol;
    public GameObject alert;

    NewGyre nG;
    gyreAlerted nA;

    public Vector2 patrolPos;
    public Vector2 alertPos;

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
        patrolPos = patrol.gameObject.transform.position;
        alertPos = alert.gameObject.transform.position;

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
        else
        {
            if(colliding)
            {
                StartCoroutine(Processing()); // start the IEnumerator Processing function
            }
        }
    }

    public IEnumerator Processing() // a function to prepare the transform
    {
        
        if (!isPatroling) // if alerted
        {
            nA.patrolling = false; // in the alerted script, set patrolling to false, to stop moving
            yield return new WaitForSeconds(1); // wait for 1 second
            Transformpatrol(); // start the Transforming function
        }
        else // if patroling
        {
            nG.patrolling = false; // in the patroling script, set patrolling to false, to stop moving
            yield return new WaitForSeconds(1); // wait for 1 second
            Transformalert(); // start the Transforming function
        }
    }

    public void Transformalert() // thisfunction transform the gyre form alert to patrol and vice versa
    {
        if (isPatroling) // if patroling
        {
            alert.gameObject.SetActive(true); // alert gameobject is set to active
            patrol.gameObject.SetActive(false); // patrol gameobject is set to unactive
            gameObject.transform.parent = alert.transform;
            gameObject.transform.localPosition = new Vector2(0, 0.411f);
            alert.transform.position = patrolPos += new Vector2(0, 0.422f);
            nA.patrolling = true; // in the alerted script, set patrolling to true, to start moving
            isPatroling = false; // and is now set to alert
        }
    }
    
    public void Transformpatrol() // thisfunction transform the gyre form alert to patrol and vice versa
    {
        if (!isPatroling) // if alert
        {
            alert.gameObject.SetActive(false); // alert gameobject is set to unactive
            patrol.gameObject.SetActive(true); // patrol gameobject is set to active
            gameObject.transform.parent = patrol.transform;
            gameObject.transform.localPosition = new Vector2(0, 0.411f);
            patrol.transform.position = alertPos -= new Vector2(0, 0.422f);
            nG.patrolling = true; // in the patroling script, set patrolling to true, to start moving
            isPatroling = true; // and is now set to patrol
        }
    }

    public void OnTriggerExit2D(Collider2D col) // if a gameobject has left the trigger,
    {
         if (col.tag == "Player") // and if the gameobject is tagged as Player,
         {
             colliding = false; // the player is no longer colliding 
         }
    }

    public void OnTriggerEnter2D(Collider2D col) // if a gameobject has entered the trigger,
    {
        if (col.tag == "Player") // the gameobject is tagged as Player,
        {
            colliding = true; // the player is colliding 
            time = resetTime; // set the time to the same number as reset time

            if (isPatroling) // the gyre is patroling
            {
                StartCoroutine(Processing()); // start the IEnumerator Processing function
            }
        }
    }
}