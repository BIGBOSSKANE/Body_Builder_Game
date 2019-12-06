/*
Creator: Daniel
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class conveyorBelt : activate
{
    GameObject belt;
    AreaEffector2D effector;
    [Tooltip("Setting this to negative reverses the conveyor belt")] public float standardSpeed = 60f;
    [Tooltip("Setting this to negative reverses the conveyor belt")] public float overchargeSpeed = 100f;
    [Tooltip("Adjust this to control the visual speed of the conveyor belt")] public float spriteAnimatorSpeedRatio = 1;
    float speed;
    bool wasOvercharged;
    spriteAnimator spriteAnimator;

    void Start()
    {
        wasOvercharged = overcharge;
        belt = gameObject.transform.Find("Effector").gameObject;
        effector = belt.GetComponent<AreaEffector2D>();
        spriteAnimator = GetComponent<spriteAnimator>();

        if(overcharge) speed = overchargeSpeed;
        else speed = standardSpeed;
        if(speed < 0) spriteAnimator.reverse = true;
        spriteAnimator.framesPerSecond = Mathf.Abs(speed * spriteAnimatorSpeedRatio);
    }

    void Update()
    {
        if(!activated)
        {
            effector.forceMagnitude = 0f;
            return;
        }

        UpdateSpeed();

        effector.forceMagnitude = speed;
        wasOvercharged = overcharge;
    }

    void UpdateSpeed()
    {
        if(!wasOvercharged && overcharge)
        {
            if(speed < 0) spriteAnimator.reverse = true;
            speed = overchargeSpeed;
            spriteAnimator.framesPerSecond = Mathf.Abs(speed * spriteAnimatorSpeedRatio);
        }
        else if(wasOvercharged && !overcharge)
        {
            if(speed < 0) spriteAnimator.reverse = true;
            speed = standardSpeed;
            spriteAnimator.framesPerSecond = Mathf.Abs(speed * spriteAnimatorSpeedRatio);
        }
    }
}
