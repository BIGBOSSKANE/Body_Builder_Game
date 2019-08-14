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

        new Camera camera;
        float initialCameraSize;
        float targetCameraSize;
        public float headSize = 5f;
        public float torsoSize = 5.5f;
        public float legSize = 6f;
        public float completeSize = 6.5f;
        float t; // lerp time

        // Shake
        public float shakeLimit = 3f;
        public float shakeDivider = 25f;
        Transform transformPos;
        private float shakeDuration = 0f;
        private float shakeMagnitude = 0.7f;
        Vector3 initialPosition;


        private void Awake()
        {
            camera = gameObject.GetComponent<Camera>(); // locates the camera immediately before player calls for a resize
            if (transformPos == null)
            {
                transformPos = gameObject.transform;
            }
        }

        public void TriggerShake(float shakeAmount , float dampenEnhancer)
        {
            shakeDuration = Mathf.Clamp(shakeAmount/4f , 0f , 0.3f);
            shakeAmount = Mathf.Clamp(shakeAmount/(shakeDivider * dampenEnhancer) , 0f , shakeLimit);
            shakeMagnitude = shakeAmount;
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

        public void Resize(int updatedPlayerParts)
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
            resize = true;
            resizeTimer = 0f;
        }
    }