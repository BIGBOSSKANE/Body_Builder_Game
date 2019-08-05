using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banshee_Script : MonoBehaviour
{
    bool spotted = false;
    public GameObject target;
    public float laserCharge = 1.5f;
    public float laserFireTime = 3f;
    public LayerMask collideableLayers;
    public LineRenderer lineRenderer;
    public GameObject laserOriginPoint;
    public Vector3 laserOrigin;
    public Vector3 laserTarget;
    CircleCollider2D circleCol;
    float laserRange = 100f;
    //bool windUp;

    // Start is called before the first frame update
    void Start()
    {
        spotted = false;
        laserOriginPoint = gameObject.transform.Find("Laser_Origin").gameObject;
        //windUp = false;
        circleCol = gameObject.GetComponent<CircleCollider2D>();
        laserRange = circleCol.radius;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();

        if (spotted == true)
        {
            laserOrigin = laserOriginPoint.transform.position;
            laserOrigin.z = 0f;

            Vector2 targ = target.transform.position;

            Vector2 objectPos = transform.position;
            targ.x = targ.x - objectPos.x;
            targ.y = targ.y - objectPos.y;

            float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            lineRenderer.SetPosition (0 , laserOrigin);

            RaycastHit hit;
            if(Physics.Raycast(laserOrigin , transform.right , out hit , collideableLayers))
            {
                laserTarget = hit.point;
                laserTarget.z = 0f;
                lineRenderer.SetPosition(1 , laserTarget);
                Debug.Log("Hit");
            }
            else
            {
                laserTarget = laserOrigin + transform.right * laserRange;
                laserTarget.z = 0f;
                lineRenderer.SetPosition(1 , laserTarget);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player") // if the player's object
        {
            target = col.gameObject;
            spotted = true;
            lineRenderer.enabled = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player") // if the player's object
        {
            spotted = false;
            lineRenderer.enabled = false;
        }
    }
}