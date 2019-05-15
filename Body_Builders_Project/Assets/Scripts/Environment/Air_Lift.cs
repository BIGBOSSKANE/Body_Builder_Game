using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Air_Lift : MonoBehaviour
{   public float gravity = -9.81f;
    public float force = 10.81f;

    void Start()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        var coll = ps.collision;
        coll.enabled = true;
        coll.bounce = 0.5f;
    }
}