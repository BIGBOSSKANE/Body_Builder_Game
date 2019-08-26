using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserRouter : MonoBehaviour
{
    public bool canStoreCharge = false;
    bool charged = false;
    bool deathRay = false;
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
    powerCell powerCell;
    powerStation powerStation;
    Vector2 laserEndpoint;

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
        Vector2 laserOriginDirection = (aim.transform.up + aim.transform.right).normalized;
        Vector2 laserOrigin = transform.position * colRadius;
        laserOrigin = new Vector2(laserOrigin.x + transform.position.x , laserOrigin.y + transform.position.y);
        laserOrigin = transform.position;

        RaycastHit2D laser = Physics2D.Raycast(transform.position, laserOriginDirection, Mathf.Infinity , laserLayer);
        if(laser.collider != null)
        {
            laserEndpoint = laser.point;

            laserLine.positionCount = 2;
            laserLine.SetPosition(0 , laserOrigin);
            laserLine.SetPosition(1 , laserEndpoint);

            Vector2 laserCollisionNormal = laser.normal;
            float collisionNormalAngle = Mathf.Atan2(laserCollisionNormal.y , laserCollisionNormal.x);
            if(collisionNormalAngle < 0f)
            {
                collisionNormalAngle = Mathf.PI * 2 + collisionNormalAngle;
            }

            collisionEffect.SetActive(true);
            collisionEffect.transform.position = laser.point;
            collisionEffect.transform.up = Quaternion.Euler(0 , 0 , (collisionNormalAngle * Mathf.Rad2Deg)) * Vector2.right;

            if(laser.collider.tag == "Shield")
            {
                if(laserTag != "Shield") // if the most recent collider hit was not the player
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
            else if(laser.collider.tag == "Player")
            {
                if(laserTag != "Player") // if the most recent collider hit was not the player
                {
                    if(charged)
                    {
                        //Destroy(target);
                        Debug.Log("Killed");
                    }
                }               
            }
            else
            {
                if(playerScript != null && laserTag == "Shield")
                {
                    playerScript.EndDeflect();
                    playerScript.DeathRay(false);
                }
            }

            if(charged)
            {
                if(laser.collider.tag == "Enemy" || laser.collider.tag == "Groundbreakable")
                {
                    Destroy(laser.collider.gameObject);
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
            collisionEffect.SetActive(false);
            laserEndpoint = laserOrigin + (laserOriginDirection * 10000f);
        }
        laserLine.positionCount = 2;
        laserLine.SetPosition(0 , laserOrigin);
        laserLine.SetPosition(1 , laserEndpoint);
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


// may need to change it so that it only gets a charge when hit by the death ray (only one stage), then permanently emits that charge