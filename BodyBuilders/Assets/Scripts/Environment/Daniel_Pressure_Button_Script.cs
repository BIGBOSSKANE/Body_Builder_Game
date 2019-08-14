using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daniel_Pressure_Button_Script : MonoBehaviour
{
    public GameObject manipulatedObject; // the first childed object
    Vector2 manipulatedObjectOriginalPos;
    public GameObject target1;
    Vector2 targetPos1;
    bool triggered;
    public float moveTimeTotal = 1f;
    float moveTime;

    void Start()
    {
        manipulatedObjectOriginalPos = manipulatedObject.transform.position;
        targetPos1 = target1.transform.position;
        moveTime = 0f;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        triggered = false;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        triggered = true;
    }

    void Update()
    {
        if(triggered == true)
        {
            moveTime += Time.deltaTime;
            if(moveTime > moveTimeTotal)
            {
                moveTime = moveTimeTotal;
            }
            manipulatedObject.transform.position = Vector2.Lerp(manipulatedObjectOriginalPos , targetPos1 , moveTime/moveTimeTotal);

        }
        else if(triggered == false)
        {
            moveTime -= Time.deltaTime;
            if(moveTime < 0f)
            {
                moveTime = 0f;
            }
            manipulatedObject.transform.position = Vector2.Lerp(manipulatedObjectOriginalPos , targetPos1 , moveTime/moveTimeTotal);
        }
    }
}
