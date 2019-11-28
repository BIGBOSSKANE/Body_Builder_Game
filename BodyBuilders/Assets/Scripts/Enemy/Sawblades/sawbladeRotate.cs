using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sawbladeRotate : activate
{
    public float rotateTime = 4f;
    public float startRotation = 0f;
    public float bladeDistance = 5f;
    public float spinSpeed = 200f;
    float rotateTimer;
    Vector2 centrePoint = Vector2.zero;
    Transform blade;
    LineRenderer lineRenderer;

    void Start()
    {
        centrePoint = (Vector2)transform.position + ((Vector2)transform.up * 1.23f * transform.localScale.x); // apply scaling for centre of mounted part
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.5f * (transform.localScale.x / 0.35f);
        lineRenderer.endWidth = 0.5f * (transform.localScale.x / 0.35f);
        blade = transform.Find("SawBlade");
        blade.GetComponent<sawblade>().spinSpeed = spinSpeed;
        lineRenderer.SetPosition(0 , centrePoint);
        lineRenderer.SetPosition(1 , blade.position);
    }

    void Update()
    {
        if(activated)
        {
            rotateTimer += Time.deltaTime / rotateTime;
            float currentAngle = 360 * rotateTimer + startRotation;
            centrePoint = (Vector2)transform.position + ((Vector2)transform.up * 1.23f * transform.localScale.x);
            blade.transform.position = (Vector2)(Quaternion.Euler(0 , 0 , currentAngle).normalized * Vector2.right) * bladeDistance + centrePoint;
        }
    }

    void LateUpdate()
    {
        if(activated)
        {
            lineRenderer.SetPosition(1 , blade.position);
        }
    }

    void OnDrawGizmos()
    {
        if(!Application.isPlaying)
        {
            Vector2 originPoint = (centrePoint == Vector2.zero)? (Vector2)transform.position + ((Vector2)transform.up * 1.23f * transform.localScale.x) : originPoint = centrePoint;
            Debug.DrawLine(originPoint , originPoint + ((Vector2)(Quaternion.Euler(0 , 0 , startRotation).normalized * Vector2.right) * bladeDistance) , Color.blue);
        }
    }
}
