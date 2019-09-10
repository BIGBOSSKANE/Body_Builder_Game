using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hookShot : MonoBehaviour
{
    // Mouse Move Tracker
    bool mouseMoved;
    float mouseMovedTime = 2f;
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

    void Awake()
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
    }

    void Update()
    {
        worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        if(previousMousePos != worldMousePos)
        {
            mouseMoved = true;
            mouseMovedTimer = mouseMovedTime;
        }

        previousMousePos = worldMousePos;

        if(mouseMoved == true)
        {
            mouseMovedTimer -= Time.deltaTime;
            if(mouseMovedTimer <= 0f)
            {
                mouseMovedTimer = 0f;
                mouseMoved = false;
            }
        }

        Vector2 facingDirection = new Vector2(worldMousePos.x - transform.position.x , worldMousePos.y - transform.position.y);
        float aimAngle = Mathf.Atan2(facingDirection.y , facingDirection.x);

        if(aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }

        aimDirection = Quaternion.Euler(0 , 0 , aimAngle * Mathf.Rad2Deg) * Vector2.right;
        augmentAimDirection = Quaternion.Euler(0 , 0 , (aimAngle * Mathf.Rad2Deg) + transform.localScale.x * 45f) * Vector2.right;

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
            Vector3 augmentFiredDirection = Quaternion.Euler(0 , 0 , (firedAimAngle * Mathf.Rad2Deg) + transform.localScale.x * 45f) * Vector2.right;
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
            if(Input.GetAxisRaw("Vertical") < 0.5f && Input.GetAxisRaw("Vertical") > -0.5f)
            {
                rappelDirection = 0;
                rappelTime = 0f;
            }
            else if(Input.GetAxisRaw("Vertical") > 0f)
            {
                if(rappelDirection == -1 || rappelDirection == 0)
                {
                    rappelTime = 0f;
                }
                rappelDirection = 1;
                distanceJoint.distance -= ropeClimbSpeed * Time.deltaTime * rappelTime;
            }
            else if(Input.GetAxisRaw("Vertical") < 0f)
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
        if(Input.GetMouseButtonDown(1))
        {
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
                playerScript.forceSlaved = false;
                hookShotAnchorPoint.SetActive(true);
                ropeAttached = true;
                ropeMiss = false;
                ropeAttachedTime = 0f;
                distanceJoint.enabled = true;
                distanceJoint.distance = Vector2.Distance(ropeAnchorPoint , transform.position);
                lineRenderer.enabled = true;
                playerScript.isSwinging = true;
                gameObject.GetComponent<Rigidbody2D>().AddForce(aimDirection * 5f, ForceMode2D.Impulse);
            }
            else
            {
                ropeMiss = true;
                DetachRope();
                ropeAnchorPoint = (Vector2)transform.position + (aimDirection * hookShotDistance);
                if(hit.collider.gameObject.layer == 11)
                {
                    ropeAnchorPoint = hit.point;
                }
                ropeAttached = false;
                lineRenderer.enabled = true;
                hookShotAnchorPoint.SetActive(true);
                ropeAttachedTime = 0f;
            }
        }
        else if((Input.GetKey("space") || Input.GetMouseButtonDown(0)) && ropeAttached)
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
        playerScript.forceSlaved = true;
        hookShotAnchorPoint.SetActive(false);
        ropeAttached = false;
        distanceJoint.enabled = false;
        lineRenderer.enabled = false;
        playerScript.isSwinging = false;
        hookShotAnchorPoint.transform.position = transform.position;
        if(!ropeMiss) gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 5f), ForceMode2D.Impulse);
    }
}