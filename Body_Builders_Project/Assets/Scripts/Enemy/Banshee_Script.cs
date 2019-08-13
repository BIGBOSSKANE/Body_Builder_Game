using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banshee_Script : MonoBehaviour
{
    bool spotted = false;
    bool isCharging = false;
    bool isFiring = false;
    public GameObject target;
    float  laserChargeTimer = 0f;
    public float laserChargeTime = 1.5f;
    float laserFireTimer = 0f;
    public float laserFireTime = 3f;
    public LayerMask collideableLayers;
    public LayerMask laserLayer;
    public LineRenderer laserLine;
    public GameObject laserOriginPoint;
    public Vector3 laserOrigin;
    public Vector3 laserTarget;
    CircleCollider2D circleCol;
    float laserRange = 100f;
    string laserTag = "none";
    playerScript playerScript;
    //bool windUp;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<playerScript>();
        spotted = false;
        laserOriginPoint = gameObject.transform.Find("laserOrigin").gameObject;
        circleCol = gameObject.GetComponent<CircleCollider2D>();
        laserRange = circleCol.radius;
        laserChargeTimer = 0f;
        laserFireTimer = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        laserLine = gameObject.GetComponent<LineRenderer>();

        if (spotted == true)
        {
            laserOrigin = laserOriginPoint.transform.position;
            laserOrigin.z = 0f;

            Vector2 targetPosition = target.transform.position;

            Vector2 objectPos = transform.position;
            targetPosition.x = targetPosition.x - objectPos.x;
            targetPosition.y = targetPosition.y - objectPos.y;

            float angle = Mathf.Atan2(targetPosition.y, targetPosition.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            laserLine.SetPosition (0 , laserOrigin);

            RaycastHit hit;
            if(Physics.Raycast(laserOrigin , transform.right , out hit , collideableLayers))
            {
                laserTarget = hit.point;
                laserTarget.z = 0f;
                laserLine.SetPosition(1 , laserTarget);
                Debug.Log("Hit");
            }
            else
            {
                laserTarget = laserOrigin + transform.right * laserRange;
                laserTarget.z = 0f;
                laserLine.SetPosition(1 , laserTarget);
            }
        }
/*
        if(isCharging)
        {
            laserChargeTimer += Time.deltaTime;
            Charging();
        }
        else
        {
            laserChargeTimer -= 2f * Time.deltaTime;
        }

        if(isFiring)
        {

        }
*/
    }

/*
            Vector2 targetPosition = target.transform.position;
            Vector2 laserOriginDirection = new Vector2(targetPosition.x - laserOrigin.x , targetPosition.y - laserOrigin.y);
            laserOriginDirection.Normalize();
            RaycastHit2D laser = Physics2D.Raycast(laserOrigin, laserOriginDirection, Mathf.Infinity , laserLayer);
            if(laser.collider != null)
            {
                bool hit;
                Vector2 laserEndpoint = laser.point;
                laserTag = laser.collider.tag;

                laserLine.positionCount = 2;
                laserLine.SetPosition(0 , laserOrigin);
                laserLine.SetPosition(1 , laserEndpoint);

                Vector2 laserCollisionNormal = laser.normal;

                if(laser.collider.tag == "Player")
                {
                    if(laserTag != "Player")
                    {
                        hit = true;
                        playerScript.ManageDeflect(hit , laser.hit.point)
                    }
                    laserTag = laser.collider.tag;
                    isCharging == true;
                    if(laserChargeTimer >= laserChargeTime)
                    {
                        LaserBlast();
                    }
                }
                else
                {
                    hit = false;
                    playerScript.ManageDeflect(hit , laser.hit.point);
                    isCharging == false;
                }

                laserTag = laser.collider.tag;
            }
        }

    void LaserBlast()
    {
        laserFireTimer += 
    }
*/

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player") // if the player's object
        {
            target = col.gameObject;
            spotted = true;
            laserLine.enabled = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player") // if the player's object
        {
            spotted = false;
            laserLine.enabled = false;
        }
    }
}