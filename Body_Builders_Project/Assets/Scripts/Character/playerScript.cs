/*
Creator: Daniel
Created 04/08/2019
Last Edited by: Daniel
Last Edit 09/08/2019
*/

/*
Still need to fix:

    time slow on groundbreakers
    currently the player can jump vertically up wall jump walls, make sure it is more horizontal
    try to get animations in from > sprites > V's Animations
    currently when w is held and you have the scale augment, you move up walls without rolling, if rotate speed is too low while w is held, lerp it up

Still need to add:

    lifter arms
    shield arms
    hookshot head - open mouth and shoot it out
    expander head

    particle effects on impact
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour
{
// BASIC MOVEMENT

    private float moveInput; // get player Input value
    private bool facingRight = true; // used to flip the character when turning
    private float reverseDirectionTimer = 0f;
    private float startingAngularDrag;

    float speed; // current movement speed
    float movementSpeed = 10f; // the max horizontal movement speed
    float augmentedMovementSpeed = 1f; // scales the movement limit with parts


// JUMPING
    float jumpPower; // the current jump force based on augments
    public float jumpForce = 1f; // modify this to change the jump force between loadouts with equal ratios
    public bool slipCatch; // if the player just fell of a platform, they can still use their first jump in mid air
    public float lastGroundedHeight; // the height you were at when you were last grounded
    float leftGroundTimer; // how long ago were you last grounded

    public bool isGrounded; // is the player on the ground?
    float maxHeight; // the maximum height of the jump
    bool jumpGate; // prevent jumping while this is true
    float jumpGateTimer; // timer for jump gate
    float jumpGateDuration = 0.6f; // the duration of the jumpGate

    public int remainingJumps; // how many jumps does the player have left?
    int maximumJumps = 1; // how many jumps does the player have?

    float fallMultiplier = 2.5f; // increase fall speed on downwards portion of the jump arc
    float unheldJumpReduction = 2f; // reduces jump power if jump button isn't held

    float raycastXOffset; // alters the distance between groundchecker raycasts based on part configuration
    float raycastYOffset; // alters raycast length based on character part configuration, this should be combined with "raycastPos"
    Vector2 raycastPos; // controls where groundcheckers come from
    float augmentedRaycastPosX; // alters the raycast position based on the player part configuration
    float augmentedRaycastPosY; // alters the raycast position based on the player part configuration
    float groundedDistance = 0.15f; // raycast distance;
    public LayerMask jumpLayer; // what layers can the player jump on?
    public LayerMask ladderLayer; // what cn the player climb?
    public LayerMask pickupLayer; // what things can the player pick up?


// AUGMENTS AND PARTS

    public int partConfiguration = 1; // 1 is head, 2 is head and arms, 3 is head and legs, 4 is full body
    public string headString; // this is referenced from the name of the head augment
    bool scaler = false;

    // Arms
    GameObject arms; // the arms component
    public string armString; // this is referenced from the name of the arm prefab
    public bool holding = false; // is the player holding a box?
    bool lifter = false; // do you have the lifter augment?
    bool climbing = false; // are you climbing?
    public bool wallSliding = false; // are you on a wall-jumpable surface
    float wallSlideSpeedMax = 0.1f; // how fast can you slide down walls
    bool wasClimbing = false; // did you just stop climbing?
    float climbingDismountTimer = 0f; // wall jump boost timer
    bool climbingRight = false; // what side are you climbing on?
    float climbSpeed = 10f; // climbing speed

    // Legs
    GameObject legs; // the legs component
    public string legString; // this is referenced from the name of the leg prefab
    public bool groundbreaker = false; // do you have the groundbreaker legs?
    bool afterburner = false; // do you have the afterburner legs equipped?
    GameObject boostSprites; // sprites used for rocket boots
    float groundbreakerDistance = 4f; // have you fallen far enough to break through ground


    float timeSlowdownFactor = 0.01f; // time speed while shapeshifting to use Groundbreakers
    float timeSlowDownLength = 1.5f; // time of the slowdown period
    bool timeSlow; // is the time slow active?
    bool groundBreakerActivate; // are the groundbreakers activated?


// ATTACHABLES AND PARTS
    float boxDetectorOffsetX = 0.68f;
    float boxDetectorOffsetY = 0.05f;
    Vector2 boxDetectorCentre;
    float boxDetectorHeight = 1.96f;
    float boxDetectorWidth = 1.93f;
    Vector2 boxDetectorSize;
    float direction = 1f;
    Rigidbody2D rb; // this object's rigidbody
    BoxCollider2D boxCol; // this object's capsule collider - potentially swap out for box collider if edge slips are undesireable
    GameObject head; // the head component
    CircleCollider2D headCol; // the collider used when the player is just a head
    Camera camera; // the scene's camera
    GameObject scalerStar; // the sprite for the Scaler Augment - starts disabled

    // Arms
    public GameObject pickupBox; // the box that the player is currently picking up
    Transform boxHoldPos; // determine where the held box is positioned
    CircleCollider2D heldBoxCol; // this collider is used for the held box
    ScreenShake screenShake; // screen shake script
    GameObject closestBox; // closest box as determined by the box assigner



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startingAngularDrag = rb.angularDrag;
        boxCol = gameObject.GetComponent<BoxCollider2D>();
        boxCol.enabled = false;
        head = gameObject.transform.Find("Head").gameObject;
        headCol = head.GetComponent<CircleCollider2D>();
        scalerStar = gameObject.transform.Find("Head").gameObject.transform.Find("ScalerStar").gameObject;
        scalerStar.SetActive(false);
        heldBoxCol = gameObject.transform.Find("BoxHoldLocation").gameObject.GetComponent<CircleCollider2D>();
        boxHoldPos = gameObject.transform.Find("BoxHoldLocation").gameObject.transform;
        heldBoxCol.enabled = false;
        camera = Camera.main;
        screenShake = camera.GetComponent<ScreenShake>();
        leftGroundTimer = 0f;
        raycastXOffset = 0.1f;
        reverseDirectionTimer = 0f;
        isGrounded = false;
        raycastPos = transform.position;
        lastGroundedHeight = -1000f;
        climbingDismountTimer = 1f;
        UpdateParts();
    }

    void FixedUpdate()
    {
        moveInput = Input.GetAxis("Horizontal"); // change to GetAxisRaw for sharper movement with less smoothing

        if(reverseDirectionTimer < 1f && partConfiguration == 1 && climbingDismountTimer > 0.1f)
        {
            reverseDirectionTimer += Time.fixedDeltaTime; // try swapping back to deltaTime if this isn't working
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, moveInput * movementSpeed, reverseDirectionTimer/1f), rb.velocity.y);
        }
        else if(climbingDismountTimer <= 0.1f && climbing == false)
        {
            rb.velocity = new Vector2(moveInput * movementSpeed * 1.2f, rb.velocity.y);
            climbingDismountTimer += Time.deltaTime;
        }
        else
        {
            rb.velocity = new Vector2(moveInput * movementSpeed, rb.velocity.y);
        }

        if(GroundCheck() == true)
        {
            isGrounded = true;

            if(maxHeight > (1f + transform.position.y))
            {
                float shakeAmount = maxHeight - transform.position.y;
                screenShake.TriggerShake(shakeAmount);
            }

            maxHeight = transform.position.y;
            lastGroundedHeight = transform.position.y;
            leftGroundTimer = 0f;
            remainingJumps = maximumJumps;
            if(boostSprites != null)
            {
                boostSprites.SetActive(false);
            }
        }
        else
        {
            isGrounded = false;
            leftGroundTimer += Time.fixedDeltaTime; // try swapping back to deltaTime if this isn't working
            speed = Mathf.Clamp(speed , 0f , movementSpeed / 1.1f); // slow player movement in the air
            if(transform.position.y > maxHeight)
            {
                maxHeight = transform.position.y;
            }
            if(remainingJumps == maximumJumps && !slipCatch)
            {
                remainingJumps --;
            }
        }

/*
        if(groundbreaker == true && transform.position.y <= (maxHeight - (groundbreakerDistance - 1f)) && timeSlow == false)
        {
            GroundbreakerTimeShift();
            timeSlow = true;
        }
*/
    }

    void Update()
    {
        boxDetectorCentre = new Vector2(boxDetectorOffsetX * direction + transform.position.x , boxDetectorOffsetY + transform.position.y);
        boxDetectorSize = new Vector2(boxDetectorWidth , boxDetectorHeight);

        raycastPos = transform.position; // this can be altered later if you would like it to change

        if(climbing == true)
        {
            climbingDismountTimer = 0f;
            wasClimbing = true;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX;
            rb.constraints = RigidbodyConstraints2D.FreezePositionY;

            if(Input.GetAxis("Vertical") == 0f && wallSliding == false)
            {
                rb.constraints = RigidbodyConstraints2D.FreezePositionY;
                transform.rotation = Quaternion.identity;
            }
            else
            {
                //rb.constraints = RigidbodyConstraints2D.FreezePositionY;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                rb.velocity = new Vector2(rb.position.x , Input.GetAxis("Vertical") * climbSpeed);
                transform.rotation = Quaternion.identity;
            }

            if(wallSliding == true) //maybe differentiate bools for ground and side collisions
            {
                if(rb.velocity.y < -wallSlideSpeedMax)
                {
                    rb.velocity = new Vector2(rb.velocity.x , -wallSlideSpeedMax);
                }
            }

            if((climbingRight && Input.GetAxis("Horizontal") < 0f) || (!climbingRight && Input.GetAxis("Horizontal") > 0f)) // if player leaves the ladder
            {
                rb.constraints = RigidbodyConstraints2D.None;
                transform.rotation = Quaternion.identity;
                climbing = false;
            }
        }
        if(wasClimbing == true && climbing == false) // turnforcer bool could mean that the player has to press the away button to jump on the wall
        {
            rb.constraints = RigidbodyConstraints2D.None;
            if(partConfiguration > 1)
            {
                transform.rotation = Quaternion.identity;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            wasClimbing = false;
        }

        // turn around
        if((facingRight == false && moveInput > 0) || facingRight == true && moveInput < 0)
        {
            facingRight = !facingRight;
            direction = -direction;
            reverseDirectionTimer = 0f;
            Vector3 Scaler = transform.localScale;
            Scaler.x *= -1;
            transform.localScale = Scaler;
        }


// JUMPING ------------------------------------------------------------------------------------------------------------------

        if(Input.GetButton("Jump") && remainingJumps > 0f && !climbing && (!jumpGate || (scaler && partConfiguration == 1))) // this last bit ensures the player can always jump, which is how the spiderclimb works
        {
            if(isGrounded == true || (leftGroundTimer < 0.3f))
            {
                rb.velocity = Vector2.up * jumpPower;
            }
            else if(afterburner == true && !climbing && remainingJumps == 1f)
            {
                boostSprites.SetActive(true);
                rb.velocity = Vector2.up * jumpPower * 1.1f;
            }
            remainingJumps --;
            jumpGateTimer = 0f;
            jumpGate = true;
        }

        if(jumpGate == true) // stops repeat jumping
        {
            jumpGateTimer += Time.deltaTime;
            if(jumpGateTimer > jumpGateDuration)
            {
                jumpGate = false;
            }
        }

// JUMP TUNING --------------------------------------------------------------------------------------------
        rb.gravityScale = 2f;

        if(rb.velocity.y < 0f && afterburner == true && (Input.GetButton("Jump") || Input.GetKey("space"))) // afterburner glide
        {
            rb.gravityScale = 1f;
            rb.velocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.deltaTime * 0.0005f;

            if(boostSprites != null && climbing == false)
            {
                boostSprites.SetActive(true);
            }
        }
        else if(rb.velocity.y < 0f && !wallSliding) // fast fall for impactful jumping... not great for the knees though (gravity inputs a negative value)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.deltaTime;
            if(boostSprites != null)
            {
                boostSprites.SetActive(false);
            }
        }
        else if (rb.velocity.y > 0f && !Input.GetButton("Jump") && !Input.GetKey("space")) // reduces jump height when button isn't held (gravity inputs a negative value)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (unheldJumpReduction - 1) * Time.deltaTime;
            if(boostSprites != null)
            {
                boostSprites.SetActive(false);
            }       
        }

        BoxInteract(); // check for box pickup or drop prompts

        DetachPart(); // detach part on "space" press
    }

    bool GroundCheck()
    {
        RaycastHit2D hitC = Physics2D.Raycast(new Vector2(raycastPos.x, raycastPos.y + raycastYOffset), Vector2.down, groundedDistance, jumpLayer);
        Debug.DrawRay(new Vector2(raycastPos.x, raycastPos.y + raycastYOffset), Vector2.down * groundedDistance, Color.green);
        wallSliding = false;

        if(partConfiguration == 1)
        {
            climbing = false;
            if(hitC.collider != null && (hitC.collider.gameObject.tag == "Legs" || hitC.collider.gameObject.tag == "Arms") && (transform.position.y > (0.1f + lastGroundedHeight) || (transform.position.y < (lastGroundedHeight - 0.08))))
            {
                return false;
            }
            else if(scaler == true && Physics2D.OverlapCircle(gameObject.transform.position, 0.4f , jumpLayer))
            {
                if(hitC.collider!= null && (hitC.collider.gameObject.tag == "Legs" || hitC.collider.gameObject.tag == "Arms"))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        else if(partConfiguration == 2 || partConfiguration == 4) // for climbing ladders
        {
            if(moveInput > 0 || (facingRight == true && moveInput == 0))
            {
                Vector2 raycastAltPosR = new Vector2(raycastPos.x , raycastPos.y - 0.75f);
                RaycastHit2D sideHitR = Physics2D.Raycast(raycastAltPosR , Vector2.right, 0.6f , ladderLayer);
                Debug.DrawRay(raycastAltPosR, Vector2.right * 0.6f, Color.green);
                if(sideHitR.collider != null && sideHitR.collider.gameObject.tag == "Climbable")
                {
                    climbing = true;
                    climbingRight = true;
                    return true;
                }
                climbing = false;
                if(sideHitR.collider != null && sideHitR.collider.gameObject.tag == "WallJump")
                {
                    wallSliding = true;
                    return true;
                }
            } 
            else if(moveInput < 0 || (facingRight == false && moveInput == 0))
            {
                Vector2 raycastAltPosL = new Vector2(raycastPos.x , raycastPos.y - 0.75f);
                RaycastHit2D sideHitL = Physics2D.Raycast(raycastAltPosL , Vector2.left, 0.6f , ladderLayer);
                Debug.DrawRay(raycastAltPosL, Vector2.left * 0.6f, Color.green);
                if(sideHitL.collider != null && sideHitL.collider.gameObject.tag == "Climbable")
                {
                    climbing = true;
                    climbingRight = false;
                    return true;
                }
                climbing = false;
                if(sideHitL.collider != null && sideHitL.collider.gameObject.tag == "WallJump")
                {
                    wallSliding = true;
                    return true;
                }
            }
        }

        if(!jumpGate) // don't groundcheck if the player just jumped
        {
            RaycastHit2D hitL = Physics2D.Raycast(new Vector2(raycastPos.x - raycastXOffset , raycastPos.y + raycastYOffset), Vector2.down, groundedDistance, jumpLayer);
            RaycastHit2D hitR = Physics2D.Raycast(new Vector2(raycastPos.x + raycastXOffset , raycastPos.y + raycastYOffset), Vector2.down, groundedDistance, jumpLayer);
            Debug.DrawRay(new Vector2(raycastPos.x - raycastXOffset, raycastPos.y + raycastYOffset), Vector2.down * groundedDistance, Color.green);
            Debug.DrawRay(new Vector2(raycastPos.x + raycastXOffset, raycastPos.y + raycastYOffset), Vector2.down * groundedDistance, Color.green);

            if(groundbreaker == true && transform.position.y <= (maxHeight - groundbreakerDistance) && hitC.collider != null) // - groundbreakerDistance))
            {
                if(hitC.collider.gameObject.tag == "Groundbreakable")
                {
                    hitC.collider.gameObject.GetComponent<Groundbreakable_Script>().Groundbreak();
                    return false;
                }
                else
                {
                    return true;
                }
            }
            // the following ensure that the player is not grounded when colliding with attachable parts, necessary for the part attacher script
            else if(hitC.collider != null)
            {
                if(hitC.collider.gameObject.tag == "Legs" && (partConfiguration == 1 || partConfiguration == 2) && (transform.position.y > (0.1f + lastGroundedHeight) || (transform.position.y < (lastGroundedHeight - 0.08f))))
                {
                    return false;
                }
                else if(hitC.collider.gameObject.tag == "Arms" && (partConfiguration == 1 || partConfiguration == 3) && (transform.position.y > (0.1f + lastGroundedHeight) || (transform.position.y < (lastGroundedHeight - 0.08f))))
                {
                    return false;
                }
                return true; // if not a part, or a non-attachable part, then act as though it is normal ground
            }
            else if(hitL.collider != null)
            {
                if(hitL.collider.gameObject.tag == "Legs" && (partConfiguration == 1 || partConfiguration == 2) && (transform.position.y > (0.1f + lastGroundedHeight) || (transform.position.y < (lastGroundedHeight - 0.08f))))
                {
                    return false;
                }
                else if(hitL.collider.gameObject.tag == "Arms" && (partConfiguration == 1 || partConfiguration == 3) && (transform.position.y > (0.1f + lastGroundedHeight) || (transform.position.y < (lastGroundedHeight - 0.08f))))
                {
                    return false;
                }
                return true; // if not a part, or a non-attachable part, then act as though it is normal ground
            }
            else if(hitR.collider != null)
            {
                if(hitR.collider.gameObject.tag == "Legs" && (partConfiguration == 1 || partConfiguration == 2) && (transform.position.y > (0.1f + lastGroundedHeight) || (transform.position.y < (lastGroundedHeight - 0.08f))))
                {
                    return false;
                }
                else if(hitR.collider.gameObject.tag == "Arms" && (partConfiguration == 1 || partConfiguration == 3) && (transform.position.y > (0.1f + lastGroundedHeight) || (transform.position.y < (lastGroundedHeight - 0.08f))))
                {
                    return false;
                }
                return true; // if not a part, or a non-attachable part, then act as though it is normal ground
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    void BoxAssigner()
    {
        float distanceFromPickupPoint;
        float previousDistanceFromPickupPoint = 0f;
        Collider2D[] hitCollider = Physics2D.OverlapBoxAll(boxDetectorCentre , boxDetectorSize, 0f , pickupLayer);
        int i = 0;
        while (i < hitCollider.Length)
        {
            if(hitCollider[i].gameObject.tag == "HeavyLiftable" || hitCollider[i].gameObject.tag == "Box")
            {
                distanceFromPickupPoint = Vector2.Distance(hitCollider[i].gameObject.transform.position , boxHoldPos.position);
                if(distanceFromPickupPoint < previousDistanceFromPickupPoint || previousDistanceFromPickupPoint == 0)
                {
                    previousDistanceFromPickupPoint = distanceFromPickupPoint;
                    closestBox = hitCollider[i].gameObject;
                }
            }
            i++;
        }
    }
void BoxInteract()
    {
        if(Input.GetKeyDown("f") && (partConfiguration == 2 || partConfiguration == 4) && holding == false) // pick up and drop box while you have arms and press "f"
        {
            BoxAssigner();
            {
                if(closestBox != null)
                {
                    closestBox.transform.parent = this.transform;
                    closestBox.transform.position = boxHoldPos.position;
                    closestBox.GetComponent<Rigidbody2D>().isKinematic = true;
                    closestBox.GetComponent<Collider2D>().enabled = false;
                    closestBox.transform.rotation = Quaternion.identity;
                    heldBoxCol.enabled = true;
                    holding = true;
                }
            }
        } 
        else if(Input.GetKeyDown("f") && holding == true)
        {
            BoxDrop();
            // if we eventually do want the box to be thrown, add some alternative code here
        }
    }
    
    void BoxDrop() // when the character drps the box;
    {
        if(closestBox != null)
        {
            closestBox.transform.parent = null;
            heldBoxCol.enabled = false;
            closestBox.GetComponent<Rigidbody2D>().isKinematic = false;
            closestBox.GetComponent<Collider2D>().enabled = true;
            holding = false;
            closestBox = null;
        } 
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxDetectorCentre , boxDetectorSize);
    }


    void DetachPart()
    {
        if (Input.GetKeyDown("space") && partConfiguration != 1) // eventually set this to create prefabs of the part, rather than a detached piece
        {
            if(partConfiguration > 2)
            {
                legs.GetComponent<Legs>().Detached();
                legs.transform.parent = null;
                remainingJumps --; // consumes jumps but doesn't require them to be used
                UpdateParts();
                rb.velocity = Vector2.up * jumpForce * 8f;
            }
            else if(partConfiguration == 2)
            {
                arms.GetComponent<Arms>().Detached();
                arms.transform.parent = null;
                remainingJumps --; // consumes jumps but doesn't require them to be used
                UpdateParts();
                rb.velocity = Vector2.up * jumpForce * 9f;
            }
        }
    }


    public void UpdateParts() // increase raycastYOffset, decrease groundcheckerDistance
    // call when acquiring or detaching part - reconfigures scaling, controls and colliders - 1 is head, 2 adds torso, 3 adds legs, 4 adds torso and legs
    {
        // assume that the robot has neither arms or legs, then check for them
        bool hasArms = false;
        bool hasLegs = false;
        for(int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).tag == "Arms")
            {
                hasArms = true;
            }
            else if (transform.GetChild(i).tag == "Legs")
            {
                hasLegs = true;
            }
        }

        Vector2 snapOffsetPos = gameObject.transform.position; // changing offset to cater for original sprites provided - may need to be re-scaled later

        if(!hasArms && !hasLegs)
        {
            partConfiguration = 1; // just a head
            movementSpeed = augmentedMovementSpeed * 5f;
            jumpPower = jumpForce * 6f;
            jumpGateDuration = 0.2f;

            // upgrades
            maximumJumps = 1;
            groundbreaker = false;
            afterburner = false;
            lifter = false;
            if(boostSprites != null)
            {
                boostSprites.SetActive(false);
                boostSprites = null;
            }

            if(headString == "Scaler") // reduces jump power if you have the Scaler Augment as a trade-off
            {
                scaler = true;
                jumpPower = jumpForce * 6f;
                fallMultiplier = 1f;
            }
            else
            {
                scaler = false;
                jumpPower = jumpForce * 7f;
                fallMultiplier = 2.5f;
            }

            boxCol.enabled = false; // don't use the typical vertical standing collider
            headCol.enabled = true; // use the circle collider instead
            rb.constraints = RigidbodyConstraints2D.None; // can roll
            head.transform.position = gameObject.transform.position; // no need for snapOffsetPos here as it is perfectly centred
            legString = "None"; // no legs
            armString = "None"; // no arms
            raycastXOffset = 0.1f;
            raycastYOffset = 0f;
            groundedDistance = 0.33f;

            BoxDrop(); // drops any box immediately
            scalerStar.transform.localScale = new Vector3(0.35f, 0.35f, 1f); // set the Scaler star/spikes to maximum size
        }


        else if (hasArms && !hasLegs)
        {
            partConfiguration = 2; // just a head and arms
            movementSpeed = augmentedMovementSpeed * 7.5f;
            jumpPower = jumpForce * 8.5f;

            NonHeadConfig();
            fallMultiplier = 3f;
            arms = gameObject.transform.Find(armString).gameObject;
            if(armString == "LifterArms")
            {
                lifter = true;
            }
            else
            {
                lifter = false;
            }

            legString = "None"; // no legs
                maximumJumps = 1;
                groundbreaker = false;
                afterburner = false;
                if(boostSprites != null)
                {
                    boostSprites.SetActive(false);
                    boostSprites = null;
                }
           
            // adjust height of other parts
            head.transform.position = new Vector2 (snapOffsetPos.x , snapOffsetPos.y + 0.55f); // head snaps up
            arms.transform.position = new Vector2 (snapOffsetPos.x , snapOffsetPos.y - 0.255f); // arms snap down relative to the head, maintaining their original height
            raycastYOffset = -0.7f;

            boxCol.size = new Vector2(0.6f , 1.6f);
            boxCol.offset = new Vector2(0f , 0.03f);
            
            if(holding == true) // keep holding the box if you were
            {
                heldBoxCol.enabled = true;
            }
            else
            {
                heldBoxCol.enabled = false;
            }
        }


        else if (!hasArms && hasLegs) // need to change collider
        {
            partConfiguration = 3;
            movementSpeed = augmentedMovementSpeed * 8.5f;
            jumpPower = jumpForce * 11f;
            
            NonHeadConfig();
            armString = "None"; // no arms
            lifter = false;
            legs = gameObject.transform.Find(legString).gameObject;
                if(legString == "AfterburnerLegs")
                {
                    maximumJumps = 2;
                    afterburner = true;
                    boostSprites = legs.transform.Find("BoostSprites").gameObject;
                }
                else
                {
                    maximumJumps = 1;
                    afterburner = false;
                    if(boostSprites != null)
                    {
                        boostSprites.SetActive(false);
                        boostSprites = null;
                    }
                }

                if(legString == "GroundbreakerLegs")
                {
                    groundbreaker = true;
                }
                else
                {
                    groundbreaker = false;
                }

            head.transform.position = new Vector2 (snapOffsetPos.x , snapOffsetPos.y + 0.155f); // head snaps up... legs stay where they are
            raycastYOffset = -0.95f;

            boxCol.size = new Vector2(0.6f , 1.45f);
            boxCol.offset = new Vector2(0f , -0.27f);
            BoxDrop(); // no arms, so drop the box
        }


        else if(hasArms && hasLegs) // need to change collider
        {
            partConfiguration = 4; // has all parts
            movementSpeed = augmentedMovementSpeed * 8.5f;
            jumpPower = jumpForce * 11f;

            NonHeadConfig();
            arms = gameObject.transform.Find(armString).gameObject;
            if(armString == "LifterArms")
            {
                lifter = true;
            }
            else
            {
                lifter = false;
            }

            legs = gameObject.transform.Find(legString).gameObject;
                if(legString == "AfterburnerLegs")
                {
                    maximumJumps = 2;
                    afterburner = true;
                    boostSprites = legs.transform.Find("BoostSprites").gameObject;
                }
                else
                {
                    maximumJumps = 1;
                    afterburner = false;
                    if(boostSprites != null)
                    {
                        boostSprites.SetActive(false);
                        boostSprites = null;
                    }
                }
                if(legString == "GroundbreakerLegs")
                {
                    groundbreaker = true;
                }
                else
                {
                    groundbreaker = false;
                }

            head.transform.position = new Vector2(snapOffsetPos.x , snapOffsetPos.y + 0.755f); // head snaps up
            arms.transform.position = new Vector2(snapOffsetPos.x , snapOffsetPos.y); // arms share the complete character's origin
            raycastYOffset = -0.95f;

            boxCol.size = new Vector2(0.6f , 2.08f);
            boxCol.offset = new Vector2(0f , 0.03f);

            if(holding == true) // keep holding the box if you already were
            {
                heldBoxCol.enabled = true;
            }
            else
            {
                heldBoxCol.enabled = false;
            }
        }

// Manage Head Augments Here

        if(headString == "Scaler") // changes whether the Scaler Augment is visible or not - no mechanical difference
        {
            scalerStar.SetActive(true);
        }
        else
        {
            headString = "BasicHead";
            scalerStar.SetActive(false);
        }

        camera.GetComponent<Camera2DFollow>().Resize(partConfiguration);
    }

    void NonHeadConfig()
    {
        headCol.enabled = false; // disable the rolling head collider
        boxCol.enabled = true; // use the capsule collider instead
        rb.constraints = RigidbodyConstraints2D.FreezePositionY; // freeze movement temporarily
        rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        jumpGate = true;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // can no longer roll
        scalerStar.transform.localScale = new Vector3(0.25f, 0.25f, 1f); // shrink the scaler star to signify it is no longer usable
        transform.rotation = Quaternion.identity; // lock rotation to 0;
        isGrounded = false;
        fallMultiplier = 4f;
        rb.velocity = Vector2.zero;
        moveInput = 0f;
        jumpGate = true;
        jumpGateDuration = 0.6f;
        jumpGateTimer = jumpGateDuration - 0.12f;
        raycastXOffset = 0.27f;
        groundedDistance = 0.15f;
    }
/*
    void GroundbreakerTimeShift()
    {
        Time.timeScale = timeSlowdownFactor;
        //Time.fixedDeltaTime = Time.timeScale * 0.001f;
    }
*/
}
