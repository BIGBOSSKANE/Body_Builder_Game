using System;
using UnityEngine;
using Random=UnityEngine.Random;

    public class Camera2DFollow : MonoBehaviour
    {
        public Transform target;
        playerScript playerScript;
        bool speedUp;
        bool slowDown;
        public float damping = 1;
        float initialDamping;
        float currentDamping;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;
        float initialLookAheadMoveThreshold;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;

        int sizeConfiguration;
        bool resize;
        float resizeTimer;
        public float resizeDuration = 0.8f;
        [HideInInspector] public bool playerLocked = false;
        [HideInInspector] public float standardResizeDuration;

        new Camera camera;
        float initialCameraSize;
        float targetCameraSize;
        public float headSize = 5f;
        public float torsoSize = 5.5f;
        public float legSize = 6f;
        public float completeSize = 6.5f;
        public float scoutSize = 10f;
        float t; // lerp time

        // Shake
        public float shakeDurationLimit = 3f;
        public float shakeMagnitudeLimit = 5f;
        public float shakeDivider = 25f;
        float shakePartModifier = 0f; // the player part configuration effect on the screenshake
        Transform transformPos;
        private float shakeDuration = 0f;
        private float shakeMagnitude = 0.7f;
        float groundBreakShakeAmplifier;
        Vector3 initialPosition;
        Vector2 rawInput;
        public float scoutSpeed = 5f;
        public float scoutSpeedTime = 1f;
        bool shiftHeld = false;
        Vector3 wayPointPos;
        float wayPointDistance;
        [HideInInspector] public bool waypointCycling = false;
        [HideInInspector] public bool unlockOnInput = false;
        [HideInInspector] public float waypointMoveTime = 1f;
        float waypointMoveTimer = 0f;
        [HideInInspector] public float wayPointResizeDuration;
        [HideInInspector] public Vector3 previousWaypointPos;
        float waypointPauseTimer = 0f;
        float waypointPauseTime = 1f;
        cameraWaypoint camPoint;
        int waypointCount;
        int waypointCounter = 0;


        private void Awake()
        {
            camera = gameObject.GetComponent<Camera>(); // locates the camera immediately before player calls for a resize
            if (transformPos == null)
            {
                transformPos = gameObject.transform;
            }
            standardResizeDuration = resizeDuration;
        }

        public void TriggerShake(float fallDistance , bool groundBreakDistance , int partConfig)
        {
            if(partConfig == 1)
            {
                shakePartModifier = 8f;
            }
            else if (partConfig == 2 && partConfig == 3)
            {
                shakePartModifier = 6f;
            }
            else
            {
                shakePartModifier = 4f;
            }

            if(groundBreakDistance)
            {
                groundBreakShakeAmplifier = 1.5f;
            }
            else
            {
                groundBreakShakeAmplifier = 1f;
            }
            
            shakeDuration = Mathf.Clamp(fallDistance * groundBreakShakeAmplifier /(shakeDivider * shakePartModifier) , 0f , shakeDurationLimit);
            shakeMagnitude = Mathf.Clamp(fallDistance / shakeDivider, 0f , shakeMagnitudeLimit);
        }

        private void Start()
        {
            target = GameObject.Find("Player").GetComponent<Transform>();
            playerScript = target.gameObject.GetComponent<playerScript>();
            transform.position = new Vector3(target.position.x , target.position.y , transform.position.z);
            m_LastTargetPosition = target.position;
            m_OffsetZ = (transform.position - target.position).z;
            transform.parent = null;
            initialDamping = damping;
            waypointCounter = 0;
            t = 0f;
        }

        public void SpeedUp()
        {
            speedUp = true;
            slowDown = false;
            currentDamping = damping;
            t = 0f;
        }

        public void RestoreSpeed()
        {
            slowDown = true;
            speedUp = false;
            currentDamping = damping;
            t = 0f;
        }

        // Update is called once per frame
        private void Update()
        {
            if(!waypointCycling) // when not cycling through waypoints
            {
                if(Input.GetKeyDown(KeyCode.LeftShift))
                {
                    shiftHeld = true;
                    AkSoundEngine.PostEvent("EnterFan" , gameObject);
                }

                if(Input.GetKeyUp(KeyCode.LeftShift))
                {
                    shiftHeld = false;
                    AkSoundEngine.PostEvent("ExitFan" , gameObject);
                }

                if(shiftHeld)
                {
                    Vector2 previousRawInput = rawInput;
                    rawInput = new Vector2(Input.GetAxisRaw("Horizontal") , Input.GetAxisRaw("Vertical"));
                    scoutSpeedTime += Time.deltaTime;
                    if(rawInput != previousRawInput)
                    {
                        scoutSpeedTime = 0f;
                    }
                    scoutSpeedTime = Mathf.Clamp(scoutSpeedTime , 0f , 1f);
                    transform.position = new Vector3(Mathf.Clamp(transform.position.x + (scoutSpeed * rawInput.x * scoutSpeedTime) , -(Mathf.Abs(target.position.x) + 20f) , Mathf.Abs(target.position.x) + 20f),
                                                     Mathf.Clamp(transform.position.y + (scoutSpeed * rawInput.y * scoutSpeedTime) , -(Mathf.Abs(target.position.y) + 20f) , Mathf.Abs(target.position.y) + 20f),
                                                     transform.position.z);
                }
                else
                {
                    if(damping <= 0f)
                    {
                        speedUp = false;
                        damping = 0f;
                    }
                    else if(speedUp == true)
                    {
                        t += Time.unscaledDeltaTime / 1f;
                        damping = Mathf.SmoothStep(currentDamping , 0f , t);
                    }
                    
                    if(slowDown == true && damping >= initialDamping)
                    {
                        damping = initialDamping;
                        slowDown = false;
                    }
                    else if (slowDown == true && damping < initialDamping)
                    {
                        t += Time.unscaledDeltaTime / 1f;
                        damping = Mathf.SmoothStep(currentDamping , initialDamping , t);
                    }

                    // only update lookahead pos if accelerating or changed direction
                    float xMoveDelta = (target.position - m_LastTargetPosition).x;

                    bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

                    if (updateLookAheadTarget)
                    {
                        m_LookAheadPos = lookAheadFactor*Vector3.right*Mathf.Sign(xMoveDelta);
                    }
                    else
                    {
                        m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime*lookAheadReturnSpeed);
                    }

                    Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward*m_OffsetZ;
                    //Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

                    Vector3 newPos = new Vector3(Mathf.SmoothDamp(transform.position.x , aheadTargetPos.x , ref m_CurrentVelocity.x , initialDamping),
                                                 Mathf.SmoothDamp(transform.position.y , aheadTargetPos.y , ref m_CurrentVelocity.y , damping),
                                                 Mathf.SmoothDamp(transform.position.z , aheadTargetPos.z , ref m_CurrentVelocity.z , damping));

                    transform.position = newPos;

                    m_LastTargetPosition = target.position;

                    if(shakeDuration > 0)
                    {
                        transformPos.localPosition = transformPos.position + Random.insideUnitSphere * shakeMagnitude;
                        shakeDuration -= Time.unscaledDeltaTime;
                        shakeMagnitude -= Time.unscaledDeltaTime;
                    }
                    else
                    {
                        shakeDuration = 0f;
                        transformPos.localPosition = transformPos.position;
                    }
                }
            }
            else // waypoint cycling
            {
                waypointMoveTimer += Time.deltaTime;
                if(waypointMoveTimer >= waypointMoveTime) // move to position
                {
                    waypointMoveTimer = waypointMoveTime;

                    waypointPauseTimer += Time.deltaTime;
                    if(waypointPauseTimer > waypointMoveTime)
                    {
                        waypointPauseTimer = waypointPauseTime;
                        waypointCounter++;
                        if(waypointCounter == waypointCount)
                        {
                            Debug.Log("end" + waypointCounter);
                            //camPoint.NextCoordinate();
                            EndCycle();
                            Debug.Log("next");
                        }
                        else
                        {
                            Debug.Log("next");
                            camPoint.NextCoordinate();
                        }
                    }
                }
                gameObject.transform.position = Vector3.Lerp(previousWaypointPos , wayPointPos , waypointMoveTimer/waypointMoveTime);
            }
            
            // resize is always on

            if(resize == true && resizeTimer < resizeDuration)
            {
                camera.orthographicSize = Mathf.Lerp(initialCameraSize , targetCameraSize , resizeTimer);
                resizeTimer += Time.deltaTime;
            }
            else
            {
                resize = false;
                resizeTimer = 0f;
            }
        }

        public void WayPointCycle(Vector2 waypoint , Vector2 prevPos , float pauseTime , float size , bool locked , float moveTime , cameraWaypoint camWay , int waypointNumber) // lock and unlock the screen
        {
            camPoint = camWay;
            waypointNumber = waypointCount;
            previousWaypointPos = new Vector3(prevPos.x , prevPos.y , transform.position.z); // the previous waypoint position
            waypointMoveTimer = 0f;
            waypointMoveTime = moveTime;
            resizeDuration = moveTime;
            waypointPauseTimer = 0f;
            wayPointPos = new Vector3(waypoint.x , waypoint.y , transform.position.z); // location of the next waypoint
            wayPointDistance = Vector2.Distance(previousWaypointPos , wayPointPos); // distance between waypoints
            playerLocked = locked; // if the player is locked, they can't move. Otherwise the screen is locked an they can move.

            if(playerLocked)
            {
                playerScript.shiftHeld = true; // disable the player controller input
            }

            if(waypointCycling)
            {
                Resize(6 , resizeDuration , size);
            }
        }

        public void EndCycle()
        {
            waypointCycling = false;
            waypointCounter = 0;
            if(camPoint.onlyOnce)
            {
                Destroy(camPoint.gameObject);
            }
            shiftHeld = false;
            resizeDuration = standardResizeDuration;
            playerScript.shiftHeld = false;
            playerScript.UpdateParts();
        }

        public void Resize(int configuration , float resizeTime , float size) // resize the camera for scout mode, and different part configurations
        {
            initialCameraSize = camera.orthographicSize;
            sizeConfiguration = configuration;

            if(sizeConfiguration == 1) // just a head
            {
                targetCameraSize = headSize;
            }
            else if(sizeConfiguration == 2) // head and arms
            {
                targetCameraSize = torsoSize;
            }
            else if (sizeConfiguration == 3) // head and legs
            {
                targetCameraSize = legSize;
            }
            else if(sizeConfiguration == 4) // full body
            {
                targetCameraSize = completeSize;
            }
            else if(sizeConfiguration == 5) // this one is just parsed from the shift controls
            {
                targetCameraSize = scoutSize;
            }
            else if(sizeConfiguration == 6) // this one allows for waypoint control
            {
                targetCameraSize = size;
            }

            resize = true;
            resizeTimer = 0f;
            if(resizeTime > 0.1f)
            {
                resizeDuration = resizeTime;
            }
        }
    }