using System;
using UnityEngine;

    public class Camera2DFollow : MonoBehaviour
    {
        public Transform target;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;

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

        // Use this for initialization

        private void Awake()
        {
            camera = gameObject.GetComponent<Camera>(); // locates the camera immediately before player calls for a resize
        }

        private void Start()
        {
            target = GameObject.Find("Player").GetComponent<Transform>();
            transform.position = new Vector3(target.position.x , target.position.y , transform.position.z);
            m_LastTargetPosition = target.position;
            m_OffsetZ = (transform.position - target.position).z;
            transform.parent = null;
        }


        // Update is called once per frame
        private void Update()
        {
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
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

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