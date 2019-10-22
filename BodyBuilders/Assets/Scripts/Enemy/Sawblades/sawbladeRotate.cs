using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sawbladeRotate : MonoBehaviour
{
    public bool activated = true;
    public float rotateTime = 4f;
    public float startRotation = 0f;
    public float bladeDistance = 5f;
    public float spinSpeed = 200f;
    float rotateTimer;
    public Vector2 centrePoint;
    Transform blade;
    LineRenderer lineRenderer;

    void Start()
    {
        centrePoint = new Vector2(0 , 0.44f * (transform.localScale.x / 0.35f));
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.5f * (transform.localScale.x / 0.35f);
        lineRenderer.endWidth = 0.5f * (transform.localScale.x / 0.35f);
        blade = transform.Find("SawBlade");
        blade.GetComponent<sawblade>().spinSpeed = spinSpeed;
    }

    void Update()
    {
        if(activated)
        {
            rotateTimer += Time.deltaTime / rotateTime;
            float currentAngle = 360 * rotateTimer + startRotation;
            blade.transform.localPosition = (Vector2)(Quaternion.Euler(0 , 0 , currentAngle).normalized * Vector2.right) * bladeDistance;
        }
    }

    void LateUpdate()
    {
        if(activated)
        {
            lineRenderer.SetPosition(0 , centrePoint + (Vector2)transform.position);
            lineRenderer.SetPosition(1 , blade.position);
        }
    }

    void OnDrawGizmos()
    {
        if(!Application.isPlaying)
        {
            Vector2 originPoint = (Vector2)transform.position + centrePoint;
            Debug.DrawLine(originPoint , originPoint + ((Vector2)(Quaternion.Euler(0 , 0 , startRotation).normalized * Vector2.right) * bladeDistance * transform.lossyScale.x) , Color.blue);
        }
    }
}
