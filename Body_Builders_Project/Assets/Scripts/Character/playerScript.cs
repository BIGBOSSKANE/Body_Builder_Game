/*
Creator: Daniel
Created 04/08/2019
Last Edited by: Daniel
Last Edit 04/08/2019
*/

/*
Still need to fix:


    fix ladder dynamics
    time slow on groundbreakers
    try to get animations in from > sprites > V's Animations
    box pickup proximity prioritisation to boxHoldPoscol - use a foreach loop

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

    int remainingJumps; // how many jumps does the player have left?
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
    public LayerMask ladderLayer; // other layers the player can jump on


// AUGMENTS AND PARTS

    public int partConfiguration = 1; // 1 is head, 2 is head and arms, 3 is head and legs, 4 is full body
    public string headString; // this is referenced from the name of the head augment
    bool scaler = false;

    // Arms
    GameObject arms; // the arms component
    public string armString; // this is referenced from the name of the arm prefab
    bool boxInRange = false; // is a box in the pickup range?
    bool holding = false; // is the player holding a box?
    bool lifter = false; // do you have the lifter augment?
    public bool climbing = false; // are you climbing?
    bool climbingRight = false; // what side are you climbing on?
    float climbSpeed = 10f; // climbing speed

    // Legs
    GameObject legs; // the legs component
    public string legString; // this is referenced from the name of the leg prefab
    public bool groundbreaker = false; // do you have the groundbreaker legs?
    bool afterburner = false; // do you have the afterburner legs equipped?
    GameObject boostSprites; // sprites used for rocket boots
    float groundbreakerDistance = 4f; // have you fallen far enough to break through ground


    float timeSlowdownFactor = 0.001f; // time speed while shapeshifting to use Groundbreakers
    float timeSlowDownLength = 1.5f; // time of the slowdown period
    bool timeSlow; // is the time slow active?
    bool groundBreakerActivate; // are the groundbreakers activated?


// ATTACHABLES AND PARTS

    Rigidbody2D rb; // this object's rigidbody
    BoxCollider2D boxCol; // this object's capsule collider - potentially swap out for box collider if edge slips are undesireable
    GameObject head; // the head component
    CircleCollider2D headCol; // the collider used when the player is just a head
    public new GameObject camera; // the scene's camera
    GameObject scalerStar; // the sprite for the Scaler Augment - starts disabled

    // Arms
    GameObject pickupBox; // the box that the player is currently picking up
    BoxCollider2D pickupBoxCol; // the area in which a player can pick up boxes (and later climb)
    Transform boxHoldPos; // determine where the held box is positioned
    CircleCollider2D heldBoxCol; // this collider is used for the held box
    ScreenShake screenShake; // screen shake script



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
        pickupBoxCol = gameObject.transform.Find("BoxHoldLocation").gameObject.GetComponent<BoxCollider2D>();
        pickupBoxCol.enabled = true;
        heldBoxCol = gameObject.transform.Find("BoxHoldLocation").gameObject.GetComponent<CircleCollider2D>();
        boxHoldPos = gameObject.transform.Find("BoxHoldLocation").gameObject.transform;
        heldBoxCol.enabled = false;
        screenShake = camera.GetComponent<ScreenShake>();
        leftGroundTimer = 0f;
        raycastXOffset = 0.1f;
        reverseDirectionTimer = 0f;
        isGrounded = false;
        raycastPos = transform.position;
        lastGroundedHeight = -1000f;
        UpdateParts();
    }

    void FixedUpdate()
    {
        moveInput = Input.GetAxis("Horizontal"); // change to GetAxisRaw for sharper movement with less smoothing

        if(reverseDirectionTimer < 1f && partConfiguration == 1)
        {
            reverseDirectionTimer += Time.deltaTime;
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, moveInput * movementSpeed, reverseDirectionTimer/1f), rb.velocity.y);
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
            leftGroundTimer += Time.deltaTime;
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
        raycastPos = transform.position; // this can be altered later if you would like it to change

        if(climbing == true)
        {
            if((climbingRight && Input.GetAxis("Horizontal") >= -0.05f) || (!climbingRight && Input.GetAxis("Horizontal") <= 0.05f))
            {
                jumpGate = true;
                rb.constraints = RigidbodyConstraints2D.FreezePositionX;
            }

            if(Input.GetAxis("Vertical") == 0f)
            {
                rb.constraints = RigidbodyConstraints2D.FreezePositionY;
            }
            else
            {
                rb.constraints = RigidbodyConstraints2D.FreezePositionY;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                rb.velocity = new Vector2(rb.position.x , Input.GetAxis("Vertical") * climbSpeed);
                rb.gravityScale = 0f;
            }
        }
        else
        {
            rb.gravityScale = 2f;
            rb.constraints = RigidbodyConstraints2D.None;
            if(partConfiguration != 1)
            {
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }

        // turn around
        if((facingRight == false && moveInput > 0) || facingRight == true && moveInput < 0)
        {
            facingRight = !facingRight;
            reverseDirectionTimer = 0f;
            Vector3 Scaler = transform.localScale;
            Scaler.x *= -1;
            transform.localScale = Scaler;
        }

/*
        // for groundbreakers
        if(timeSlow == true)
        {
            Time.timeScale += (1f / timeSlowDownLength) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
            if(Time.timeScale >= 1f || isGrounded == true)
            {
                Time.timeScale = 1f;
                //Time.fixedDeltaTime = Time.timeScale;
                groundBreakerActivate = true;
                timeSlow = false;
            }
        }
*/


// JUMPING
        if(Input.GetButton("Jump") && (jumpGate != true || (scaler && partConfiguration == 1)) && remainingJumps > 0f && !climbing) // scaler jump
        {
            remainingJumps --;
            jumpGateTimer = 0f;
            jumpGate = true;
            if(isGrounded == true || leftGroundTimer < 0.3f)
            {
                rb.velocity = Vector2.up * jumpPower;
            }
            else if(afterburner == true && !climbing && remainingJumps == 1f)
            {
                boostSprites.SetActive(true);
                rb.velocity = Vector2.up * jumpPower * 1.1f;
            }
        }

        if(jumpGate == true) // stops repeat jumping
        {
            jumpGateTimer += Time.deltaTime;
            if(Input.GetButtonUp("Jump"))
            {
                jumpGate = false;
            }
            else if(jumpGateTimer > jumpGateDuration)
            {
                jumpGate = false;
            }
        }

// JUMP TUNING
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
        else if(rb.velocity.y < 0f) // fast fall for impactful jumping... not great for the knees though (gravity inputs a negative value)
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

        if(partConfiguration == 1)
        {
            climbing = false;
            if(hitC.collider != null)
            {
                if((hitC.collider.gameObject.tag == "Legs" || hitC.collider.gameObject.tag == "Arms") && (transform.position.y > (0.1f + lastGroundedHeight) || (transform.position.y < (lastGroundedHeight - 0.08))))
                {
                    return false;
                }
            }
            else if(scaler == true)
            {
                if(Physics2D.OverlapCircle(gameObject.transform.position, 0.4f , jumpLayer))
                {
                    return true;
                }
            }
        }
        else if(partConfiguration == 2 || partConfiguration == 4) // for climbing ladders
        {
            if(moveInput > 0 || (facingRight == true && moveInput == 0))
            {
                RaycastHit2D sideHitR = Physics2D.Raycast(raycastPos , Vector2.right, 0.6f , ladderLayer);
                Debug.DrawRay(raycastPos, Vector2.right * 0.6f, Color.green);
                if(sideHitR.collider != null && sideHitR.collider.gameObject.tag == "Climbable")
                {
                    climbing = true;
                    climbingRight = true;
                    if(rb.velocity.y < 0f)
                    {
                        rb.gravityScale = 0.2f;
                    }
                    return true;
                }
                climbing = false;
            } 
            else if(moveInput < 0 || (facingRight == false && moveInput == 0))
            {
                RaycastHit2D sideHitL = Physics2D.Raycast(raycastPos , Vector2.left, 0.6f , ladderLayer);
                Debug.DrawRay(raycastPos, Vector2.left * 0.6f, Color.green);
                if(sideHitL.collider != null && sideHitL.collider.gameObject.tag == "Climbable")
                {
                    climbing = true;
                    climbingRight = false;
                    if(rb.velocity.y < 0f)
                    {
                        rb.gravityScale = 0.2f;
                    }
                    return true;
                }
                climbing = false;
            }
        }

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
            jumpGateDuration = 0.4f;
            jumpLayer = jumpLayer;

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
            pickupBoxCol.enabled = false; // can't pick up any more boxes
            scalerStar.transform.localScale = new Vector3(0.35f, 0.35f, 1f); // set the Scaler star/spikes to maximum size
        }


        else if (hasArms && !hasLegs)
        {
            partConfiguration = 2; // just a head and arms
            movementSpeed = augmentedMovementSpeed * 7.5f;
            jumpPower = jumpForce * 8.5f;
            jumpLayer = jumpLayer;

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
            pickupBoxCol.enabled = true; // the player can now pick up boxes
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
            jumpLayer = jumpLayer;
            
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
            pickupBoxCol.enabled = true;
            BoxDrop(); // no arms, so drop the box
        }


        else if(hasArms && hasLegs) // need to change collider
        {
            partConfiguration = 4; // has all parts
            movementSpeed = augmentedMovementSpeed * 8.5f;
            jumpPower = jumpForce * 11f;
            jumpLayer = jumpLayer;

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

            pickupBoxCol.enabled = true;
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
        rb.constraints = RigidbodyConstraints2D.FreezePositionY;
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

    void OnTriggerEnter2D(Collider2D col) // changes interactable box to the one the player approaches - still random if 2 boxes
    {
        if(col.tag == "Box" && holding == false) //&& pickupBox == null)
        {
            pickupBox = col.gameObject;
            boxInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D col) // can't grab the box if it's not in range
    {
        if(col.tag == "Box")
        {
            boxInRange = false;
        }
    }

    void BoxInteract()
    {
        if(pickupBox != null) // Pick up or drop boxes
        {
            if(Input.GetKeyDown("f") && (partConfiguration == 2 || partConfiguration == 4)) // pick up and drop box while you have arms and press "f"
            {
                if(holding == false && boxInRange == true)
                // currently the player character chooses from all boxes in range at random this may need to be changed if they can have multiple boxes at a time
                {
                    pickupBox.transform.parent = this.transform;
                    pickupBox.transform.position = boxHoldPos.position;
                    pickupBox.GetComponent<Rigidbody2D>().isKinematic = true;
                    pickupBox.GetComponent<Collider2D>().enabled = false;
                    heldBoxCol.enabled = true;
                    holding = true;
                } 
                else
                {
                    BoxDrop();
                    // if we eventually do want the box to be thrown, add some alternative code here
                }
            }
        }
    }
    
    void BoxDrop() // when the character drps the box;
    {
        if(pickupBox != null)
        {
            pickupBox.transform.parent = null;
            heldBoxCol.enabled = false;
            pickupBox.GetComponent<Rigidbody2D>().isKinematic = false;
            pickupBox.GetComponent<Collider2D>().enabled = true;
            holding = false;
        } 
    }
}
