/*
Creator: Daniel
Created 04/08/2019
Last Edited by: Daniel
Last Edit 01/09/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserRouter : activate
{
    [Tooltip("Use a secondary laser (indicated by the other aim sprite)")] public bool laserSplitter; // does the laser router fire 2 lasers? (the second is projected from the laser aim splitter sprite)
    [Tooltip("Is it firing a laser?")] public bool charged = false; // is the laser router charged?
    bool wasCharged; // was the laser router charged in the last frame? - this is used to send an activate signal to doors/fanse/etc that use the laser router as a switch
    [Tooltip("Can it store a charge after being hit by a laser?")] public bool canStoreCharge = false; // can the laser router store a charge, or does it only route lasers while a laser is colliding with it?
    [Tooltip("Does it rotate when sent a signal to do so?")] public bool canRotate = true; // does the laser router rotate when sent a right or !right signal
    bool rotating; // is the laser router currently rotating?
    [Tooltip("Does it rotate anticlockwise?")] public bool anticlockwise = false; // rotate anticlockwise when receiving a right signal, rather than clockwise
    [Tooltip("Does the laser router rotate or move between points?")] public bool patrol = false; // does the laser router move between points when activated?
    [Tooltip("Is the patrol rotational?")] public bool rotationalPatrol; // does the laser router rotate instead of moving?
    [Tooltip("Does the patrol loop, or move to an endpoint?")] public bool patrolLoop = false; // does the laser router move back and forth between points?
    bool patrolForwards = true; // moving forwards in patrol route
    [Tooltip("Does retriggering a non-looping patrol system cause it to reverse or move to the next stage")] public bool patrolSwitch = false; // if patrol loop is untrue, this means that deactivating the router will cause it to return to the original position
    Vector2 originalPosition; // starting point if moving due to an activate call
    [Tooltip("Where to move to when non-rotationally patrolling")] public Vector2 moveTo; // end point when moving due to an activate call
    Vector2 targetPosition; // the endpoint of the movement, put into world space
    GameObject player; // the player
    protected bool deathRay = false; // will the ray kill?
    protected float colRadius; // radius of the circle collider
    protected GameObject aimSprite; // sprite indicating the aim direction
    GameObject coreSprite; // sprite representing if the laser router is storing a charge
    protected GameObject collisionEffect; // collision burst effect
    protected GameObject burstEffect; // emanating from shield burst effect
    Quaternion targetRotation; // point to rotate towards
    float rotateTimer = 0f; // time tracker for rotation
    float moveTimer = 0f; // time tracker for movement
    [Tooltip("How long does it take to move between points?")] public float moveTimeTotal = 1f; // time taken to move between points
    [Tooltip("How fast does the router rotate?")] public float rotatePatrolSpeed = 1f; // speed of rotation;
    [Tooltip("How long does it take the router to rotate when triggered?")] public float rotateTime = 4f; // duration of switch rotation;
    protected LineRenderer laserLine; // the line renderer for the laser
    protected string laserTag; // name of the thing hit by the laser
    protected playerScript playerScript; // get a reference to the player script
    [Tooltip("Layers the laser can hit")] public LayerMask laserLayer; // what can the laser collide with? 
    protected powerCell powerCell; // the powercell script of the powercell hit
    protected powerStation powerStation; // the script of the powerstation hit
    protected Vector2 laserEndpoint; // the endpoint of the laser
    [Tooltip("")] public GameObject [] activates; // does the laser router activate something while it stores a charge
    Quaternion initialRotation;
    
    GameObject aimSpriteSplit;
    GameObject burstEffectSplit;
    GameObject collisionEffectSplit;
    LineRenderer laserLineSplit;
    Vector2 laserEndpointSplit;

    void Start()
    {
        player = GameObject.Find("Player").gameObject;
        aimSprite = gameObject.transform.Find("aimSprite").gameObject;
        coreSprite = aimSprite.transform.Find("coreSprite").gameObject;
        coreSprite.SetActive(false);
        rotateTimer = 0f;
        collisionEffect = gameObject.transform.Find("collisionEffectPosition").gameObject;
        burstEffect = gameObject.transform.Find("burstEffectPosition").gameObject;
        collisionEffect.SetActive(false);
        burstEffect.SetActive(false);
        colRadius = gameObject.GetComponent<CircleCollider2D>().radius * transform.localScale.x + 0.01f;
        laserLine = aimSprite.GetComponent<LineRenderer>();
        laserLine.enabled = false;
        playerScript= GameObject.Find("Player").GetComponent<playerScript>();
        originalPosition = (Vector2)transform.position;
        targetPosition = moveTo + (Vector2)transform.position;
        wasCharged = false;
        initialRotation = transform.rotation;

        // splitter lasercaster
        aimSpriteSplit = gameObject.transform.Find("aimSpriteSplit").gameObject;
        burstEffectSplit = gameObject.transform.Find("burstEffectSplit").gameObject;
        collisionEffectSplit = gameObject.transform.Find("collisionEffectSplit").gameObject;
        laserLineSplit = aimSpriteSplit.GetComponent<LineRenderer>();
        laserLineSplit.enabled = false;
        collisionEffectSplit.SetActive(false);
        burstEffectSplit.SetActive(false);

        if(laserSplitter)
        {
            aimSpriteSplit.SetActive(true);
        }
        else
        {
            aimSpriteSplit.SetActive(false);
        }
    }

    void Rotate(bool right , float extraRotate)
    {
        int direction = (right)? 1 : -1;
        targetRotation = aimSprite.transform.rotation;
        targetRotation *= Quaternion.Euler(0f, 0f, direction * 90f + extraRotate);
        rotating = true;
    }

    void Update()
    {
        if(rotating == true)
        {
            aimSprite.transform.rotation = Quaternion.Slerp(aimSprite.transform.rotation, targetRotation, rotateTimer);
        }

        rotateTimer += Time.deltaTime / rotateTime;
        if(rotateTimer >= 1f)
        {
            rotating = false; 
        }

        if(charged)
        {
            laserLine.enabled = true;
            if(laserSplitter)
            {
                laserLineSplit.enabled = true;
            }
            LaserCaster();
        }

        if(wasCharged != charged)
        {
            StateChange(charged);
        }

        wasCharged = charged;


        Patrol(); // check if the laser router is being prompted to move
    }

    public virtual void LaserCaster()
    {
        if(laserSplitter)
        {
            SecondaryLaserCaster();
        }
        Vector2 laserOriginDirection = (aimSprite.transform.up + aimSprite.transform.right).normalized;
        Vector2 laserOrigin = (Vector2)transform.position + (laserOriginDirection * colRadius);

        RaycastHit2D laser = Physics2D.Raycast(laserOrigin, laserOriginDirection, Mathf.Infinity , laserLayer);
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

            if(laser.collider.tag == "LaserRouter")
            {
                if(laserTag != "laserRouter")
                {
                    laserRouter laserRouter = laser.transform.gameObject.GetComponent<laserRouter>();
                    laserRouter.Charged();
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
            else if(laser.collider.tag == "HeldBox")
            {
                collisionEffect.transform.position = laser.point;
                if(playerScript.heldBoxTag == "powerCell" && ((laserTag != "HeldBox" && charged) || (!wasCharged && charged)))
                {
                    powerCell powerCell = player.transform.Find("PowerCell").GetComponent<powerCell>();
                    powerCell.activated = true;
                    if(overcharge) powerCell.overcharge = true;
                }
            }
            else if(laser.collider.tag == "Shield")
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
                        playerScript.Die(0.5f);
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
                if((laserTag != "powerCell" && charged) || (!wasCharged && charged))
                {
                    powerCell = laser.transform.gameObject.GetComponent<powerCell>();
                    powerCell.activated = true;
                }
            }
            else if(laser.collider.tag == "PowerStation")
            {
                if(laserTag != "PowerStation")
                {
                    powerStation = laser.transform.gameObject.GetComponent<powerStation>();
                    powerStation.activated = true;
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

        float laserAngle = Mathf.Atan2(laserOriginDirection.y , laserOriginDirection.x); // apply the laser burst effect
        if(laserAngle < 0f)
        {
            laserAngle = Mathf.PI * 2 + laserAngle;
        }
            
        burstEffect.SetActive(true);
        burstEffect.transform.position = (Vector2)transform.position + (laserOriginDirection * (colRadius - 0.01f));
        burstEffect.transform.up = Quaternion.Euler(0 , 0 , (laserAngle * Mathf.Rad2Deg)) * Vector2.right;
    }

    void SecondaryLaserCaster()
    {
        Vector2 laserOriginDirection = (aimSpriteSplit.transform.up + aimSpriteSplit.transform.right).normalized;
        Vector2 laserOrigin = (Vector2)transform.position + (laserOriginDirection * colRadius);

        RaycastHit2D laser = Physics2D.Raycast(laserOrigin, laserOriginDirection, Mathf.Infinity , laserLayer);
        if(laser.collider != null)
        {
            laserEndpointSplit = laser.point;

            laserLineSplit.positionCount = 2;
            laserLineSplit.SetPosition(0 , laserOrigin);
            laserLineSplit.SetPosition(1 , laserEndpointSplit);

            Vector2 laserCollisionNormal = laser.normal;
            float collisionNormalAngle = Mathf.Atan2(laserCollisionNormal.y , laserCollisionNormal.x);
            if(collisionNormalAngle < 0f)
            {
                collisionNormalAngle = Mathf.PI * 2 + collisionNormalAngle;
            }

            collisionEffectSplit.SetActive(true);
            collisionEffectSplit.transform.position = laser.point;
            collisionEffectSplit.transform.up = Quaternion.Euler(0 , 0 , (collisionNormalAngle * Mathf.Rad2Deg)) * Vector2.right;

            if(laser.collider.tag == "LaserRouter")
            {
                if(laserTag != "laserRouter")
                {
                    laserRouter laserRouter = laser.transform.gameObject.GetComponent<laserRouter>();
                    laserRouter.Charged();
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
            else if(laser.collider.tag == "HeldBox")
            {
                collisionEffect.transform.position = laser.point;
                if(playerScript.heldBoxTag == "powerCell" && ((laserTag != "HeldBox" && charged) || (!wasCharged && charged)))
                {
                    powerCell powerCell = player.transform.Find("PowerCell").GetComponent<powerCell>();
                    powerCell.activated = true;
                    if(overcharge) powerCell.overcharge = true;
                }
            }
            else if(laser.collider.tag == "Shield")
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
                        playerScript.Die(0.5f);
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
                if((laserTag != "powerCell" && charged) || (!wasCharged && charged))
                {
                    powerCell = laser.transform.gameObject.GetComponent<powerCell>();
                    powerCell.activated = true;
                }
            }
            else if(laser.collider.tag == "PowerStation")
            {
                if(laserTag != "PowerStation")
                {
                    powerStation = laser.transform.gameObject.GetComponent<powerStation>();
                    powerStation.activated = true;
                }
            }
            
            laserTag = laser.collider.tag;
        }
        else
        {
            collisionEffect.SetActive(false);
            laserEndpointSplit = laserOrigin + (laserOriginDirection * 10000f);
        }
        laserLineSplit.positionCount = 2;
        laserLineSplit.SetPosition(0 , laserOrigin);
        laserLineSplit.SetPosition(1 , laserEndpointSplit);

        float laserAngle = Mathf.Atan2(laserOriginDirection.y , laserOriginDirection.x); // apply the laser burst effect
        if(laserAngle < 0f)
        {
            laserAngle = Mathf.PI * 2 + laserAngle;
        }
            
        burstEffectSplit.SetActive(true);
        burstEffectSplit.transform.position = (Vector2)transform.position + (laserOriginDirection * (colRadius - 0.01f));
        burstEffectSplit.transform.up = Quaternion.Euler(0 , 0 , (laserAngle * Mathf.Rad2Deg)) * Vector2.right;
    }

    public void Charged()
    {
        charged = true;
        coreSprite.SetActive(true);
        laserLine.enabled = true;
        if(laserSplitter)
        {
           laserLineSplit.enabled = true;
        }
    }

    public void Drained()
    {
        if(!canStoreCharge)
        {
            charged = false;
            coreSprite.SetActive(false);
            laserLine.enabled = false;
            if(laserSplitter)
            {
                laserLineSplit.enabled = true;
            }
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

    void Patrol()
    {
        if(patrol)
        {
            if(rotationalPatrol) // rotate
            {
                if(activated)
                {
                    if(patrolForwards)
                    {
                        transform.Rotate(0f , 0f, rotatePatrolSpeed * Time.deltaTime);
                    }
                    else
                    {
                        transform.Rotate(0f , 0f, -rotatePatrolSpeed * Time.deltaTime);
                    }
                }       
            }
            else
            {
                if(patrolLoop) // the router will move back and forwards while activated
                {
                    if(activated)
                    {
                        if(patrolForwards)
                        {
                            moveTimer += Time.deltaTime / moveTimeTotal;
                            if(moveTimer > 1)
                            {
                                moveTimer = 1;
                                patrolForwards = false;
                            }
                            gameObject.transform.position = Vector2.Lerp(originalPosition , targetPosition , moveTimer);
                        }
                        else
                        {
                            moveTimer -= Time.deltaTime / moveTimeTotal;
                            if(moveTimer < 0f)
                            {
                                moveTimer = 0f;
                                patrolForwards = true;
                            }
                            gameObject.transform.position = Vector2.Lerp(originalPosition , targetPosition , moveTimer);
                        }
                    }   
                }
                else // movement occurs at set increments
                {
                    if(patrolSwitch) // when deactivated, move back to the original position
                    {
                        if(activated)
                        {
                            moveTimer += Time.deltaTime / moveTimeTotal;
                            if(moveTimer > 1)
                            {
                                moveTimer = 1;
                            }
                            gameObject.transform.position = Vector2.Lerp(originalPosition , targetPosition , moveTimer);
                        }
                        else
                        {
                            moveTimer -= Time.deltaTime / moveTimeTotal;
                            if(moveTimer < 0f)
                            {
                                moveTimer = 0f;
                            }
                            gameObject.transform.position = Vector2.Lerp(originalPosition , targetPosition , moveTimer);
                        }
                    }
                    else // cannot move back
                    {
                        if(activated)
                        {
                            moveTimer += Time.deltaTime / moveTimeTotal;
                            if(moveTimer > 1)
                            {
                                moveTimer = 1;
                            }
                            gameObject.transform.position = Vector2.Lerp(originalPosition , targetPosition , moveTimer);
                        }
                    }
                }
            }
        }
    }


    void StateChange(bool active)
    {
        foreach (GameObject activateable in activates)
        {
            if(activateable != null)
            {
                activate activateScript = activateable.GetComponent<activate>();
                activateScript.Activate(active);
                // don't need to send a signal to change the right variable
                if(overcharge)
                {
                    activateScript.Overcharge(charged);
                }
            }
        }
    }

    override public void ActivateDirection(bool right)
    {
        if(anticlockwise)
        {
            right = !right;
        }

        float currentRotation = (rotating)? (targetRotation.eulerAngles - aimSprite.transform.rotation.eulerAngles).z : 0;

        rotateTimer = 0f;
        Rotate(right , currentRotation);
    }

    void OnDrawGizmos() // shows the waypoints in both editor and in game
    {
        if(!rotationalPatrol || !patrol)
        {
            Gizmos.color = Color.blue;
            float locationIdentifier = 0.3f;
            

            if(Application.isPlaying)
            {
                Gizmos.DrawLine(targetPosition - Vector2.up * locationIdentifier , targetPosition + Vector2.up * locationIdentifier);
                Gizmos.DrawLine(targetPosition - Vector2.left * locationIdentifier , targetPosition + Vector2.left * locationIdentifier);
                Gizmos.DrawLine(originalPosition - Vector2.up * locationIdentifier , originalPosition + Vector2.up * locationIdentifier);
                Gizmos.DrawLine(originalPosition - Vector2.left * locationIdentifier , originalPosition + Vector2.left * locationIdentifier);

                Gizmos.DrawLine(originalPosition , targetPosition); // draw a line so we can see where the laser router moves
            }
            else
            {
                targetPosition = moveTo + (Vector2)transform.position;
                Gizmos.DrawLine(targetPosition - Vector2.up * locationIdentifier , targetPosition + Vector2.up * locationIdentifier);
                Gizmos.DrawLine(targetPosition - Vector2.left * locationIdentifier , targetPosition + Vector2.left * locationIdentifier);
                Gizmos.DrawLine((Vector2)transform.position - Vector2.up * locationIdentifier , (Vector2)transform.position + Vector2.up * locationIdentifier);
                Gizmos.DrawLine((Vector2)transform.position - Vector2.left * locationIdentifier , (Vector2)transform.position + Vector2.left * locationIdentifier);

                Gizmos.DrawLine(transform.position , targetPosition); // draw a line so we can see where the laser router moves
            }
        }
    }
}


// may need to change it so that it only gets a charge when hit by the death ray (only one stage), then permanently emits that charge