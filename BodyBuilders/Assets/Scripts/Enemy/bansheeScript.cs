using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bansheeScript : MonoBehaviour
{
    bool isCharging = false;
    bool isFiring = false;
    GameObject target;
    GameObject collisionEffect;
    GameObject burstEffect;
    float  laserChargeTimer = 0f;
    public float laserChargeTime = 1.5f;
    float laserFireTimer = 0f;
    public float laserFireTime = 3f;
    public LayerMask laserLayer;
    public LayerMask playerLayer;
    LineRenderer laserLine;
    GameObject laserOriginPoint;
    Vector2 laserOrigin;
    CircleCollider2D circleCol;
    float laserRange = 100f;
    string laserTag = "none";
    float laserAngle;
    playerScript playerScript;
    laserRouter laserRouter;
    Vector2 targetPosition;
    Vector2 laserOriginDirection;
    Vector2 laserEndpoint;
    powerCell powerCell;
    powerStation powerStation;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player");
        playerScript = target.GetComponent<playerScript>();
        laserOriginPoint = gameObject.transform.Find("laserOrigin").gameObject;
        collisionEffect = gameObject.transform.Find("collisionEffectPosition").gameObject;
        burstEffect = gameObject.transform.Find("burstEffectPosition").gameObject;
        collisionEffect.SetActive(false);
        burstEffect.SetActive(false);
        circleCol = gameObject.GetComponent<CircleCollider2D>();
        laserLine = gameObject.GetComponent<LineRenderer>();
        laserLine.enabled = true;
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
                laserEndpoint = laser.point;
                collisionEffect.SetActive(true);

                laserLine.positionCount = 2;
                laserLine.SetPosition(0 , laserOrigin);
                laserLine.SetPosition(1 , laserEndpoint);

                Vector2 laserCollisionNormal = laser.normal;
                float collisionNormalAngle = Mathf.Atan2(laserCollisionNormal.y , laserCollisionNormal.x);
                if(collisionNormalAngle < 0f)
                {
                    collisionNormalAngle = Mathf.PI * 2 + collisionNormalAngle;
                }

                if(playerScript.partConfiguration == 1 && laser.collider.tag == "Player")
                {
                    collisionEffect.transform.up = Quaternion.Euler(0 , 0 , (laserAngle * Mathf.Rad2Deg) + 180f) * Vector2.right;
                }
                else
                {
                    collisionEffect.transform.up = Quaternion.Euler(0 , 0 , (collisionNormalAngle * Mathf.Rad2Deg)) * Vector2.right;
                }

                if(laser.collider.tag == "Player") // if hitting the player and they do not have the deflector shield
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
                else if(laser.collider.tag == "Shield") // if it hits the player's force field shield
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
                else if(laser.collider.tag == "LaserRouter") // if it hits a laser router
                {
                    if(laserTag != "laserRouter")
                    {
                        laserRouter = laser.transform.gameObject.GetComponent<laserRouter>();
                        laserRouter.Charged();
                        if(isFiring == true)
                        {
                            laserRouter.DeathRay(true);
                        }
                        else
                        {
                            laserRouter.DeathRay(false);
                        }
                    }
                }
                else
                {
                    if(laserRouter != null && laserTag == "laserRouter")
                    {
                        laserRouter.Drained();
                        laserRouter.DeathRay(false);
                    }

                    if(laserTag == "Shield")
                    {
                        playerScript.DeathRay(false);
                        playerScript.EndDeflect();
                    }

                    isCharging = false;

                    if(laser.collider.tag == "Box" || laser.collider.tag == "Enemy")
                    {
                        collisionEffect.transform.position = laser.collider.transform.position;
                    }
                    else
                    {
                        collisionEffect.transform.position = laser.point;
                    }
                }

                if(laser.collider.tag == "powerCell")
                {
                    if(laserTag != "powerCell")
                    {
                        powerCell = laser.transform.gameObject.GetComponent<powerCell>();
                        powerCell.charged = true;
                    }
                }
                else if(laser.collider.tag == "PowerStation")
                {
                    if(laserTag != "PowerStation")
                    {
                        powerStation = laser.transform.gameObject.GetComponent<powerStation>();
                        powerStation.energised = true;
                    }
                }
                laserTag = laser.collider.tag;
            }
            else
            {
                laserTag = "null";
                collisionEffect.SetActive(false);
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

        burstEffect.SetActive(false);
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
   
            float laserAngle = Mathf.Atan2(laserOriginDirection.y , laserOriginDirection.x); // apply the laser burst effect
            if(laserAngle < 0f)
            {
                laserAngle = Mathf.PI * 2 + laserAngle;
            }
                
            burstEffect.SetActive(true);
            burstEffect.transform.position = laserOrigin;
            burstEffect.transform.up = Quaternion.Euler(0 , 0 , (laserAngle * Mathf.Rad2Deg)) * Vector2.right;
        }
    }
}