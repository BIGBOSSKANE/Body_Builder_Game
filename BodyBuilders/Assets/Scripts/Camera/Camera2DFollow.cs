using System;
using UnityEngine;
using Random=UnityEngine.Random;

    public class Camera2DFollow : MonoBehaviour
    {
        // Lock Axis
        bool lockX = false, lockY = false, lockAxis = false;
        float yLockPos , xLockPos;

        // Normal Movement
        Transform target;
        playerScript playerScript;
        Rigidbody2D rb;
        bool speedUp;
        bool slowDown;
        [Tooltip("Damping of camera movement")] public float damping = 1;
        float initialDamping;
        float currentDamping;
        [Tooltip("Distance camera looks ahead")] public float lookAheadFactor = 3;
        [Tooltip("Time taken for camera to move between look ahead locations")] public float lookAheadReturnSpeed = 0.5f;
        [Tooltip("Movement speed required to initiate camera movement")] public float lookAheadMoveThreshold = 0.1f;
        float initialLookAheadMoveThreshold;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;

        int sizeConfiguration;
        bool resize;
        float resizeTimer;
        [Tooltip("Time taken for the camera to resize when scouting, or the player changes their part configuration")] public float resizeDuration = 0.8f;
        [HideInInspector] public bool playerLocked = false;
        [HideInInspector] public float standardResizeDuration;

        new Camera camera;
        float initialCameraSize;
        float targetCameraSize;
        [Tooltip("Camera size for player as just a head")] public float headSize = 5f;
        [Tooltip("Camera size for player as just a head and arms")] public float torsoSize = 5.5f;
        [Tooltip("Camera size for player as just a head with legs")] public float legSize = 6f;
        [Tooltip("Camera size for player with full body")] public float completeSize = 6.5f;
        [Tooltip("Size when scouting")] public float scoutSize = 10f;
        float t; // lerp time

        // Shake
        [Tooltip("Shake time limit")] public float shakeDurationLimit = 3f;
        [Tooltip("Shake amount limit")] public float shakeMagnitudeLimit = 5f;
        [Tooltip("Increase to reduce screen shake")] public float shakeDivider = 25f;
        float shakePartModifier = 0f; // the player part configuration effect on the screenshake
        Transform transformPos;
        private float shakeDuration = 0f;
        private float shakeMagnitude = 0.7f;
        float groundBreakShakeAmplifier;
        Vector3 initialPosition;
        [Tooltip("Acceleration factor of scout mode camera movement")] public float scoutAcceleration = 5f;
        [Tooltip("The maximum camera scout speed")] public float scoutSpeedMax = 2f;
        [Tooltip("Reduce camera movement to zero after this many seconds of no scout input")] public float scoutInputTime = 2f;
        float scoutInputTimer;

        bool scouting = false;
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
        int waypointCount = 0;
        int waypointCounter = 0;
        float waypointCyclingTimer = 0f;
        bool lockView = false;


        private void Awake() // OnEnable is called after awake if that needs to be used instead
        {
            camera = gameObject.GetComponent<Camera>(); // locates the camera immediately before player calls for a resize
            rb = gameObject.GetComponent<Rigidbody2D>();
            rb.isKinematic = true;
            if (transformPos == null)
            {
                transformPos = gameObject.transform;
            }
            standardResizeDuration = resizeDuration;
        }

        private void Start()
        {
            SeekTarget();
            m_LastTargetPosition = target.position;
            m_OffsetZ = (transform.position - target.position).z;
            scoutInputTimer = 1f;
            transform.parent = null;
            initialDamping = damping;
            waypointCounter = 0;
            t = 0f;
        }

        public void SeekTarget() // call this when instantiating a new player, so that the camera correctly locks to them
        {
            target = GameObject.Find("Player").GetComponent<Transform>();
            playerScript = target.gameObject.GetComponent<playerScript>();
            transform.position = new Vector3(target.position.x , target.position.y , transform.position.z);
        }

        public void LockAxis(bool locked , bool x , bool y , Vector2 lockPoint , float size , float resizeDuration) // Lock or unlock a specific axis
        // need to prevent normal resizing to occur while in lock axis
        {
            if(locked)
            {
                lockAxis = true;
                if(x)
                {
                    lockX = true;
                    xLockPos = lockPoint.x;
                    rb.constraints = RigidbodyConstraints2D.FreezePositionX;
                }
                else
                {
                    rb.constraints = RigidbodyConstraints2D.None;
                    lockX = false;
                }

                if(y)
                {
                    lockY = true;
                    yLockPos = lockPoint.y;
                    rb.constraints = RigidbodyConstraints2D.FreezePositionY;
                    
                    if(x)
                    {
                        xLockPos = lockPoint.x;
                        rb.constraints = RigidbodyConstraints2D.FreezePositionX;
                    }
                    else
                    {
                        lockX = false;
                    }
                }
                else
                {
                    lockY = false;
                    rb.constraints = RigidbodyConstraints2D.None;

                    if(x)
                    {
                        xLockPos = lockPoint.x;
                        rb.constraints = RigidbodyConstraints2D.FreezePositionX;
                    }
                    else
                    {
                        lockX = false;
                    }
                }
            }
            else
            {
                lockAxis = false;
                rb.constraints = RigidbodyConstraints2D.None;
            }

            Resize(6 , resizeDuration , size);
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
            
            shakeDuration = Mathf.Clamp(fallDistance * groundBreakShakeAmplifier / (shakeDivider * shakePartModifier) , 0f , shakeDurationLimit);
            shakeMagnitude = Mathf.Clamp(fallDistance / shakeDivider, 0f , shakeMagnitudeLimit);
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

        public void ToggleScoutMode(bool toggle)
        {
            if(toggle)
            {
                scouting = true;
                AkSoundEngine.PostEvent("EnterFan" , gameObject);
                rb.isKinematic = false;
                rb.simulated = true;
            }
            else
            {
                scouting = false;
                AkSoundEngine.PostEvent("ExitFan" , gameObject);
                rb.isKinematic = true;
                rb.simulated = false;
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if(unlockOnInput && waypointCycling && waypointCyclingTimer > 1f && (Input.GetAxisRaw("Vertical") != 0|| Input.GetAxisRaw("Horizontal") != 0))
            {
                EndCycle();
                playerScript.UpdateParts();
            }

            if(!waypointCycling) // when not cycling through waypoints
            {
                if(!lockView)
                {
                    if(scouting)
                    {
                        if(Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
                        {
                            if(scoutInputTimer < 0f)
                            {
                                scoutInputTimer = 0f;
                            }
                            rb.velocity = Vector2.ClampMagnitude(rb.velocity , scoutInputTimer);
                            scoutInputTimer -= Time.deltaTime / scoutInputTime;
                        }
                        else
                        {
                            scoutInputTimer = 1f;
                            Vector2 input = new Vector2(Input.GetAxis("Horizontal") , Input.GetAxis("Vertical"));
                            rb.AddForce(scoutAcceleration * input , ForceMode2D.Force);
                            rb.velocity = Vector2.ClampMagnitude(rb.velocity , scoutSpeedMax);
                            float maxScoutDistance = 30f;
                            transform.position = new Vector3(Mathf.Clamp(transform.position.x , target.position.x - maxScoutDistance , target.position.x + maxScoutDistance) ,
                                                             Mathf.Clamp(transform.position.y , target.position.y - maxScoutDistance , target.position.y + maxScoutDistance) ,
                                                             transform.position.z);
                        }
                    }
                }


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

                if(lockAxis)
                {
                    if(lockY) aheadTargetPos.y = yLockPos;
                    if(lockX) aheadTargetPos.x = xLockPos;
                }

                Vector3 newPos = new Vector3(Mathf.SmoothDamp(transform.position.x , aheadTargetPos.x , ref m_CurrentVelocity.x , initialDamping),
                                             Mathf.SmoothDamp(transform.position.y , aheadTargetPos.y , ref m_CurrentVelocity.y , damping),
                                             Mathf.SmoothDamp(transform.position.z , aheadTargetPos.z , ref m_CurrentVelocity.z , damping));

                if(!scouting) transform.position = newPos;

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
            else if(waypointCycling)// waypoint cycling
            {
                waypointCyclingTimer += Time.deltaTime; // track overall time

                waypointMoveTimer += Time.deltaTime; // track time of each position
                if(waypointMoveTimer >= waypointMoveTime) // move to position
                {
                    waypointMoveTimer = waypointMoveTime;

                    if(lockView)
                    {
                        Vector2 screenPoint = camera.WorldToViewportPoint(target.position);
                        bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
                        if(!onScreen)
                        {
                            EndCycle();
                            playerScript.UpdateParts();
                        }
                    }
                    else
                    {
                        waypointPauseTimer += Time.deltaTime; // track stay time at each position - if lockView is on, it will stay here, so the timer stops
                    }

                    if(waypointPauseTimer > waypointMoveTime)
                    {
                        waypointPauseTimer = waypointPauseTime;
                        waypointCounter++;
                        if(waypointCounter == waypointCount)
                        {
                            EndCycle();
                        }
                        else
                        {
                            camPoint.NextCoordinate();
                        }
                    }
                }
                gameObject.transform.position = Vector3.Lerp(previousWaypointPos , wayPointPos , waypointMoveTimer/waypointMoveTime);
            }
            
            // resize is always on

            if(resize == true)
            {
                if(resizeTimer < 1f)
                {
                    resizeTimer += Time.deltaTime / resizeDuration;
                }
                else
                {
                    resizeTimer = 1f;
                    resize = false;
                }
                camera.orthographicSize = Mathf.Lerp(initialCameraSize , targetCameraSize , resizeTimer);
            }
        }

        public void WayPointCycle(Vector2 waypoint , Vector2 prevPos , float pauseTime , float size , bool locked , bool viewLock , bool unlockOnInput , float moveTime , cameraWaypoint camWay , int waypointNumber) // lock and unlock the screen
        {
            camPoint = camWay;
            waypointMoveTimer = 0f;
            waypointMoveTime = moveTime;
            resizeDuration = moveTime;
            waypointPauseTimer = 0f;

            if(viewLock)
            {
                lockView = viewLock;
                waypointNumber = 1;
                previousWaypointPos = transform.position;
                waypointMoveTimer = 0f;
                waypointMoveTime = moveTime;
            }
            else
            {
                waypointNumber = waypointCount;
                previousWaypointPos = new Vector3(prevPos.x , prevPos.y , transform.position.z); // the previous waypoint position
            }
            waypointPauseTimer = 0f;
            wayPointPos = new Vector3(waypoint.x , waypoint.y , transform.position.z); // location of the next waypoint
            wayPointDistance = Vector2.Distance(previousWaypointPos , wayPointPos); // distance between waypoints
            playerLocked = locked; // if the player is locked, they can't move. Otherwise the screen is locked an they can move.

            if(playerLocked)
            {
                playerScript.lockController = true; // disable the player controller input
                Rigidbody2D playerRB = playerScript.gameObject.GetComponent<Rigidbody2D>();
                playerRB.velocity = Vector2.zero;
                playerRB.constraints = RigidbodyConstraints2D.FreezePositionX;
            }

            if(waypointCycling)
            {
                Resize(6 , resizeDuration , size);
            }
        }

        public void EndCycle()
        {
            lockView = false;
            waypointCycling = false;
            waypointCyclingTimer = 0f;
            scouting = false;
            resizeDuration = standardResizeDuration;
            playerScript.lockController = false;
            playerScript.UpdateParts();
            if(camPoint != null && camPoint.onlyOnce && camPoint.gameObject != null)
            {
                Destroy(camPoint.gameObject);
            }
        }

        public void Resize(int configuration , float resizeTime , float size) // resize the camera for scout mode, and different part configurations
        {
            if(camera == null)
            {
                camera = gameObject.GetComponent<Camera>();
            }
            initialCameraSize = camera.orthographicSize;
            sizeConfiguration = configuration;

            if(sizeConfiguration == 1 && !lockView && !lockAxis) // just a head
            {
                targetCameraSize = headSize;
            }
            else if(sizeConfiguration == 2 && !lockView && !lockAxis) // head and arms
            {
                targetCameraSize = torsoSize;
            }
            else if (sizeConfiguration == 3 && !lockView && !lockAxis) // head and legs
            {
                targetCameraSize = legSize;
            }
            else if(sizeConfiguration == 4 && !lockView && !lockAxis) // full body
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