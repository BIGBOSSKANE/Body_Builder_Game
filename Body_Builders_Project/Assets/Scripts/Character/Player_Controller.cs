/*
Creator: Daniel
Created: 09/04/2019
Last Edited by: Daniel
Last Edit: 26/05/2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    public int partConfiguration = 1; // 1 is head, 2 adds arms, 3 adds legs, 4 adds arms and legs
    public float movementSpeedAdjuster = 10f;
    float movementSpeed; // how fast can you move?
    float moveTimer;
    public float maxMoveTimer = 0.7f;
    float speed; // current speed
    float moveSpeedPrior; // used to track movement speed for speed increase Lerp 
    public float jumpForceAdjuster = 10f; // control the level of jumps
    private float jumpForce; // how powerful is your jump? altered from the jumpForceAdjuster by different part combinations
    bool jumpGate; // prevent the character from jumping while this is true (set to disable corner jumps eventually)
    bool cutJump; // removes a jump when you leave a platform
    float jumpGateTimer; // timer for jump gate
    float jumpGateLimit = 0.6f;
    bool leftGround; // did the player just leave a platform? - used to allow a short window to jump after the player falls off of a ledge
    public float leftGroundTimer; // how long ago did they leave the platform?
    private float moveInput; // get player Input value
    private bool facingRight = true; // used for flipping the character visuals (and arm interaction area)
    GameObject pickupBox; // the box that the player is currently picking up
    public Transform boxHoldPos; // determine where the held box is positioned
    public bool boxInRange = false; // is a box in the pickup range?
    public bool holding = false; // is the player holding a box?
    public CircleCollider2D heldBoxCol;  // if the player is holding a box, substitute this collider for it
    private Rigidbody2D rb; // this object's rigidbody
    public CapsuleCollider2D capCol; // collider used and adjusted when player is more than a head
    GameObject head; // the head component - make sure the head is called "head"
    GameObject arms; // the arms component - make sure all legs are called "Legs"
    GameObject legs; // the legs component - make sure all arms are called "Arms"
    public CircleCollider2D headCol; // collider used when the player is just a head
    public BoxCollider2D pickupBoxCol; // area in which a player can pick up boxes (and later climb walls)
    public bool isGrounded; //is the character on the ground?
    new public GameObject camera; // the scene's camera
    public GameObject groundChecker; // the ground checker object (used for the Scaler Augment)
    float groundCheckerRadius;
    float rayCastOffset; // alters raycast position based on character position
    Vector2 rayCastPos; // alters raycast origin based on player part configuration - good for stopping pass through platform jumps
    public LayerMask JumpLayer1; // what can the player jump on?
    public LayerMask JumpLayer2; // had 2 layers and combined them, because the Unity editor wasn't allowing multiple selections at an earlier dev stage
    private LayerMask canJumpOn; // combines the 2 layers that can be jumped on
    float groundedDistance = 0.3f; // distance of the grounded raycast
    public GameObject scalerStar; // the sprite for the Scaler Augment - starts disabled

    public int remainingJumps; // how many jumps beyond 1 does the player currently have?
    public int maximumJumps = 1; // how many jumps does the player have?

    public float fallMultiplier = 2.5f; // increase fall speed on downwards portion of jump arc
    public float unheldJumpReduction = 2f; // alters jump height based on duration of the jump button being held

    public string armString; // this is referenced from the name of the arm prefab the player collides with
    public string legString; // this is referenced from the name of the leg prefab the player collides with
    public string headString; // will be used later to recall head loadout - will be using later to instantiate prefabs for checkpoints


    // Augment related variables are listed here

    private bool afterburner = false;
    GameObject boostSprites;
    private bool groundbreaker = false;
    public float groundbreakerWaitTime = 0.4f; // the fall duration before groundbreakers activate



    void Start()
    {
        groundChecker = this.transform.Find("GroundChecker").gameObject;
        rb = GetComponent<Rigidbody2D>();
        capCol = GetComponent<CapsuleCollider2D>();
        head = this.transform.Find("Head").gameObject;
        headCol = head.GetComponent<CircleCollider2D>();
        pickupBoxCol = this.transform.Find("BoxHoldLocation").gameObject.GetComponent<BoxCollider2D>();
        remainingJumps = maximumJumps;
        canJumpOn = JumpLayer1 | JumpLayer2;
        jumpForce = jumpForceAdjuster;
        movementSpeed = movementSpeedAdjuster;
        UpdateParts();
        heldBoxCol.enabled = false;
        groundedDistance = 0.34f;
        cutJump = false;
        leftGroundTimer = 0f;
        camera = GameObject.Find("Main Camera");
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundChecker.transform.position, groundCheckerRadius);
    }

    void FixedUpdate() // This covers all movement speed, and "isGrounded" for the Scaler head augment
    {
        rayCastPos = new Vector2(transform.position.x , transform.position.y + rayCastOffset);

        if(!groundCheckRaycast()) // slow the player's horizontal movement in the air
        {
            speed = movementSpeed / 1.1f;
        }
        else
        {
            speed = movementSpeed;
        }

        moveInput = Input.GetAxis("Horizontal"); // change to GetAxisRaw for sharper movement with less smoothing
        rb.velocity = new Vector2(moveInput * speed , rb.velocity.y);

        // Checks for Ground while in Scaler mode

        isGrounded = Physics2D.OverlapCircle(groundChecker.transform.position, groundCheckerRadius , canJumpOn); // returns true if circular ground checker overlaps a jumpable layer
    }

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

    void Update()
    {
        if((facingRight == false && moveInput > 0) || (facingRight == true && moveInput < 0))  // if changing directions, flip sprite
        {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
        }

        Debug.DrawRay(rayCastPos, Vector2.down * groundedDistance, Color.green);
        // visualises the groundCheckRaycast in the editor so you can see it with your face


        if(jumpGate == true && jumpGateTimer < jumpGateLimit) // the player can't jump again while this is true
        // this prevents jumping twice when moving through pass through platforms, and also stops edge jumping
        {
            jumpGateTimer += Time.deltaTime;
        }
        else
        {
            jumpGate = false;
            if(TrueGroundCheck()) // won't automatically check for ground for the first 0.4 seconds after jumping
            {
                remainingJumps = maximumJumps;
                leftGround = false;
                cutJump = false;
            }
            else
            {
                leftGround = true;
            }
        }

        if(leftGround == true) // if the timer is below a number, the player can still jump as though they were on the platform
        {
            leftGroundTimer += Time.deltaTime;

            if(leftGroundTimer < 0.1f && cutJump == false)
            {
                remainingJumps --;
                cutJump = true;
            }
            groundChecker.SetActive(false);
        }

 // An attempt to create slow mo when the groundbreakers shapeshift
        if(leftGroundTimer >= groundbreakerWaitTime - 0.1f && leftGroundTimer <= groundbreakerWaitTime + 0.1f && groundbreaker == true)
        {
            Time.timeScale = 0.2f;
            //Time.fixedDeltaTime *= 0.2f;
        }
        else
        {
            Time.timeScale = 1f;
        }


        if(partConfiguration == 1 && headString == "Scaler" && Input.GetButton("Jump")) // different rules for Scaler Script
        {
            if((TrueGroundCheck() || leftGroundTimer < 0.3f ) && remainingJumps > 0f)
            {
                rb.velocity = Vector2.up * jumpForce;
                remainingJumps --;
            }
        }
        else
        {
            if (Input.GetKeyDown("space") && partConfiguration != 1)// && remainingJumps > 0)
            // eventually set this to create prefabs of the part, rather than a detached piece
            {
                if(partConfiguration > 2)
                {
                    legs.GetComponent<Legs>().Detached();
                    legs.transform.parent = null;
                    remainingJumps --; // consumes jumps but doesn't require them to be used
                    UpdateParts();
                    rb.velocity = Vector2.up * jumpForce * 1.2f;
                }
                else if(partConfiguration == 2)
                {
                    arms.GetComponent<Arms>().Detached();
                    arms.transform.parent = null;
                    remainingJumps --; // consumes jumps but doesn't require them to be used
                    UpdateParts();
                    rb.velocity = Vector2.up * jumpForce * 1.2f;
                }
            } 

            
            if(Input.GetButton("Jump") && TrueGroundCheck() && jumpGate == false) // jumping from a platform
            {
                rb.velocity = Vector2.up * jumpForce;
                jumpGateTimer = 0f;
                jumpGate = true;
                
                // remaining jumps is not reduced here, as it would just reset on the next frame's ground check, it is reduced by the leftGroundTimer instead
            }
            else if(Input.GetButton("Jump") && !TrueGroundCheck() && remainingJumps == maximumJumps && jumpGate == false && leftGroundTimer > 0.3f) // if jumping after leftGround off of a ledge
            {
                remainingJumps --;
                if(remainingJumps > 0f)
                {
                    rb.velocity = Vector2.up * jumpForce;
                    jumpGateTimer = 0f;
                    remainingJumps --;
                    jumpGate = true;
                }
            }
            else if(Input.GetButton("Jump") && !TrueGroundCheck() && remainingJumps > 0f && jumpGate == false)
            {
                remainingJumps --;
                jumpGateTimer = 0f;
                jumpGate = true;
                if(afterburner == false)
                {
                    rb.velocity = Vector2.up * jumpForce;
                }
                else if(remainingJumps == 0f && afterburner == true)
                {
                    boostSprites.SetActive(true);
                    rb.velocity = Vector2.up * jumpForce * 1.1f;
                }
            }

            if(Input.GetButtonUp("Jump"))
            {
                jumpGate = false;
            }    
        }


    // Jump tuning
        if(rb.velocity.y < 0f && afterburner == true && Input.GetButton("Jump")) // afterburner glide
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.deltaTime * 0.0005f;
            if(boostSprites != null)
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
        else if (rb.velocity.y > 0f && !Input.GetButton("Jump")) // reduces jump height when button isn't held (gravity inputs a negative value)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (unheldJumpReduction - 1) * Time.deltaTime;
            if(boostSprites != null)
            {
                boostSprites.SetActive(false);
            }       
        }
    

        if(pickupBox != null) // Pick up or drop boxes
        {
            if(Input.GetKeyDown("f") && (partConfiguration == 2 || partConfiguration == 4)) // pick up and drop box while you have arms and press "f"
            {
                if(holding == false && boxInRange == true)
                // currently the player character chooses from all boxes in range at random this may need to be changed if they can have multiple boxes at a time
                {
                    pickupBox.transform.parent = this.transform;
                    BoxPickup();
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

    void BoxPickup() // when picking up the box, replace its collider with a substitute and make it kinematic
    {
        pickupBox.transform.position = boxHoldPos.position;
        pickupBox.GetComponent<Rigidbody2D>().isKinematic = true;
        pickupBox.GetComponent<Collider2D>().enabled = false;
        heldBoxCol.enabled = true;
        holding = true;
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

    bool groundCheckRaycast() // instantly returns "true" if raycast hits a layer in the canJumpOn layermask, also sets isGrounded to true
    // for the player in rolling head (non-scaler) mode
    {
        RaycastHit2D hit = Physics2D.Raycast(rayCastPos, Vector2.down, groundedDistance, canJumpOn);
        if(hit.collider != null)
        {
            if(groundbreaker == true && hit.collider.gameObject.tag == "Groundbreakable" && leftGroundTimer >= groundbreakerWaitTime) // left ground time is not working
            {
                hit.collider.gameObject.GetComponent<Groundbreakable_Script>().Groundbreak();
                Debug.Log("Groundbroken");
                return false;
            }
            else
            {
                isGrounded = true;
                if(boostSprites != null)
                {
                    boostSprites.SetActive(false);
                }
                return true;
            }
        }
        else
        {
            return false;
        }
    }


    public bool TrueGroundCheck()
    {
        groundCheckRaycast();
        groundChecker.SetActive(true);
        isGrounded = Physics2D.OverlapCircle(groundChecker.transform.position, groundCheckerRadius , canJumpOn);
        if(groundCheckRaycast() || isGrounded == true)
        {
            leftGroundTimer = 0f;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UpdateParts()
    // call when acquiring or detaching part - reconfigures scaling, controls and colliders
    // arms need to have "Arms" tag, and legs must have the "Legs" tag
    // 1 is head, 2 adds torso, 3 adds legs, 4 adds torso and legs
    {
        // assume that the robot has neither arms or legs, then check
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
            movementSpeed = movementSpeedAdjuster * 0.5f;
            jumpForce = jumpForceAdjuster * 0.7f;
            jumpGateLimit = 0.4f;

            // upgrades
            maximumJumps = 1;
            groundbreaker = false;
            afterburner = false;
            if(boostSprites != null)
            {
                boostSprites.SetActive(false);
                boostSprites = null;
            }

            if(headString == "Scaler") // reduces jump power if you have the Scaler Augment as a trade-off
            {
                jumpForce = jumpForceAdjuster * 0.6f;
                groundCheckerRadius = 0.45f;
                fallMultiplier = 1f;
            }
            else
            {
                jumpForce = jumpForceAdjuster * 0.7f;
                groundCheckerRadius = 0.2f;
                fallMultiplier = 2.5f;
            }

            capCol.enabled = false; // don't use the typical vertical standing collider
            headCol.enabled = true; // use the circle collider instead
            rb.constraints = RigidbodyConstraints2D.None; // can roll
            head.transform.position = gameObject.transform.position; // no need for snapOffsetPos here as it is perfectly centred
            groundChecker.transform.position = gameObject.transform.position; // centre the groundChecker
            legString = "None"; // no legs
            armString = "None"; // no arms
            rayCastOffset = 0f;

            BoxDrop(); // drops any box immediately
            pickupBoxCol.enabled = false; // can't pick up any more boxes
            scalerStar.transform.localScale = new Vector3(0.35f, 0.35f, 1f); // set the Scaler star/spikes to maximum size
        }


        else if (hasArms && !hasLegs)
        {
            partConfiguration = 2; // just a head and arms
            movementSpeed = movementSpeedAdjuster * 0.75f;
            jumpForce = jumpForceAdjuster * 0.85f;     

            NonHeadConfig();
            fallMultiplier = 3f;
            arms = gameObject.transform.Find(armString).gameObject;
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
            groundChecker.transform.localPosition = new Vector2(0f, -0.50f);
            groundCheckerRadius = 0.292f;
            rayCastOffset = -0.59f;

            capCol.size = new Vector2(0.6f , 1.6f);
            capCol.offset = new Vector2(0f , 0.03f);
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
            movementSpeed = movementSpeedAdjuster * 0.85f;
            jumpForce = jumpForceAdjuster * 1.1f;            
            
            NonHeadConfig();
            legs = gameObject.transform.Find(legString).gameObject;
            armString = "None"; // no arms
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
            groundChecker.transform.localPosition = new Vector2(0f, -0.75f);
            groundCheckerRadius = 0.292f;
            rayCastOffset = -0.7f;

            capCol.size = new Vector2(0.6f , 1.45f);
            capCol.offset = new Vector2(0f , -0.27f);
            pickupBoxCol.enabled = true;
            BoxDrop(); // no arms, so drop the box
        }


        else if(hasArms && hasLegs) // need to change collider
        {
            partConfiguration = 4; // has all parts
            movementSpeed = movementSpeedAdjuster * 0.85f;
            jumpForce = jumpForceAdjuster * 1.1f;

            NonHeadConfig();
            arms = gameObject.transform.Find(armString).gameObject;
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
            groundChecker.transform.localPosition = new Vector2(0f, -0.75f);
            groundCheckerRadius = 0.292f;
            rayCastOffset = -0.73f;

            capCol.size = new Vector2(0.6f , 2.08f);
            capCol.offset = new Vector2(0f , 0.03f);

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
            scalerStar.SetActive(false);
        }

        camera.GetComponent<Camera2DFollow>().Resize(partConfiguration);
    }

    void NonHeadConfig() // Generic changes for non-head part updates (referenced in UpdateParts)
    {
        headCol.enabled = false; // disable the rolling head collider
        capCol.enabled = true; // use the capsule collider instead
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // can no longer roll
        scalerStar.transform.localScale = new Vector3(0.25f, 0.25f, 1f); // shrink the scaler star to signify it is no longer usable
        transform.rotation = Quaternion.identity; // lock rotation to 0;
        isGrounded = false;
        fallMultiplier = 4f;
        rb.velocity = Vector2.zero;
        moveInput = 0f;
        jumpGate = true;
        jumpGateLimit = 0.6f;
        jumpGateTimer = jumpGateLimit - 0.1f;
    }
}