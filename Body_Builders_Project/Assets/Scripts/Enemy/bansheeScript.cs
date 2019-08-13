using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bansheeScript : MonoBehaviour
{
    bool isCharging = false;
    bool isFiring = false;
    public GameObject target;
    float  laserChargeTimer = 0f;
    public float laserChargeTime = 1.5f;
    float laserFireTimer = 0f;
    public float laserFireTime = 3f;
    public LayerMask collideableLayers;
    public LayerMask laserLayer;
    public LayerMask playerLayer;
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
        target = GameObject.Find("Player");
        playerScript = target.GetComponent<playerScript>();
        laserOriginPoint = gameObject.transform.Find("laserOrigin").gameObject;
        circleCol = gameObject.GetComponent<CircleCollider2D>();
        laserLine = gameObject.GetComponent<LineRenderer>();
        laserChargeTimer = 0f;
        laserFireTimer = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Physics2D.OverlapCircle(gameObject.transform.position, 15f , playerLayer))
        {
            Vector2 targetPosition = target.transform.position;
            laserOrigin = laserOriginPoint.transform.position;

            Vector2 laserOriginDirection = new Vector2(targetPosition.x - laserOrigin.x , targetPosition.y - laserOrigin.y);
            float laserAngle = Mathf.Atan2(laserOriginDirection.y , laserOriginDirection.x);
            if(laserAngle < 0f)
            {
                laserAngle = Mathf.PI * 2 + laserAngle;
            }

            if(!isFiring)
            {
                gameObject.transform.up = Quaternion.Euler(0 , 0 , (laserAngle * Mathf.Rad2Deg) + 90f) * Vector2.right;
            }

            laserOriginDirection.Normalize();

            RaycastHit2D laser = Physics2D.Raycast(laserOrigin, laserOriginDirection, laserRange , laserLayer);
            if(laser.collider != null)
            {
                Vector2 laserEndpoint = laser.point;

                laserLine.positionCount = 2;
                laserLine.SetPosition(0 , laserOrigin);
                laserLine.SetPosition(1 , laserEndpoint);

                Vector2 laserCollisionNormal = laser.normal;

                if(laser.collider.tag == "Player" && isFiring) 
                {
                    Destroy(target);
                    Debug.Log("Killed");
                }
                else if (laser.collider.tag == "Shield" && isFiring)
                {
                    playerScript.firingLaser = true;
                }
                else if(laser.collider.tag == "Shield" || laser.collider.tag == "Player")
                {
                    isCharging = true;

                    if(laserTag != "Shield" && laser.collider.tag != "Player")
                    {
                        playerScript.InitiateDeflect();
                    }

                    if(laser.collider.tag == "Player")
                    {
                        playerScript.EndDeflect();
                    }
                }
                else if(laser.collider.tag != "Shield")
                {
                    playerScript.EndDeflect();
                    isCharging = false;
                }
                laserTag = laser.collider.tag;
            }
        }

        if(isCharging)
        {
            laserChargeTimer += Time.deltaTime;
            if(laserChargeTimer >= laserChargeTime)
            {
                isFiring = true;
            }
            //Charging();
        }
        else
        {
            laserChargeTimer -= 2f * Time.deltaTime;
        }

        if(isFiring)
        {
            LaserBlast();
        }

    }

    void LaserBlast()
    {
        laserFireTimer += Time.deltaTime;
        if(laserFireTimer > laserFireTime)
        {
            isFiring = false;
            laserChargeTime = 0f;
            laserFireTimer = 0f;
        }
    }
}