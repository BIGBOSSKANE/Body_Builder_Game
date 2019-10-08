using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gyreDetect : MonoBehaviour
{
    public GameObject patrol;
    public GameObject alert;

    NewGyre nG;
    gyreAlerted nA;

[Tooltip("tick if it's normal, if not it's alert")]
    public bool normalOrAlert = true; //if true, then it is set to a normal patrol Gyre, if false it is set to the alert Gyre

    public float time = 3.5f;
    float resetTime = 3.5f;

    bool colliding = true;

    // Start is called before the first frame update
    void Start()
    {
        if(normalOrAlert) // if true
        {
            alert.gameObject.SetActive(false);
            patrol.gameObject.SetActive(true);
        }
        else //if false
        {
            patrol.gameObject.SetActive(false);
            alert.gameObject.SetActive(true);
        }

        nG = patrol.gameObject.GetComponent<NewGyre>();
        nA = alert.gameObject.GetComponent<gyreAlerted>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!normalOrAlert)
        {
            if (!colliding)
            {
                time -= Time.deltaTime;
                if (time < 0)
                {
                    time = 0;
                }
            }

            if (time == 0)
            {
                nA.detected = false;
                StartCoroutine(Processing());
            }
        }
    }

    public IEnumerator Processing()
    {
        if (!normalOrAlert)
        {
            nA.patrolling = false;
        }
        else
        {
            nG.patrolling = false;
        }
        yield return new WaitForSeconds(1);
        Transforming();
    }

    public void Transforming()
    {
        if (!normalOrAlert)
        {
            Debug.Log("transformed");
            normalOrAlert = true;
        }
        else
        {
            Debug.Log("transformed");
            normalOrAlert = false;
        }
    }

    public void OnTriggerExit2D(Collider2D col)
    {
        if (!normalOrAlert)
        {
            if (col.tag == "Player")
            {
                Debug.Log("Missing");
                colliding = false;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            if (normalOrAlert)
            {
                Debug.Log("player detected");
                StartCoroutine(Processing());
            }
            else
            {
                Debug.Log("player found");
                colliding = true;
                time = resetTime;
            }
        }
    }
}