using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hookShot : MonoBehaviour
{
    bool mouseMoved;
    public float mouseMovedTime = 2f;
    float mouseMovedTimer;
    Vector2 aimDirection;
    Vector3 previousMousePos;

    GameObject hookShotAnchorPoint;
    DistanceJoint2D ropeJoint;
    Transform crosshair;
    SpriteRenderer crosshairSprite;
    //public PlayerMovement playerMovement;
    private bool ropeAttached;
    private Vector2 playerPosition;
    private Rigidbody2D hookShotAnchorPointRb;
    private SpriteRenderer hookshotAnchorSprite;

    LineRenderer ropeRenderer;
    public LayerMask ropeLayerMask;
    public float ropeMaxCastDistance = 20f;
    private List<Vector2> ropePositions = new List<Vector2>();

    void Start()
    {
        playerPosition = gameObject.transform.position;
        hookShotAnchorPoint = gameObject.transform.Find("HookshotAnchor").gameObject;
        hookShotAnchorPointRb = hookShotAnchorPoint.GetComponent<Rigidbody2D>();
        hookshotAnchorSprite = hookShotAnchorPoint.GetComponent<SpriteRenderer>();
        crosshair = gameObject.transform.Find("Crosshair").gameObject.transform;
        crosshairSprite = crosshair.GetComponent<SpriteRenderer>();
        ropeJoint = gameObject.GetComponent<DistanceJoint2D>();
        ropeJoint.enabled = false;
        ropeRenderer = gameObject.transform.Find("HookshotAnchor").gameObject.GetComponent<LineRenderer>();
    }

    void Update()
    {
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x , Input.mousePosition.y , 0f));
        
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

        var facingDirection = worldMousePos - transform.position;
        var aimAngle = Mathf.Atan2(facingDirection.y , facingDirection.x);

        if(aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }

        var aimDirection = Quaternion.Euler(0 , 0 , aimAngle * Mathf.Rad2Deg) * Vector2.right;

        playerPosition = transform.position;

        if(!ropeAttached && mouseMoved)
        {
            SetCrossHairPosition(aimAngle);
        }
        else
        {
            crosshairSprite.enabled = false;
        }

        HandleInput(aimDirection);
    }

    private void SetCrossHairPosition(float aimAngle)
    {
        if(!crosshairSprite.enabled)
        {
            crosshairSprite.enabled = true;
        }

        float crossHairPosX = transform.position.x + 1f * Mathf.Cos(aimAngle);
        var crossHairPosY = transform.position.y + 1f * Mathf.Sin(aimAngle);

        var crossHairPosition = new Vector3(crossHairPosX , crossHairPosY , 0);
        crosshair.transform.position = crossHairPosition;
    }

    private void HandleInput(Vector2 aimDirection) // call from update
    {
        if(Input.GetMouseButton(1)) // right click
        {
            if(ropeAttached) return;
            ropeRenderer.enabled = true;

            RaycastHit2D hit = Physics2D.Raycast(playerPosition, aimDirection, ropeMaxCastDistance, ropeLayerMask);

            if(hit.collider != null)
            {
                ropeAttached = true;
                if(!ropePositions.Contains(hit.point)) // check that the hitpoint isn't already part of the rope
                {
                    Debug.Log("Hit" + hit.point);
                    // the issue is that the point is spawning in the player now, that will change
                    // apply a small impulse force upwards on the player to get them off the ground
                    transform.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 5f , ForceMode2D.Impulse);
                    ropePositions.Add(hit.point);
                    ropeJoint.distance = Vector2.Distance(playerPosition, hit.point);
                    ropeJoint.enabled = true;
                    hookshotAnchorSprite.enabled = true;
                }
            }
            else // the rope misses
            {
                ropeRenderer.enabled = false;
                ropeAttached = false;
                ropeJoint.enabled = false;
            }
        }

        if(Input.GetMouseButtonUp(1))
        {
            ResetRope();
        }
    }

    private void ResetRope()
    {
        ropeJoint.enabled = false;
        ropeAttached = false;
        //playerMovement.isSwinging = false;
        ropeRenderer.positionCount = 2;
        ropeRenderer.SetPosition(0 , transform.position);
        ropeRenderer.SetPosition(1 , transform.position);
        ropePositions.Clear();
        hookshotAnchorSprite.enabled = false;
    }
}