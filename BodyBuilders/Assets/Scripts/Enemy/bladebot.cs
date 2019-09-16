using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bladebot : MonoBehaviour
{
    public float patrolDuration = 4f; // how long does it take to move to each patrol point
    float patrolTimer = 0f; // current patrol time
    public float pursuitDelay = 0.5f; // how long after seeing the player does the blade bot take to move
    float pursuitDelayTimer = 1f; // time charging before the bladebot pursues the player
    bool pursuitDelaying;
    public float pursuitSpeed = 2f; // the speed at which the blade moves when pursuing the player
    public float disengageSpeed = 1.5f; // the speed at which the blade returns to its original position
    float disengageTimer;
    float disengageMaxTime;
    bool verticalPatrol = true; // does the bladebot move vertically or horizontally? (it will aim at a 90 degree angle from the patrol route)
    public bool positiveAim = true; // is the bladebot aiming at the positive direction (right when moving vertically or up when moving horizontally)? if not, then it will be left or down
    bool pursuit = false; // is the bladebot pursuing the player?
    bool disengage = false; // is the bladebot returning to it's original position?
    Rigidbody2D rb; // this kinematic rigidbody
    public GameObject spinningBlade; // purely visual sprite for the spinning blades
    Vector2 aimDirection; // direction of laser
    Vector2 patrolPoint1; // childed gameObject that determines first patrol route point
    Vector2 patrolPoint2; // childed gameObject that determines the second patrol route point
    Vector2 targetPoint; // the position 
    Vector2 lastPatrolPoint;
    float patrolDistance;
    public LayerMask laserLayer;
    LineRenderer lineRenderer;
    float radius;
    playerScript playerScript;

    void Start()
    {
        pursuit = false;
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        radius = gameObject.GetComponent<CircleCollider2D>().radius;
        patrolPoint1 = GameObject.Find("patrolPoint1").gameObject.transform.position;
        patrolPoint2 = GameObject.Find("patrolPoint2").gameObject.transform.position;
        playerScript = GameObject.Find("Player").gameObject.GetComponent<playerScript>(); // we can swap this out for the scene manager once it has been added

        if (Mathf.Abs(patrolPoint2.x - patrolPoint1.x) > Mathf.Abs(patrolPoint2.y - patrolPoint1.y))
        {
            verticalPatrol = false;
        }
        else
        {
            verticalPatrol = true;
        }

        rb = gameObject.GetComponent<Rigidbody2D>();
        if(verticalPatrol)
        {
            float averageX = (patrolPoint1.x + patrolPoint2.x) / 2f;
            patrolPoint1.x = averageX;
            patrolPoint2.x = averageX;
            patrolDistance = patrolPoint2.x - patrolPoint1.x;

            if(positiveAim)
            {
                aimDirection = Vector2.right;
            }
            else
            {
                aimDirection = -Vector2.right;
            }
        }
        else // if patrolling horizontally
        {
            float averageY = (patrolPoint1.y + patrolPoint2.y) / 2f;
            patrolPoint1.y = averageY;
            patrolPoint2.y = averageY;
            patrolDistance = patrolPoint2.y - patrolPoint1.y;

            if(positiveAim)
            {
                aimDirection = Vector2.up;
            }
            else
            {
                aimDirection = Vector2.down;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        LaserCaster();

        if(pursuitDelayTimer < pursuitDelay && pursuitDelaying)
        {
            pursuitDelayTimer += Time.deltaTime;
            spinningBlade.transform.Rotate(Vector3.forward * ((60f * (pursuitDelayTimer / pursuitDelay)) + 0.01f) * Time.deltaTime);
        }
        else if(pursuitDelayTimer >= pursuitDelay )
        {
            pursuit = true;
            spinningBlade.transform.Rotate(Vector3.forward * 60f * Time.deltaTime);
            pursuitDelaying = false;
        }
        
        if(disengage)
        {
            disengageTimer -= Time.deltaTime;
            if(disengageTimer > 0f)
            {
                float disengagingTimer = disengageTimer/disengageMaxTime;
                transform.position = Vector2.Lerp(lastPatrolPoint , targetPoint , disengagingTimer);
            }
            else
            {
                transform.position = lastPatrolPoint;
                disengage = false;
            }
        }
        else
        {
            patrolTimer += Time.deltaTime / patrolDuration;
            if(verticalPatrol)
            {
                gameObject.transform.position = new Vector2(transform.position.x , patrolPoint1.y + patrolDistance*Mathf.PingPong(patrolTimer , 1f));
            }
            else
            {
                gameObject.transform.position = new Vector2(patrolPoint1.x + patrolDistance*Mathf.PingPong(patrolTimer , 1f) , transform.position.y);
            }
        }
    }

    void StartPursuit(Vector2 target)
    {
        lastPatrolPoint = gameObject.transform.position;
        pursuitDelaying = true;
        pursuitDelayTimer = 0f;
        targetPoint = target;
    }

    void Disengage()
    {
        disengage = true;
        pursuit = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Groundbreakable" || col.tag == "Enemy")
        {
            Destroy(col.gameObject);
        }

       if(col.tag == "Player")
        {
            playerScript.Die(0.2f);
        }
    }

    void LaserCaster()
    {
        RaycastHit2D laser = Physics2D.Raycast(transform.position, aimDirection, Mathf.Infinity , laserLayer);

        // potentially add more lasers (each offset by the "radius" float in terms of height when moving vertically, and width when moving horizontally)
        // draw the actual line renderer from the central laser only
        // if any hit the player, while others are hitting environment, move to avoid the environment then move
        // if any of them hit environment in a close enough range while moving (radius + 0.5f), then stop, and disengage

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0 , transform.position);

        if(laser.collider != null)
        {
            lineRenderer.SetPosition(1 , laser.point);

            if(laser.collider.tag == "Shield" || laser.collider.tag == "Player")
            {
                pursuitDelayTimer = 0f;
                pursuitDelaying = true;
                StartPursuit(laser.collider.gameObject.transform.position);
            }
            else if(pursuit && Vector2.Distance(transform.position , laser.point) < 2f && (laser.collider.tag == "Environment" || laser.collider.tag == "WallJump" || laser.collider.tag == "Climbable" || laser.collider.tag == "Untagged"))
            {
                targetPoint = transform.position;
                disengageTimer = Vector2.Distance((Vector2)transform.position , lastPatrolPoint)/disengageSpeed;
                disengageMaxTime = disengageTimer;
                Disengage();
            }
        }
        else
        {
            lineRenderer.SetPosition(1 , (Vector2)transform.position + (aimDirection * 1000f));
        }
    }
}
