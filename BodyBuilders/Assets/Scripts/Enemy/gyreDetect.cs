using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gyreDetect : MonoBehaviour
{
    public GameObject myGyre;
    NewGyre nG;
    gyreAlerted gA;

    [Tooltip("It ticked it's normal, if not it's alert")]
    public bool normalOrAlert;

    public float time = 3.5f;
    float resetTime = 3.5f;

    bool colliding = true;

    // Start is called before the first frame update
    void Start()
    {
        if(normalOrAlert)
        {
            nG = myGyre.GetComponent<NewGyre>();
        }
        else
        {
            gA = myGyre.GetComponent<gyreAlerted>();
        }
    }

    // Update is called once per frame
    void Update()
    {
       if(!colliding)
        {
            time -= Time.deltaTime;
            if (time < 0)
            {
                time = 0;
            }
        }

        if (time == 0)
        {
            gA.detected = false;
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
                time = resetTime;
            }

        }
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (normalOrAlert)
        {
            if (col.tag == "Player")
            {
                nG.detected = true;
            }
        }
        if (!normalOrAlert)
        {
            if (col.tag == "Player")
            {
                Debug.Log("found again");
                colliding = true;
                time = resetTime;
            }
            else
            {
                Debug.Log("Missing");
                colliding = false;
            }
        }
    }
}
