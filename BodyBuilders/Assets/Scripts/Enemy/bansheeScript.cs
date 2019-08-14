using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bansheeScript : MonoBehaviour
{
    bool isCharging = false;
    bool isFiring = false;
    GameObject target;
    GameObject collisionEffect;
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
    float laserAngle;
    playerScript playerScript;
    Vector2 targetPosition;
    Vector2 laserOriginDirection;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player");
        playerScript = target.GetComponent<playerScript>();
        laserOriginPoint = gameObject.transform.Find("laserOrigin").gameObject;
        collisionEffect = gameObject.transform.Find("collisionEffectPosition").gameObject;
        collisionEffect.SetActive(false);
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
            collisionEffect.SetActive(true);
            laserLine.enabled = true;

            if(!isFiring)
            {
                targetPosition = target.transform.position;
                laserOrigin = laserOriginPoint.transform.position;
                laserOriginDirection = new Vector2(targetPosition.x - laserOrigin.x , targetPosition.y - laserOrigin.y);
                laserAngle = Mathf.Atan2(laserOriginDirection.y , laserOriginDirection.x);
                if(laserAngle < 0f)
                {
                    laserAngle = Mathf.PI * 2 + laserAngle;
                }
                gameObject.transform.up = Quaternion.Euler(0 , 0 , (laserAngle * Mathf.Rad2Deg) + 90f) * Vector2.right;
                laserOriginDirection.Normalize();
            }

            RaycastHit2D laser = Physics2D.Raycast(laserOrigin, laserOriginDirection, laserRange , laserLayer);
            if(laser.collider != null)
            {
                Vector2 laserEndpoint = laser.point;

                laserLine.positionCount = 2;
                laserLine.SetPosition(0 , laserOrigin);
                laserLine.SetPosition(1 , laserEndpoint);

                Vector2 laserCollisionNormal = laser.normal;
                float collisionNormalAngle = Mathf.Atan2(laserCollisionNormal.y , laserCollisionNormal.x);
                if(collisionNormalAngle < 0f)
                {
                    collisionNormalAngle = Mathf.PI * 2 + collisionNormalAngle;
                }

                collisionEffect.transform.up = Quaternion.Euler(0 , 0 , (collisionNormalAngle * Mathf.Rad2Deg)) * Vector2.right;

                if(laser.collider.tag == "Player")
                {
                    collisionEffect.transform.position = targetPosition;
                    playerScript.DeathRay(false);
                    playerScript.EndDeflect();
                    if(isFiring == true)
                    {
                        //Destroy(target);
                        Debug.Log("Killed");
                    }
                    else
                    {
                        isCharging = true;
                    }
                }
                else if(laser.collider.tag == "Shield")
                {
                    collisionEffect.transform.position = laser.point;
                    if(laserTag != "Shield")
                    {
                        playerScript.InitiateDeflect();
                        laserChargeTimer = 0f;
                    }

                    if(isFiring == true)
                    {
                        playerScript.DeathRay(true);
                    }
                    else
                    {
                        playerScript.DeathRay(false);
                        isCharging = true;
                    }
                }
                else
                {
                    playerScript.DeathRay(false);
                    playerScript.EndDeflect();
                    isCharging = false;
                    collisionEffect.transform.position = laser.point;
                }
                laserTag = laser.collider.tag;
            }
        }
        else
        {
            isCharging = false;
            collisionEffect.SetActive(false);
            laserLine.enabled = false;
            laserChargeTimer -= 2f * Time.deltaTime;
        }

        if(isCharging)
        {
            isFiring = false;
            laserFireTimer = 0f;
            laserChargeTimer += Time.deltaTime;
            if(laserChargeTimer >= laserChargeTime)
            {
                isFiring = true;
            }
        }

        if(isFiring)
        {
            isCharging = false;
            laserChargeTimer = 0f;
            laserFireTimer += Time.deltaTime;
            if(laserFireTimer > laserFireTime)
            {
                isFiring = false;
                laserFireTimer = 0f;
            }
        }

    }
}