using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class edgelord : MonoBehaviour
{
    public float outerBladeRadius = 3f;
    float outerBladeRadiusIndicatorOffset = 2f;
    public float spinSpeed = 3f;
    public float innerBladeSpinSpeed = 6f;
    public float outerBladeSpinSpeed = 3f;
    public List<Transform> outerBlades;
    [HideInInspector] public List<Transform> blades;
    [HideInInspector] public List<Vector2> bladePositions;
    [HideInInspector] public List<Vector2> outerbladeStartPositions;
    [HideInInspector] public List<Transform> arms;
    GameObject core;
    Transform coreBlade;
    float velocity;
    float rotationalVelocity;
    int currentWaypoint = 0;
    bool moving = true;
    float moveTime;
    float moveTimer;
    float bounceBackTime = 2.5f;
    float bounceBackTimer;
    bool bounceBack;
    Vector2 bounceBackVector;
    float pauseTime;
    float pauseTimer;
    float acceleration;
    bool accelerate = true;
    bool accelerateRotation = true;
    bool losePart = false;
    bool horizontal;
    float distanceOffset;
    float radialDivisions;
    int negative;
    int outerBladeToLose;
    int currentRotations;
    Vector2 targetPosition;
    Vector2 startPosition;
    Vector3 moveDirection;
    Vector2 destroyArmPos;
    Quaternion endRotation;
    float previousdistanceFromCollisionPoint;
    float rotationAmount;
    bool startUp = true;
    [Tooltip("To create a new waypoint, increase the size of this array, then adjust the parameters of each waypoint.")] public SubClass[] waypointCycle;


    void Start()
    {
        outerBladeRadius *= transform.localScale.x;

        for(int i = 0 ; i < waypointCycle.Length ; i++)
        {
            Vector2 armStopPoint = Quaternion.AngleAxis(waypointCycle[i].endZRotation, Vector3.forward) * Vector2.right * outerBladeRadius;
            waypointCycle[i].stopPoint = waypointCycle[i].waypointPos + armStopPoint;
        }
        
        coreBlade = gameObject.transform.Find("Core").gameObject.transform.Find("blade");

        for (int i = 0; i < outerBlades.Count; i++)
        {
            blades.Add(outerBlades[i].transform.Find("blade"));
        }

        UpdateOuterBlades();

        for(int i = 0; i < outerBlades.Count; i++)
        {
            outerBlades[i].localPosition = bladePositions[i];
            outerBlades[i].transform.up = (outerBlades[i].transform.position - transform.position).normalized;
        }

        core = gameObject.transform.Find("Core").gameObject;
        UpdateWaypoint(currentWaypoint);

        startUp = false;
    }

    void UpdateWaypoint(int newWaypoint)
    {
        startPosition = transform.position;
        moveTime = waypointCycle[newWaypoint].waypointMoveTime;
        pauseTime = waypointCycle[newWaypoint].waypointPauseTime;
        targetPosition = waypointCycle[newWaypoint].waypointPos;
        accelerate = waypointCycle[newWaypoint].accelerate;
        accelerateRotation = waypointCycle[newWaypoint].accelerateRotation;
        losePart = waypointCycle[newWaypoint].losePart;
        destroyArmPos = waypointCycle[newWaypoint].stopPoint;

        velocity = 2 * (Vector2.Distance(targetPosition , startPosition) / moveTime);
        moveDirection = (targetPosition - startPosition).normalized;
        horizontal = (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y))? true : false;
        distanceOffset = (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y))? moveDirection.x : moveDirection.y;
        distanceOffset += -Mathf.Sign(distanceOffset) * 0.05f;
        if(Mathf.Sign(distanceOffset) > 0) negative = 0;
        else negative = 1;

        rotationAmount = (outerBlades.Count > 0)? waypointCycle[newWaypoint].endZRotation + transform.rotation.z + (waypointCycle[newWaypoint].numberOfRotations * radialDivisions) : 0;
        endRotation = Quaternion.Euler(0 , 0 , rotationAmount);
        rotationalVelocity = 2 * rotationAmount / moveTime;

        currentRotations += outerBladeToLose;
    }

    void UpdateOuterBlades()
    {
        int oddCount = (outerBlades.Count % 2 == 0)? 0 : 1;

        radialDivisions = (outerBlades.Count > 0)? (360 / outerBlades.Count) : 0;

        transform.rotation = Quaternion.identity;
        
        if(!startUp)
        {
            outerbladeStartPositions.Clear();
            for (int i = 0; i < outerBlades.Count; i++) // track all previous positions for smooth retraction
            {
                outerbladeStartPositions.Add(bladePositions[i]);
            }
        }

        arms.Clear();
        bladePositions.Clear();
        for (int i = 0; i < outerBlades.Count; i++)
        {
            Vector2 direction = ((Vector2)(Quaternion.Euler(0 , 0 , i * radialDivisions) * transform.right)).normalized;
            arms.Add(outerBlades[i].Find("arm").transform);
            bladePositions.Add(direction * outerBladeRadius);
        }
    }

    void Update()
    {
        Spin();
        Move();
    }

    void DetachOuterBlade(int indexToRemove)
    {
        outerBlades[indexToRemove].parent = null;
        arms.RemoveAt(indexToRemove);
        outerBlades.RemoveAt(indexToRemove);
        bladePositions.RemoveAt(indexToRemove);
        blades.RemoveAt(indexToRemove);
        UpdateOuterBlades();
    }

    void Spin()
    {
        if(moving)
        {
            if(accelerateRotation)
            {
                float currentRotationalVelocity = rotationalVelocity * moveTimer;
                transform.Rotate(0f , 0f, currentRotationalVelocity * Time.deltaTime);
            }
            else
            {
                transform.Rotate(0f , 0f , rotationalVelocity * Time.deltaTime / 2f);
            }
        }
        else
        {
            transform.rotation = endRotation;
        }

        core.transform.up = Vector2.up; // make sure the centre doesn't rotate
        coreBlade.Rotate(0f , 0f , -innerBladeSpinSpeed * Time.deltaTime); // spin central blade

        foreach (Transform blade in blades)
        {
            blade.Rotate(0f , 0f , -outerBladeSpinSpeed * Time.deltaTime); // spin outer blades
        }        
    }

    void Move()
    {
        if(moving)
        {
            if(moveTimer < 1)
            {
                moveTimer += Time.deltaTime / moveTime;
            }
            
            if(accelerate)
            {
                float currentVelocity = velocity * moveTimer;
                transform.position += moveDirection * currentVelocity * Time.deltaTime;
            }
            else
            {
                transform.position = Vector2.Lerp(startPosition , targetPosition , moveTimer);
            }

            if(moveTimer > 1)
            {
                moveTimer = 1;
                bounceBackTimer = 0;
                pauseTimer = 0;
                moving = false;
                transform.position = targetPosition;
                transform.rotation = endRotation;
                core.transform.up = Vector2.up; // make sure the centre doesn't rotate
                bounceBack = false;

                if(losePart)
                {
                    bounceBackVector = ((Vector2)transform.position - destroyArmPos).normalized;
                    int indexToDetach = 0;

                    previousdistanceFromCollisionPoint = 10000;
                    for(int i = 0 ; i < outerBlades.Count ; i++) // cycle through to check which outer blade is closest to the point of collision
                    {
                        float distanceFromCollisionPoint = Vector2.Distance(destroyArmPos , outerBlades[i].position);
                        if(distanceFromCollisionPoint < previousdistanceFromCollisionPoint)
                        {
                            previousdistanceFromCollisionPoint = distanceFromCollisionPoint;
                            indexToDetach = i;
                        }
                    }
                    DetachOuterBlade(indexToDetach); // this needs to go during bounceback
                }
            }
        }
        else
        {
            if(!bounceBack) // wait before windup or don't lose a part
            {
                if(pauseTimer < 1) pauseTimer += Time.deltaTime / pauseTime;

                if(pauseTimer > 1)
                {
                    pauseTimer = 1;
                    moveTimer = 0;
                    if(!losePart)
                    {
                        if(currentWaypoint < waypointCycle.Length -1)
                        {
                            moving = true;
                            currentWaypoint ++;
                            UpdateWaypoint(currentWaypoint);
                            for(int i = 0; i < outerBlades.Count ; i ++)
                            {
                                outerBlades[i].transform.localPosition = bladePositions[i];
                                outerBlades[i].transform.up = (outerBlades[i].transform.position - transform.position).normalized;
                            }
                        }
                    }
                    else
                    {
                        bounceBackTimer = 0f;
                        bounceBack = true;
                    }
                }  
            }
            else // wrench itself free from the wall, and then spin, rearranging all outer blades to their next position
            {
                if(bounceBackTimer < 1) bounceBackTimer += Time.deltaTime / bounceBackTime;

                float withdrawDuration = 0.1f;
                float centreDuration = 0.45f;
                float expandDuration = 0.45f;
                
                if(bounceBackTimer > 1)
                {
                    bounceBackTimer = 1;
                    pauseTimer = 0f;
                    moveTimer = 0f;
                    for(int i = 0; i < outerBlades.Count ; i ++)
                    {
                        outerBlades[i].transform.localPosition = bladePositions[i];
                        outerBlades[i].transform.up = (outerBlades[i].transform.position - transform.position).normalized;
                    }
                    
                    if(currentWaypoint < waypointCycle.Length -1)
                    {
                        moving = true;
                        currentWaypoint ++;
                        bounceBack = false;
                        bounceBackTimer = 0f;
                        UpdateWaypoint(currentWaypoint);
                    }
                }
                else if(bounceBackTimer >= withdrawDuration)
                {
                    float rotationVelocity = 720 * 2 / (1 - withdrawDuration);
                    float currentRotationVelocity = rotationVelocity * (bounceBackTimer - withdrawDuration);

                    transform.rotation = Quaternion.Euler(0f , 0f, rotationAmount + currentRotationVelocity);

                    core.transform.up = Vector2.up;

                    if(bounceBackTimer > centreDuration + withdrawDuration) // expand outer blades to their new positions
                    {
                        float expandTimer = (bounceBackTimer - (withdrawDuration + centreDuration)) / expandDuration;
                        for(int i = 0 ; i < outerBlades.Count ; i++)
                        {
                            outerBlades[i].transform.localPosition = Vector2.Lerp(Vector2.zero , bladePositions[i] , expandTimer);
                            outerBlades[i].transform.up = (outerBlades[i].transform.position - transform.position).normalized;
                            arms[i].localScale = Vector2.Lerp(Vector2.zero , Vector2.one, expandTimer);
                        }         
                    }
                    else // centre outer blades second
                    {
                        float withdrawTimer = (bounceBackTimer - withdrawDuration) / centreDuration;
                        for(int i = 0 ; i < outerBlades.Count ; i++) 
                        {
                            outerBlades[i].transform.localPosition = Vector2.Lerp(outerbladeStartPositions[i] , Vector2.zero , withdrawTimer);
                            outerBlades[i].transform.up = (outerBlades[i].transform.position - transform.position).normalized;
                            arms[i].localScale = Vector2.Lerp(Vector2.one , Vector2.zero, withdrawTimer);
                        }
                    }
                }
                else // retreat from wall first
                {     
                    transform.position = Vector2.Lerp(targetPosition , targetPosition + 3f * bounceBackVector , bounceBackTimer / withdrawDuration);
                }
            }
        }
    }


    // Whenever a part is lost, reduce the size of the outer blades array, and then find the new radial positions, the closest outerblade moves towards that location while it does its angry spin


    // To lose a part, find which one is closest to the stop point in waypointCycle


    void OnDrawGizmosSelected() // show all of the camera waypoints
    {
        float locationIdentifier = 0.3f;

        Gizmos.color = Color.green;

        if(!Application.isPlaying)
        {
            for(int i = 0 ; i < waypointCycle.Length ; i++)
            {
                Gizmos.DrawLine(waypointCycle[i].waypointPos - Vector2.up * locationIdentifier , waypointCycle[i].waypointPos + Vector2.up * locationIdentifier);
                Gizmos.DrawLine(waypointCycle[i].waypointPos - Vector2.left * locationIdentifier , waypointCycle[i].waypointPos + Vector2.left * locationIdentifier);

                Gizmos.color = Color.red;

                Vector2 armStopPoint = Quaternion.AngleAxis(waypointCycle[i].endZRotation, Vector3.forward) * Vector2.right * (outerBladeRadius * transform.localScale.x + outerBladeRadiusIndicatorOffset);

                Gizmos.DrawLine(waypointCycle[i].waypointPos + armStopPoint - Vector2.up * locationIdentifier , waypointCycle[i].waypointPos + armStopPoint + Vector2.up * locationIdentifier);
                Gizmos.DrawLine(waypointCycle[i].waypointPos + armStopPoint - Vector2.left * locationIdentifier , waypointCycle[i].waypointPos + armStopPoint + Vector2.left * locationIdentifier);

                Gizmos.color = Color.blue;
                if(i != 0) Gizmos.DrawLine(waypointCycle[i].waypointPos , waypointCycle[i-1].waypointPos);
                else  Gizmos.DrawLine(waypointCycle[i].waypointPos , transform.position);
            }
        }
        else
        {
            for(int i = 0 ; i < waypointCycle.Length ; i++)
            {
                Gizmos.DrawLine(waypointCycle[i].waypointPos - Vector2.up * locationIdentifier , waypointCycle[i].waypointPos + Vector2.up * locationIdentifier);
                Gizmos.DrawLine(waypointCycle[i].waypointPos - Vector2.left * locationIdentifier , waypointCycle[i].waypointPos + Vector2.left * locationIdentifier);

                Gizmos.color = Color.red;
                
                Gizmos.DrawLine(waypointCycle[i].stopPoint - Vector2.up * locationIdentifier , waypointCycle[i].stopPoint + Vector2.up * locationIdentifier);
                Gizmos.DrawLine(waypointCycle[i].stopPoint - Vector2.left * locationIdentifier , waypointCycle[i].stopPoint + Vector2.left * locationIdentifier);

                Gizmos.color = Color.blue;
                if(i != 0) Gizmos.DrawLine(waypointCycle[i].waypointPos , waypointCycle[i-1].waypointPos);
                else Gizmos.DrawLine(waypointCycle[i].waypointPos , transform.position);
            }
        }
    }

    public SubClass GetValue (int index)
    {
        return waypointCycle[index];
    }

    [System.Serializable]
    public class SubClass
    {
        public Vector2 waypointPos;
        public float waypointPauseTime;
        public float waypointMoveTime = 10f; // time for camera movement
        public bool accelerate = true;
        public bool accelerateRotation = true;
        [Tooltip("This is divided by the number of blades")] public int numberOfRotations;
        [Range(0 , 360)] public float endZRotation;
        public bool losePart = false;
        [HideInInspector] public Vector2 stopPoint;
    }
}



/*

Still need to jump back out of the wall, and do a super fast spin in which the blades are set to their new values (rather than immediately)

*/