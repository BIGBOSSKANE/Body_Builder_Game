using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daniel_Timer_Button_Script : MonoBehaviour
{
    public GameObject manipulatedObject; // the first childed object
    Vector2 manipulatedObjectOriginalPos;
    public GameObject target1;
    Vector2 targetPos1;
    int triggerCounter;
    public float moveTimeTotal = 1f;
    float moveTime;

    public float totalTime = 5f;
    float timeRemaining;
    bool triggered;

    void Start()
    {
        manipulatedObjectOriginalPos = manipulatedObject.transform.position;
        targetPos1 = target1.transform.position;
        triggerCounter = 0;
        moveTime = 0f;
        timeRemaining = totalTime;
        triggered = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        triggerCounter ++;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        triggerCounter --;
    }

    void Update()
    {
        timeRemaining -= Time.deltaTime;
        {
            if(timeRemaining < 0f)
            {
                timeRemaining = 0f;
            }
        }

        if(triggerCounter > 0)
        {
            timeRemaining = totalTime;
            triggered = true;
        }

        if(triggered == true && timeRemaining > 0f)
        {
            moveTime += Time.deltaTime;
            if(moveTime > moveTimeTotal)
            {
                moveTime = moveTimeTotal;
            }
            manipulatedObject.transform.position = Vector2.Lerp(manipulatedObjectOriginalPos , targetPos1 , moveTime/moveTimeTotal);
        }

        if(triggerCounter == 0 && timeRemaining == 0f)
        {
            triggered = false;
            moveTime -= Time.deltaTime;
            if(moveTime < 0f)
            {
                moveTime = 0f;
            }
            manipulatedObject.transform.position = Vector2.Lerp(manipulatedObjectOriginalPos , targetPos1 , moveTime/moveTimeTotal);
        }
    }
}
