using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserRouter : MonoBehaviour
{
    public bool canStoreCharge = false;
    bool charged = false;
    public bool deathRay = false;
    float colRadius;
    GameObject aim;
    GameObject core;
    GameObject collisionEffect;
    Quaternion targetRotation;
    public bool rotating;
    float rotateTimer = 0f;
    bool clockwiseButtonUntriggered = true;
    bool anticlockwiseButtonUntriggered = true;
    buttonSwitchScript clockwiseButton;
    buttonSwitchScript anticlockwiseButton;
    LineRenderer laserLine;
    string laserTag;
    playerScript playerScript;
    public LayerMask laserLayer;

    void Start()
    {
        aim = gameObject.transform.Find("Direction").gameObject;
        core = gameObject.transform.Find("Direction").gameObject.transform.Find("Core").gameObject;
        core.SetActive(false);
        rotateTimer = 0f;
        collisionEffect = gameObject.transform.Find("collisionEffectPosition").gameObject;
        collisionEffect.SetActive(false);
        clockwiseButton = gameObject.transform.Find("clockwiseTrigger").gameObject.GetComponent<buttonSwitchScript>();
        anticlockwiseButton = gameObject.transform.Find("anticlockwiseTrigger").gameObject.GetComponent<buttonSwitchScript>();
        colRadius = gameObject.GetComponent<CircleCollider2D>().radius * transform.localScale.x;
        laserLine = gameObject.GetComponent<LineRenderer>();
        laserLine.enabled = false;
        playerScript= GameObject.Find("Player").GetComponent<playerScript>();
    }

    public void RotateAnticlockwise()
    {
        targetRotation = aim.transform.rotation;
        targetRotation *= Quaternion.Euler(0f, 0f, 90f);
        rotating = true;
    }

    public void RotateClockwise()
    {
        targetRotation = aim.transform.rotation;
        targetRotation *= Quaternion.Euler(0f, 0f, -90f);
        rotating = true;
    }

    void Update()
    {
        if(rotating == true)
        {
            aim.transform.rotation = Quaternion.Slerp(aim.transform.rotation, targetRotation, rotateTimer);
        }
        else
        {
            if(clockwiseButton.triggered && clockwiseButtonUntriggered)
            {
                rotateTimer = 0f;
                RotateClockwise();
            }
            else if(anticlockwiseButton.triggered && anticlockwiseButtonUntriggered)
            {
                rotateTimer = 0f;
                RotateAnticlockwise();
            }
        }

        rotateTimer += Time.deltaTime;
        if(rotateTimer >= 1f)
        {
            rotating = false; 
        }

        if(clockwiseButton.triggered == false)
        {
            clockwiseButtonUntriggered = true;
        }
        else
        {
            clockwiseButtonUntriggered = false;
        }

        if(anticlockwiseButton.triggered == false)
        {
            anticlockwiseButtonUntriggered = true;
        }
        else
        {
            anticlockwiseButtonUntriggered = false;
        }

        if(charged)
        {
            LaserCaster();
        }
    }

    void LaserCaster()
    {
        Vector2 laserOriginDirection = aim.transform.up;
        laserOriginDirection.Normalize();
        Vector2 laserOrigin = aim.transform.position * colRadius;
        laserOrigin = new Vector2(laserOrigin.x + transform.position.x , laserOrigin.y + transform.position.y);
        RaycastHit2D laser = Physics2D.Raycast(laserOrigin, laserOriginDirection, Mathf.Infinity , laserLayer);
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

            collisionEffect.transform.position = laser.point;
            collisionEffect.transform.up = Quaternion.Euler(0 , 0 , (collisionNormalAngle * Mathf.Rad2Deg)) * Vector2.right;

            if(laser.collider.tag == "Player")
            {
                if(laserTag != "Player")
                {
                    playerScript playerScript = laser.transform.gameObject.GetComponent<playerScript>();
                    playerScript.InitiateDeflect();
                    if(deathRay)
                    {
                        playerScript.DeathRay(true);
                    }
                    else
                    {
                        playerScript.DeathRay(false);
                    }
                }
            }
            else
            {
                if(playerScript != null)
                {
                    playerScript.EndDeflect();
                    playerScript.DeathRay(false);
                }
            }

            /*                                          // add this later for charging power cells
            if(laser.collider.tag == "PowerCell")
            {
                if(laserTag != "PowerCell")
                {
                    powerCell = laser.transform.gameObject.GetComponent<powerCell>();
                    powerCell.Charged();
                    if(deathRay)
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
                if(powercell != null)
                {
                    laserRouter.Drained();
                    laserRouter.DeathRay(false);
                }
            }          
            */

            if(charged)
            {
                if(laser.collider.tag == "Enemy" || laser.collider.tag == "Groundbreakable")
                {
                    Destroy(laser.collider.gameObject);
                }
            }
            
            laserTag = laser.collider.tag;
        }
    }

    public void Charged()
    {
        charged = true;
        core.SetActive(true);
        laserLine.enabled = true;
    }

    public void Drained()
    {
        if(!canStoreCharge)
        {
            charged = false;
            core.SetActive(false);
            laserLine.enabled = false;
        }
    }

    public void DeathRay(bool death)
    {
        if(death)
        {
            deathRay = true;
        }
        else
        {
            deathRay = false;
        }
    }
}
