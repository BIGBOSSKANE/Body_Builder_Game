using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserRouter : activate
{
    public bool charged = false; // is the laser router charged?
    bool wasCharged; // was the laser router charged in the last frame? - this is used to send an activate signal to doors/fanse/etc that use the laser router as a switch
    public bool canStoreCharge = false; // can the laser router store a charge, or does it only route lasers while a laser is colliding with it?
    public bool canRotate = true; // does the laser router rotate when sent a right or !right signal
    bool rotating; // is the laser router currently rotating?
    public bool reverseRotate = false; // rotate anticlockwise when receiving a right signal, rather than clockwise
    public bool patrol = false; // does the laser router move between points when activated?
    public bool patrolLoop = false; // does the laser router move back and forth between points?
    public bool patrolReverseOnReactivate = false; // if patrol loop is untrue, this means that pressing a button again will cause the laser Router to move to the other position each time it is pressed
    Vector2 originalPosition; // starting point if moving due to an activate call
    public Vector2 moveTo; // end point when moving due to an activate call
    Vector2 targetPosition; // the endpoint of the movement, put into world space
    bool deathRay = false; // will the ray kill?
    float colRadius; // radius of the circle collider
    GameObject aimSprite; // sprite indicating the aim direction
    GameObject coreSprite; // sprite representing if the laser router is storing a charge
    GameObject collisionEffect;
    Quaternion targetRotation;
    float rotateTimer = 0f;
    LineRenderer laserLine;
    string laserTag;
    playerScript playerScript;
    public LayerMask laserLayer;
    powerCell powerCell;
    powerStation powerStation;
    Vector2 laserEndpoint;
    public GameObject [] activates; // does the laser router activate something while it stores a charge

    void Start()
    {
        aimSprite = gameObject.transform.Find("Direction").gameObject;
        coreSprite = gameObject.transform.Find("Direction").gameObject.transform.Find("coreSprite").gameObject;
        coreSprite.SetActive(false);
        rotateTimer = 0f;
        collisionEffect = gameObject.transform.Find("collisionEffectPosition").gameObject;
        collisionEffect.SetActive(false);
        colRadius = gameObject.GetComponent<CircleCollider2D>().radius * transform.localScale.x;
        laserLine = gameObject.GetComponent<LineRenderer>();
        laserLine.enabled = false;
        playerScript= GameObject.Find("Player").GetComponent<playerScript>();
        originalPosition = (Vector2)transform.position;
        targetPosition = moveTo + (Vector2)transform.position;
        wasCharged = false;
    }

    public void RotateAnticlockwise()
    {
        targetRotation = aimSprite.transform.rotation;
        targetRotation *= Quaternion.Euler(0f, 0f, 90f);
        rotating = true;
    }

    public void RotateClockwise()
    {
        targetRotation = aimSprite.transform.rotation;
        targetRotation *= Quaternion.Euler(0f, 0f, -90f);
        rotating = true;
    }

    void Update()
    {
        if(rotating == true)
        {
            aimSprite.transform.rotation = Quaternion.Slerp(aimSprite.transform.rotation, targetRotation, rotateTimer);
        }

        rotateTimer += Time.deltaTime;
        if(rotateTimer >= 1f)
        {
            rotating = false; 
        }

        if(charged)
        {
            LaserCaster();
        }


        if(wasCharged != charged)
        {
            StateChange(charged);
        }

        wasCharged = charged;
    }

    void LaserCaster()
    {
        Vector2 laserOriginDirection = (aimSprite.transform.up + aimSprite.transform.right).normalized;
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
        coreSprite.SetActive(true);
        laserLine.enabled = true;
    }

    public void Drained()
    {
        if(!canStoreCharge)
        {
            charged = false;
            coreSprite.SetActive(false);
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


    void StateChange(bool active)
    {
        foreach (GameObject activateable in activates)
        {
            if(activateable != null)
            {
                activateable.GetComponent<activate>().Activate(active);
            }
        }
    }

    override public void ActivateDirection(bool right)
    {
        if(canRotate)
        {
            if(reverseRotate)
            {
                right = !right;
            }

            if(right)
            {
                rotateTimer = 0f;
                RotateClockwise();
            }
            else
            {
                rotateTimer = 0f;
                RotateAnticlockwise();
            }
        }
    }

    void OnDrawGizmos() // shows the waypoints in both editor and in game
    {
        Gizmos.color = Color.blue;
        float locationIdentifier = 0.3f;
        
        Gizmos.DrawLine(targetPosition - Vector2.up * locationIdentifier , targetPosition + Vector2.up * locationIdentifier);
        Gizmos.DrawLine(targetPosition - Vector2.left * locationIdentifier , targetPosition + Vector2.left * locationIdentifier);
        Gizmos.DrawLine(originalPosition - Vector2.up * locationIdentifier , originalPosition + Vector2.up * locationIdentifier);
        Gizmos.DrawLine(originalPosition - Vector2.left * locationIdentifier , originalPosition + Vector2.left * locationIdentifier);
        Gizmos.DrawLine(originalPosition , targetPosition); // draw a line so we can see where the laser router moves
    }
}


// may need to change it so that it only gets a charge when hit by the death ray (only one stage), then permanently emits that charge