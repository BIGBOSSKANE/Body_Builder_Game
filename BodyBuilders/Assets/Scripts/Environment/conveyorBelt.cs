using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class conveyorBelt : MonoBehaviour
{
    GameObject belt;
    AreaEffector2D effector;
    public float movementSpeed = 30f;

    void Start()
    {
        belt = gameObject.transform.Find("Effector").gameObject;
        effector = belt.GetComponent<AreaEffector2D>();
    }

    void Update()
    {
        effector.forceMagnitude = movementSpeed;
    }
}
