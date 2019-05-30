using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daniel_Switch_Button_Script : MonoBehaviour
{
    public GameObject manipulatedObject; // the first childed object
    Vector2 manipulatedObjectOriginalPos;
    public GameObject target1;
    Vector2 targetPos1;
    int triggerCounter;
    public float moveTimeTotal = 1f;
    float moveTime;
    bool triggered;

    void Start()
    {
        manipulatedObjectOriginalPos = manipulatedObject.transform.position;
        targetPos1 = target1.transform.position;
        triggerCounter = 0;
        moveTime = 0f;
        triggered = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        triggerCounter ++;
        if(triggerCounter == 1)
        {
            triggered = !triggered;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        triggerCounter --;
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
