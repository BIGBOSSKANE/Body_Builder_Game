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

        int playerPartConfiguration;
        bool resize;
        float resizeTimer;
        public float resizeDuration = 0.8f;
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
            if(Input.GetKeyDown(KeyCode.LeftShift))
            {
                shiftHeld = true;
            }

            if(Input.GetKeyUp(KeyCode.LeftShift))
            {
                shiftHeld = false;
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

                if (shakeDuration > 0)
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

        public void Resize(int updatedPlayerParts , float resizeTime)
        {
            initialCameraSize = camera.orthographicSize;
            playerPartConfiguration = updatedPlayerParts;
            if(playerPartConfiguration == 1)
            {
                targetCameraSize = headSize;
            }
            else if(playerPartConfiguration == 2)
            {
                targetCameraSize = torsoSize;
            }
            else if (playerPartConfiguration == 3)
            {
                targetCameraSize = legSize;
            }
            else if(playerPartConfiguration == 4)
            {
                targetCameraSize = completeSize;
            }
            else if(playerPartConfiguration == 5) // this one is just parsed from the shift controls
            {
                targetCameraSize = scoutSize;
            }
            resize = true;
            resizeTimer = 0f;
            if(resizeTime > 0.1f)
            {
                resizeDuration = resizeTime;
            }
        }
    }