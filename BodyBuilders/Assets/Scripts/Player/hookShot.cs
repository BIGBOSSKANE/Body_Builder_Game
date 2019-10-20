using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hookShot : MonoBehaviour
{
    // Mouse Move Tracker
    bool mouseMoved;
    float mouseMovedTime = 0.1f;
    float mouseMovedTimer;
    Vector2 worldMousePos;
    Vector2 aimDirection;
    Vector2 previousMousePos;

    // Hookshot
    bool ropeMiss;
    float hookShotDistance = 6f;
    float ropeAttachedTime = 0f;
    float rappelTime = 0f;
    int rappelDirection = 0;
    [HideInInspector] public bool ropeAttached = false;
    [Tooltip("How long before you can fire the hookshot again")] public float cooldown = 0.3f;
    float cooldownTimer;
    [Tooltip("Rappel descent and climb speed of the rope")] public float ropeClimbSpeed = 2f;
    [Tooltip("Maximum distance of the rope")] public float maxropeLength = 8f;
    Vector3 augmentAimDirection;
    playerScript playerScript;
    GameObject hookShotAnchorPoint;
    Vector3 ropeAnchorPoint;
    GameObject hookShotAugment;
    DistanceJoint2D distanceJoint;
    Transform crosshair;
    SpriteRenderer crosshairSprite;
    private Rigidbody2D hookShotAnchorPointRb;
    private SpriteRenderer hookshotAnchorSprite;
    LineRenderer lineRenderer;
    Rigidbody2D rb;
    [Tooltip("What layers can the player tether to?")] public LayerMask tetherLayer;
    private Vector2 playerPosition;
    playerSound playerSound;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        playerPosition = gameObject.transform.position;
        hookShotAnchorPoint = gameObject.transform.Find("HookshotAnchor").gameObject;
        hookShotAnchorPointRb = hookShotAnchorPoint.GetComponent<Rigidbody2D>();
        hookshotAnchorSprite = hookShotAnchorPoint.GetComponent<SpriteRenderer>();
        distanceJoint = gameObject.GetComponent<DistanceJoint2D>();
        distanceJoint.enabled = false;
        lineRenderer = gameObject.transform.Find("HookshotAnchor").gameObject.GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        playerScript = GameObject.Find("Player").gameObject.GetComponent<playerScript>();        
        hookShotAugment = gameObject.transform.Find("Head").gameObject.transform.Find("HookshotHead").gameObject;
        playerSound = GetComponent<playerSound>();
    }

    void Update()
    {
        cooldownTimer += Time.deltaTime;

        Vector2 mousePos = (Vector2)Input.mousePosition;
        
        if(previousMousePos != mousePos)
        {
            mouseMoved = true;
            mouseMovedTimer = mouseMovedTime;
        }
        else mouseMoved = false;

        previousMousePos = mousePos;

        if(mouseMoved == true)
        {
            mouseMovedTimer -= Time.deltaTime;
            if(mouseMovedTimer <= 0f)
            {
                mouseMovedTimer = 0f;
                mouseMoved = false;
            }
        }


        worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 facingDirection = new Vector2(worldMousePos.x - transform.position.x , worldMousePos.y - transform.position.y);
        if(mouseMoved != true)
        {
            if(InputManager.JoystickRight() != Vector2.zero) facingDirection = InputManager.JoystickRight();
        }
        else
        {
            InputManager.previousJoystickRightHorizontal = 0f;
            InputManager.previousJoystickRightVertical = 0f;
        }
        
        float aimAngle = Mathf.Atan2(facingDirection.y , facingDirection.x);

        if(aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }

        aimDirection = Quaternion.Euler(0 , 0 , aimAngle * Mathf.Rad2Deg) * Vector2.right;
        float scaleX = transform.localScale.x;
        augmentAimDirection = Quaternion.Euler(0 , 0 , (aimAngle * Mathf.Rad2Deg) + scaleX * 45f) * Vector2.right;

        playerPosition = transform.position;

        if(!ropeAttached && playerScript.partConfiguration == 1) // aim at cursor
        {
            hookShotAugment.transform.up = augmentAimDirection;
        }
        else if(playerScript.partConfiguration == 1) // aim at anchorpoint
        {
            Vector2 firedDirection = new Vector2(ropeAnchorPoint.x , ropeAnchorPoint.y) - playerPosition;
            float firedAimAngle = Mathf.Atan2(firedDirection.y , firedDirection.x);
            if(firedAimAngle < 0f)
            {
                firedAimAngle = Mathf.PI * 2 + aimAngle;
            }
            Vector3 augmentFiredDirection = Quaternion.Euler(0 , 0 , (firedAimAngle * Mathf.Rad2Deg) + scaleX * 45f) * Vector2.right;
            hookShotAugment.transform.up = augmentFiredDirection;
            playerScript.ropeDirection = augmentFiredDirection;
        }

        HookShotFire();
        if(ropeAttached)
        {
            DrawRope();
        }

        if(ropeMiss)
        {
            DrawRope();
            if(ropeAttachedTime >= 1f)
            {
                lineRenderer.enabled = false;
                hookShotAnchorPoint.SetActive(true);
                ropeMiss = false;
                hookShotAnchorPoint.transform.position = transform.position;
            }
        }

        if(ropeAttached)
        {
            if((InputManager.JoystickLeftVertical() < 0.4f && InputManager.JoystickLeftVertical() > -0.4f) || playerScript.lockController)
            {
                rappelDirection = 0;
                rappelTime = 0f;
            }
            else if(InputManager.JoystickLeftVertical() > 0.1f)
            {
                if(rappelDirection == -1 || rappelDirection == 0)
                {
                    rappelTime = 0f;
                }
                rappelDirection = 1;
                distanceJoint.distance -= ropeClimbSpeed * Time.deltaTime * rappelTime;
            }
            else if(InputManager.JoystickLeftVertical() < -0.1f)
            {
                if(rappelDirection == 1)
                {
                    rappelTime = 0f;
                }
                rappelDirection = -1;
                distanceJoint.distance += 2.5f * ropeClimbSpeed * Time.deltaTime * rappelTime;
            }
            rappelTime += Time.deltaTime;
            if(rappelTime >= 1f)
            {
                rappelTime = 1f;
            }
        }  

        if(distanceJoint.distance >= maxropeLength)
        {
            distanceJoint.distance = maxropeLength;
        }
    }

    private void HookShotFire()
    {
        if(InputManager.Cast() && !playerScript.lockController && cooldownTimer > cooldown)
        {
            playerSound.ShootPlay();
            cooldownTimer = 0f;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, aimDirection, hookShotDistance, tetherLayer);
            if(hit.collider != null && !(playerScript.isGrounded && hit.collider.gameObject.transform.position.y < transform.position.y) && hit.collider.gameObject.layer != 11)
            {
                if(hit.collider.gameObject.tag == "TetherPoint")
                {
                    ropeAnchorPoint = hit.collider.gameObject.transform.position;
                }
                else
                {
                    ropeAnchorPoint = hit.point;
                }

                lineRenderer.enabled = true;
                ropeAttachedTime = 0f;
                hookShotAnchorPoint.SetActive(true);

                if(aimDirection.y < 0.8f && playerScript.scalerTrueGrounded)
                {
                    if(hit.collider != null) ropeAnchorPoint = hit.point;
                    ropeAttached = false;
                    ropeMiss = true;
                    Debug.Log("Heyo");
                    return;
                }

                playerScript.forceSlaved = false;
                ropeAttached = true;
                ropeMiss = false;
                distanceJoint.enabled = true;
                distanceJoint.distance = Vector2.Distance(ropeAnchorPoint , transform.position);
                playerScript.isSwinging = true;
                gameObject.GetComponent<Rigidbody2D>().AddForce(aimDirection * 5f, ForceMode2D.Impulse);
            }
            else
            {
                Debug.Log("Did it");
                ropeMiss = true;
                DetachRope();
                ropeAnchorPoint = (Vector2)transform.position + (aimDirection * hookShotDistance);
                if(hit.collider != null && hit.collider.gameObject.layer == 11) // if hitting an enemy, draw the rope towards them
                {
                    ropeAnchorPoint = hit.point;
                }
                ropeAttached = false;
                lineRenderer.enabled = true;
                hookShotAnchorPoint.SetActive(true);
                ropeAttachedTime = 0f;
            }
        }
        else if((InputManager.Detach() || InputManager.ButtonADown() || Input.GetMouseButtonDown(0)) && ropeAttached)
        {
            DetachRope();
        }
    }

    private void DrawRope()
    {
        hookShotAnchorPoint.transform.position = ropeAnchorPoint;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0 , transform.position);
        if(ropeAttachedTime < 1f)
        {
            Vector2 castPos = (Vector2)transform.position + (((Vector2)ropeAnchorPoint - (Vector2)transform.position) * ropeAttachedTime);
            lineRenderer.SetPosition(1 , castPos);
            ropeAttachedTime += Time.deltaTime * 6f;
        }
        else
        {
            lineRenderer.SetPosition(1 , ropeAnchorPoint);
        }

        if(ropeAttached)
        {
            RaycastHit2D ropeRay = Physics2D.Raycast(transform.position , ropeAnchorPoint - transform.position , distanceJoint.distance - 0.5f, tetherLayer);
            if(ropeRay.collider != null)
            {
                DetachRope();
            }
        }
    }

    public void DetachRope()
    {
        if(playerScript != null) playerScript.forceSlaved = true;
        hookShotAnchorPoint.SetActive(false);
        ropeAttached = false;
        distanceJoint.enabled = false;
        lineRenderer.enabled = false;
        if(playerScript != null) playerScript.isSwinging = false;
        hookShotAnchorPoint.transform.position = transform.position;
        if(!ropeMiss && !playerScript.scalerTrueGrounded && distanceJoint.distance > 0.3f) rb.AddForce(new Vector2(0f, 5f), ForceMode2D.Impulse);
    }
}