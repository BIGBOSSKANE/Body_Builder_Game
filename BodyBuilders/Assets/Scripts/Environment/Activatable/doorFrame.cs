using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorFrame : MonoBehaviour
{
    bool open;
    bool turning;
    GameObject leftCog;
    GameObject rightCog;
    [HideInInspector] public float moveTime;
    float timer = 0f;
    [HideInInspector] public float moveDistance;
    float angle;
    float maxAngle;
    public float rotateSpeed;

    void Start()
    {
        leftCog = gameObject.transform.Find("LeftGear").gameObject;
        rightCog = gameObject.transform.Find("RightGear").gameObject;
    }

    public void ReceiveDoorTraits(float time , float distance)
    {
        moveTime = time;
        moveDistance = distance;
        maxAngle = rotateSpeed * moveDistance;
        angle = maxAngle / moveTime;
    }

    void Update()
    {
        if(turning)
        {
            if(open)
            {
                timer += Time.deltaTime / moveTime;
                if(timer >= 1) timer = 1;
            }
            else
            {
                timer -= Time.deltaTime / moveTime;
                if(timer <= 0) timer = 0;
            }

            leftCog.transform.Rotate(new Vector3(0 , 0 , angle) , Space.Self);
            rightCog.transform.Rotate(new Vector3(0 , 0 , -angle) , Space.Self);

            if((timer >= 1 && open) || (timer <= 0 && !open)) turning = false;
        }
    }

    public void Operate(bool opening)
    {
        if(open != opening) angle = -angle;
        open = opening;
        turning = true;
    }
}
