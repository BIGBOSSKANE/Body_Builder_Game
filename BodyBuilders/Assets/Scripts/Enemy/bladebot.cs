using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bladebot : MonoBehaviour
{
    public float patrolDuration = 4f;
    float patrolTimer = 0f;
    public float pursuitDelay = 0.5f; // how long after seeing the player does the blade bot take to move
    float pursuitDelayTimer = 1f;
    public float pursuitSpeed = 2f;
    public float disengeSpeed = 1.5f;
    bool verticalPatrol = true; // does the bladebot move vertically or horizontally? (it will aim at a 90 degree angle from the patrol route)
    public bool positiveAim = true; // is the bladebot aiming at the positive access (right or up)?
    bool pursuit = false;
    bool disengage = false;
    Rigidbody2D rb;
    public GameObject spinningBlade;
    Vector2 aimDirection;
    Vector2 patrolPoint1;
    Vector2 patrolPoint2;
    Vector2 targetPoint;
    Vector2 lastPatrolPoint;
    float patrolDistance;
    public LayerMask laserLayer;
    LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        patrolPoint1 = GameObject.Find("patrolPoint1").gameObject.transform.position;
        patrolPoint2 = GameObject.Find("patrolPoint2").gameObject.transform.position;

        if(Mathf.Abs(patrolPoint2.x - patrolPoint1.x) > Mathf.Abs(patrolPoint2.y - patrolPoint1.y))
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

        if(pursuitDelayTimer < pursuitDelay)
        {
            pursuitDelayTimer += Time.deltaTime;
            pursuit = true;
            spinningBlade.transform.Rotate(Vector3.forward * ((60f * (pursuitDelayTimer / pursuitDelay)) + 0.01f) * Time.deltaTime);
        }
        else if(pursuit)
        {
            rb.velocity = aimDirection * pursuitSpeed;
            spinningBlade.transform.Rotate(Vector3.forward * 60f * Time.deltaTime);
        }
        else if(disengage)
        {
            if(Vector2.Distance((Vector2)transform.position , lastPatrolPoint) > 2f)
            {
                rb.velocity = -aimDirection * disengeSpeed;
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
        if(col.tag == "Player" || col.tag == "Groundbreakable" || col.tag == "Enemy")
        {
            // kill things here
        }
    }

    void LaserCaster()
    {
        RaycastHit2D laser = Physics2D.Raycast(transform.position, aimDirection, Mathf.Infinity , laserLayer);

        // potentially add more lasers

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0 , transform.position);

        if(laser.collider != null)
        {
            lineRenderer.SetPosition(1 , laser.point);

            if(laser.collider.tag == "Shield" || laser.collider.tag == "Player")
            {
                StartPursuit(laser.collider.gameObject.transform.position);
            }
            else if(pursuit && Vector2.Distance(transform.position , laser.point) < 2f && (laser.collider.tag == "Environment" || laser.collider.tag == "WallJump" || laser.collider.tag == "Climbable"))
            {
                Disengage();
            }
        }
        else
        {
            lineRenderer.SetPosition(1 , (Vector2)transform.position + (aimDirection * 1000f));
        }
    }
}
