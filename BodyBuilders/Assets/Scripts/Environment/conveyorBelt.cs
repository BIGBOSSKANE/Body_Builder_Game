using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class conveyorBelt : activate
{
    GameObject belt;
    AreaEffector2D effector;
    public float standardSpeed = 60f;
    public float overloadSpeed = 100f;
    float speed;

    void Start()
    {
        belt = gameObject.transform.Find("Effector").gameObject;
        effector = belt.GetComponent<AreaEffector2D>();
    }

    void Update()
    {
        if(!activated)
        {
            effector.forceMagnitude = 0f;
            return;
        }

        if(overcharge)
        {
            speed = overloadSpeed;
        }
        else
        {
            speed = standardSpeed;
        }

        effector.forceMagnitude = speed;
    }
}
