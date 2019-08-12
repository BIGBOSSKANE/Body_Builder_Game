﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hookshotScript : MonoBehaviour
{
    // Mouse Move Tracker
    bool mouseMoved;
    public float mouseMovedTime = 2f;
    float mouseMovedTimer;
    float hookShotDistance = 6f;
    Vector2 worldMousePos;
    Vector2 aimDirection;
    Vector2 previousMousePos;

    // Hookshot
    bool ropeAttached = false;
    public float ropeClimbSpeed = 2f;
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
    public LayerMask tetherLayer;
    private Vector2 playerPosition;
    public float ropeMaxCastDistance = 20f;

    void Start()
    {
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
        if(ropeAttached == true)
        {
            DrawRope();
        }

        if(ropeAttached && Input.GetAxis("Vertical") != 0f)
        {
            if(Input.GetAxis("Vertical") > 0f)
            {
                distanceJoint.distance -= ropeClimbSpeed * Time.deltaTime;
            }
            else if(Input.GetAxis("Vertical") < 0f)
            {
                distanceJoint.distance += 5f * Time.deltaTime;
            }
        }
    }

    private void HookShotFire()
    {
        if(Input.GetMouseButtonDown(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, aimDirection, hookShotDistance, tetherLayer);
            if(hit.collider != null)
            {
                if(hit.collider.gameObject.tag == "TetherPoint")
                {
                    ropeAnchorPoint = hit.collider.gameObject.transform.position;
                }
                else
                {
                    ropeAnchorPoint = hit.point;
                }
                hookShotAnchorPoint.SetActive(true);
                ropeAttached = true;
                distanceJoint.enabled = true;
                lineRenderer.enabled = true;
                playerScript.isSwinging = true;
                if(!playerScript.isGrounded)
                {
                    gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 5f), ForceMode2D.Impulse);
                }
            }
        }
        else if(Input.GetKey("space") && ropeAttached)
        {
            hookShotAnchorPoint.SetActive(false);
            ropeAttached = false;
            distanceJoint.enabled = false;
            lineRenderer.enabled = false;
            playerScript.isSwinging = false;
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 5f), ForceMode2D.Impulse);
        }
    }

    private void DrawRope()
    {
        hookShotAnchorPoint.transform.position = ropeAnchorPoint;
        distanceJoint.distance = Vector2.Distance(ropeAnchorPoint , transform.position);
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0 , transform.position);
        lineRenderer.SetPosition(1 , ropeAnchorPoint);
    }
}
