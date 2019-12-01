/*
Creator: Daniel
Created 04/08/2019
Last Edited by: Daniel
Last Edit 06/09/2019
*/

/*

To do:
 - Bladebot
 - Refine screenshake
 - Sound implementation
 - Animation Implementation
 - Level Design


The wall jump is failing because normal jump is being called first

Altering wall jump movement in the air isn't feeling smooth, because deceleration is being applied, and then the target velocity > current velocity clause is completely shifting the movement speed.

Wall jumping upwards disables groundchecker, meaning that if you hit a ceiling, you instantly bounce off if you jumped from a short enough distance - solved?



Still need to fix:

    try to get animations in from > sprites > V's Animations

    currently when w is held and you have the scaler augment, you move up walls without rolling, if rotate speed is too low while w is held, lerp it up

    wall jumping allows you to jump straight up, don't use walls until wall jump is properly implemented from the addition section below (later on)

    Fix up hit ray on scaler form so that ground check isn't disabled when colliding with the ceiling


Still need to add:

    headbanger - like groundbreaker legs, but requiring a speed threshold to gain armour plates - jump - swing break puzzle
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class playerScript : MonoBehaviour
{
// Checkpoint

    private Vector2 currentSpawnPoint;

// BASIC MOVEMENT
    public bool spaceForJump = false;
    [HideInInspector] public bool forceSlaved = false;
    [HideInInspector] public bool scalingWall = false;
    private float inputX; // get player Input value
    Vector2 movementInput;
    Vector2 rawMovementInput;
    [HideInInspector] public int rawInputX;
    [HideInInspector] public int facingDirection; // used to flip the character when turning
    [HideInInspector] public float reverseDirectionTimer = 0f;
    private float startingAngularDrag;

    bool mouseMoved = false;

    Vector2 previousMousePos;

    [HideInInspector] public float speed; // current movement speed
    [Tooltip("Maximum player inputted horizontal movement")] public float movementSpeed = 10f; // the max horizontal movement speed
    float augmentedMovementSpeed = 1f; // scales the movement limit with parts


// JUMPING
    float jumpPower; // the current jump force based on augments
    [Tooltip("Jump force multiplier")] public float jumpForce = 1f; // modify this to change the jump force between loadouts with equal ratios`
    [Tooltip("Velocity applied to the player when they leap from a wall")] public Vector2 wallJumpForce = new Vector2(8f , 8f); // force of wall jump sideways propulsion
    [Tooltip("How long does the player have to jump after falling from a ledge?")] public bool coyoteTime; // if the player just fell of a platform, they can still use their first jump in mid air
    bool coyoteJump; // is the player within the coyote time limit?
    [Tooltip("How long does the player have to jump after falling from a ledge?")] public float coyoteTimeLimit = 0.3f;
    [HideInInspector] public float lastGroundedHeight; // the height you were at when you were last grounded
    float leftGroundTimer; // how long ago were you last grounded
    [HideInInspector] public bool fastFall = true;


    public bool isGrounded; // is the player on the ground?
    [HideInInspector] public float maxHeight; // the maximum height of the jump
    [HideInInspector] public bool jumpGate; // prevent jumping while this is true
    [HideInInspector] public bool jumpBan; // is an external script preventing the player from jumping (like the slam up elevator)
    [HideInInspector] public float jumpGateTimer; // timer for jump gate
    float jumpGateDuration = 0.6f; // the duration of the jumpGate

    public int remainingJumps; // how many jumps does the player have left?
    [HideInInspector] public int maximumJumps = 1; // how many jumps does the player have?

    float fallMultiplier = 2.5f; // increase fall speed on downwards portion of the jump arc
    float unheldJumpReduction = 2f; // reduces jump power if jump button isn't held

    float raycastXOffset; // alters the distance between groundchecker raycasts based on part configuration
    float raycastYOffset; // alters raycast length based on character part configuration, this should be combined with "raycastPos"
    Vector2 raycastPos; // controls where groundcheckers come from
    float groundedDistance = 0.15f; // raycast distance;
    [Tooltip("What layers can the player jump on?")] public LayerMask jumpLayer; // what layers can the player jump on?
    [Tooltip("Normal Jump Layer + Parts")] public LayerMask nonHeadJumpLayer; // what layers can the player jump on?
    [Tooltip("What layer are ladders on?")] public LayerMask ladderLayer; // what cn the player climb?
    [Tooltip("What layers can the player pick up?")] public LayerMask pickupLayer; // what things can the player pick up?
    [Tooltip("What layers does the laser hit?")] public LayerMask laserLayer; // what can the deflected laser hit?

    [HideInInspector] public Vector2 tetherPoint;
    [Tooltip("Force of swinging while tethered and using the hookshot")] public float swingForce = 4f;
    [HideInInspector] public bool isSwinging;


// AUGMENTS AND PARTS

    [Tooltip("1 is head, 2 is head and arms, 3 is head and legs, 4 is full body")] [Range (1 , 4)] public int partConfiguration = 1;
    [Tooltip("1 is basic head, 2 is scaler, 3 is hookshot, 4 is all augments")] [Range (1 , 4)] public int headConfiguration = 1;
    [Tooltip("1 is basic, 2 is LifterArms, 3 is DeflectorArms")] [Range (1 , 3)] public int armConfiguration = 1;
    [Tooltip("1 is basic, 2 is groundbreakers, 3 is afterburners")] [Range (1 , 3)] public int legConfiguration = 1;
    [HideInInspector] public string headString; // this is referenced from the name of the head augment
    [HideInInspector] public bool scaler = false;
    [HideInInspector] public bool hookShot = false;

    // Arms
    GameObject arms; // the arms component
    [HideInInspector] public string armString; // this is referenced from the name of the arm prefab
    bool holding = false; // is the player holding a box?
    [HideInInspector] public GameObject heldBox;
    [HideInInspector] public string heldBoxTag;
    bool lifter = false; // do you have the lifter augment?
    [HideInInspector] public bool shield = false; // do you have the shield augment?
    bool climbing = false; // are you climbing?
    [HideInInspector] public bool wallSliding = false; // are you on a wall-jumpable surface
    float wallSlideSpeedMax = 0.1f; // how fast can you slide down walls
    bool wasClimbing = false; // did you just stop climbing?
    float climbingDismountTimer = 0f; // wall jump boost timer
    int climbDirection = 1; // what side are you climbing on?
    float climbSpeed = 10f; // climbing speed

    // Legs
    GameObject legs; // the legs component
    [HideInInspector] public string legString; // this is referenced from the name of the leg prefab
    [HideInInspector] public bool groundbreaker = false; // do you have the groundbreaker legs?
    bool afterburner = false; // do you have the afterburner legs equipped?
    GameObject boostSprites; // sprites used for rocket boots
    [Tooltip("Distance that the player must descend to activate groundbreakers")] public float groundbreakerDistance = 4f; // have you fallen far enough to break through ground
    bool groundBreakerReset; // used to make sure time slow is only used once

// ATTACHABLES AND PARTS
    float boxDetectorOffsetX = 0.68f;
    float boxDetectorOffsetY = 0.05f;
    Vector2 boxDetectorCentre;
    Vector2 boxDetectorSize = new Vector2(0.8f , 0.8f);
    Rigidbody2D rb; // this object's rigidbody
    BoxCollider2D boxCol; // this object's capsule collider - potentially swap out for box collider if edge slips are undesireable
    GameObject head; // the head component
    CircleCollider2D headCol; // the collider used when the player is just a head
    new Camera camera; // the scene's camera
    GameObject scalerAugment; // the sprite for the Scaler Augment - starts disabled
    GameObject hookshotAnchor;
    GameObject hookshotAugment;
    hookShot hookshotScript;
    bool wasGrounded; // was the player grounded on the last frame?

    // Arms
    Transform boxHoldPos; // determine where the held box is positioned
    CircleCollider2D heldBoxCol; // this collider is used for the held box
    CapsuleCollider2D heldPowerCellCol; // the collider used for the power cell while held
    GameObject closestBox; // closest box as determined by the box assigner
    timeSlow timeSlowScript; // time slow script
    Camera2DFollow cameraScript;
    bool cameraAdjuster;

    GameObject shieldBubble; // the shield bubble object
    [HideInInspector] public bool shieldActive = false;
    bool isDeflecting = false; // is the player currently deflecting
    bool firingLaser = false;
    bool wasFiringLaser = false;
    GameObject collisionEffect;
    GameObject burstEffect;
    Vector2 laserOrigin;
    float shieldUnmodifiedRadius;
    float shieldModifiedRadius;
    float shieldRadius;
    LineRenderer laserLine;
    [HideInInspector] public Vector2 ropeDirection;
    [Tooltip("The fritction physics material")] public PhysicsMaterial2D frictionMaterial;
    [Tooltip("The slippery physics material")] public PhysicsMaterial2D slipperyMaterial;
    string laserTag;
    laserRouter laserRouter;
    powerCell powerCell;
    powerStation powerStation;
    Vector2 laserEndpoint;
    [HideInInspector] public float forceSlavedTimer = 0f;
    int previousFacingDirection;
    [HideInInspector] public bool scalerTrueGrounded = false;
    float deathTimer = 0;
    [HideInInspector] public bool dying;
    [HideInInspector] public bool lockController = false;

    bool wallHang = false;
    [Tooltip("Laser material while not firing")] public Material laserMaterialAim;
    [Tooltip("Laser material while firing")] public Material laserMaterialFire;
    int previousPartConfiguration;
    float jumpDisableTimer = 10f;
    Vector2 previousVelocity;
    bool ceilingAbove = false;
    float afterburnerApex;
    bool afterburnerGlide = false;
    checkpointData checkpointData;

    [HideInInspector] public int armIdentifier = 0;
    [HideInInspector] public int legIdentifier = 0;
    [HideInInspector] public int augmentScalerIdentifier = 0;
    [HideInInspector] public int augmentHookshotIdentifier = 0;
    [HideInInspector] public Vector2 currentVelocity;
    playerSound playerSound;

    [Tooltip("Animation Functions Sub-Object")] public RobotAnimations anims;
    [HideInInspector] public laserBurstColour burstColour;
    [HideInInspector] public laserBurstColour collisionColour;
    bool wasDying = false;
    // Jump Sounds
    bool justJumped;

    bool touchedGroundSinceSpawn = false;
    // set this to true upon jumping, if the groundcheck returns false after this happened, make it false and play the jump sound

    [HideInInspector] public bool paused = false;



    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCol = gameObject.GetComponent<BoxCollider2D>();
        head = gameObject.transform.Find("Head").gameObject;
        headCol = head.GetComponent<CircleCollider2D>();
        scalerAugment = gameObject.transform.Find("Head").gameObject.transform.Find("ScalerHead").gameObject;
        scalerAugment.SetActive(false);
        hookshotScript = gameObject.GetComponent<hookShot>();
        hookshotScript.enabled = false;
        hookshotAugment = gameObject.transform.Find("Head").gameObject.transform.Find("HookshotHead").gameObject;
        hookshotAugment.SetActive(false);
        hookshotAnchor = gameObject.transform.Find("HookshotAnchor").gameObject;
        hookshotAnchor.SetActive(false);
        hookShot = false;
        gameObject.transform.Find("BoxHoldLocation").gameObject.SetActive(true);
        heldBoxCol = gameObject.transform.Find("BoxHoldLocation").gameObject.GetComponent<CircleCollider2D>();
        heldPowerCellCol = gameObject.transform.Find("BoxHoldLocation").gameObject.GetComponent<CapsuleCollider2D>();
        boxHoldPos = gameObject.transform.Find("BoxHoldLocation").gameObject.transform;
        heldPowerCellCol.enabled = false;
        heldBoxCol.enabled = false;
        camera = Camera.main;
        cameraScript = camera.GetComponent<Camera2DFollow>();
        cameraAdjuster = true;
    }

    void Start()
    {
        startingAngularDrag = rb.angularDrag;
        timeSlowScript = gameObject.GetComponent<timeSlow>();
        leftGroundTimer = 0f;
        raycastXOffset = 0.1f;
        reverseDirectionTimer = 0f;
        isGrounded = false;
        raycastPos = transform.position;
        lastGroundedHeight = -1000f;
        climbingDismountTimer = 1f;
        shieldBubble = gameObject.transform.Find("shieldBubble").gameObject;
        shieldUnmodifiedRadius = shieldBubble.GetComponent<CircleCollider2D>().radius;
        shieldModifiedRadius = shieldBubble.transform.localScale.x;
        shieldRadius = shieldUnmodifiedRadius * shieldModifiedRadius + 0.01f;
        shieldBubble.SetActive(false);
        laserLine = gameObject.GetComponent<LineRenderer>();
        collisionEffect = gameObject.transform.Find("collisionEffectPosition").gameObject;
        burstEffect = gameObject.transform.Find("burstEffectPosition").gameObject;
        collisionColour = collisionEffect.GetComponent<laserBurstColour>();
        burstColour = burstEffect.GetComponent<laserBurstColour>();
        currentSpawnPoint = transform.position;
        collisionEffect.SetActive(false);
        burstEffect.SetActive(false);
        isSwinging = false;
        facingDirection = 1;
        previousPartConfiguration = 0;
        checkpointData = gameObject.GetComponent<checkpointData>();
        camera.transform.position = new Vector3(transform.position.x , transform.position.y , camera.transform.position.z);
        rb.sharedMaterial = frictionMaterial;
        playerSound = GetComponent<playerSound>();
        playerSound.Respawn();
        touchedGroundSinceSpawn = false;
    }

    void FixedUpdate()
    {
        if(!paused)
        {
            if(dying) // hold the death frame of death for a short while to let the player know what killed them
            {
                if(!wasDying)
                {
                    playerSound.DeathPlay();
                    wasDying = true;
                }
                deathTimer -= Time.fixedDeltaTime;
                rb.velocity = Vector2.zero;
                if(deathTimer <= 0 )
                {
                    GameObject.Find("GameManager").GetComponent<gameManager>().RestartLevel();
                }
                return; // stop the player from doing anything, we could later replace this with a time slow effect
            }

            previousVelocity = rb.velocity;
            Vector2 targetVelocity = Vector2.zero;

            if(InputManager.ToggleScout())
            {
                if(!lockController)
                {
                    cameraScript.ToggleScoutMode(true);
                    cameraScript.Resize(5 , cameraScript.standardResizeDuration , 1f); // resize the camera for scout mode
                    lockController = true;
                    inputX = 0f;
                    rawInputX = 0;
                }
                else
                {
                    cameraScript.ToggleScoutMode(false);
                    cameraScript.Resize(partConfiguration , 0.4f , 1f);
                    lockController = false;
                }
            }

            if(!lockController)
            {
                movementInput = InputManager.JoystickLeft();
                rawMovementInput = InputManager.JoystickLeftRaw();
                inputX = movementInput.x;
                rawInputX = Mathf.RoundToInt(2f * rawMovementInput.x)/2; //get the pure integer value of horizontal input
            }
            else
            {
                inputX = 0f;
                rawInputX = 0;
            }

            if(((facingDirection == -1 && rawInputX > 0.5) || facingDirection == 1 && rawInputX < -0.5) && !isSwinging)
            {
                reverseDirectionTimer = 0f;
                if(Mathf.Abs(rawInputX) >= 0.5f)
                {
                    facingDirection = Mathf.CeilToInt(rawInputX);
                    transform.localScale = new Vector3(facingDirection , 1 , 1);
                }
            }

            if(isGrounded) // check if the player has just landed or if they have been on the ground for a while
            {
                wasGrounded = true;
            }
            else
            {
                wasGrounded = false;
            }

            if(partConfiguration == 1) // this was added to fix a Unity bug where the box collider would be active whilst disabled
            {
                Destroy(boxCol);
            }


            if(GroundCheck() == true)
            {
                rb.gravityScale = 2f; // restore gravity to normal value (after changed by swinging or rocket boost legs)
                isGrounded = true;
                coyoteJump = true;
                fastFall = true;

                timeSlowScript.TimeNormal(); // disable any time slow effects

                if(maxHeight > (1f + transform.position.y) && touchedGroundSinceSpawn) // cause the ground to shake if you just landed
                {
                    float shakeAmount = maxHeight - transform.position.y;
                    cameraScript.TriggerShake(shakeAmount , false , partConfiguration);
                    if(!(ceilingAbove && scaler)) playerSound.LandingPlay();
                }

                touchedGroundSinceSpawn = true;

                maxHeight = transform.position.y; // reset maxHeight
                lastGroundedHeight = transform.position.y; // reset lastGroundedHeight
                leftGroundTimer = 0f; // reset the jump time
                remainingJumps = maximumJumps; // reset remaining jumps
                if(boostSprites != null) // disable jump booster sprites if they were active
                {
                    boostSprites.SetActive(false);
                }
                
                //ANIMATION CODE - LAND
                if (!wasGrounded && anims != null)
                {
                    anims.Land();
                }
            }
            else // if not grounded
            {
                leftGroundTimer += Time.fixedDeltaTime; // try swapping back to deltaTime if this isn't working

                if(justJumped)
                {
                    justJumped = false;
                    playerSound.JumpPlay();
                }

                if(leftGroundTimer > coyoteTimeLimit && coyoteJump)
                {
                    if(remainingJumps == maximumJumps && coyoteTimeLimit > jumpGateDuration)
                    {
                        remainingJumps --;
                    }
                    coyoteJump = false;
                }

                if(transform.position.y > maxHeight) // set the maximum height of the current air time
                {
                    maxHeight = transform.position.y;
                }

                isGrounded = false;
                
            }
        
        // Controls

            if(isSwinging) // disables the velocity defining parameters to allow a force to be applied
            {
                rb.gravityScale = 3f; // lower gravity for a floatier swing

                forceSlaved = true;
                forceSlavedTimer = 0f;
                
                // optimised swinging code sourced from raywenderlich.com - Sean Duffy
                Vector2 playerToHookDirection = ((Vector2)hookshotAnchor.transform.position - (Vector2)transform.position).normalized;

                if((!wasGrounded || playerToHookDirection.y < 0.8f) && scalerTrueGrounded) // if you are swinging, and hit the ground, detach the rope
                {
                    hookshotScript.DetachRope();
                }

                Vector2 perpendicularDirection = Vector2.zero;
                if (rawInputX < 0)
                {
                    perpendicularDirection = new Vector2(-playerToHookDirection.y, playerToHookDirection.x);
                    Vector2 leftPerpPos = (Vector2)transform.position - perpendicularDirection * -2f;
                    Debug.DrawLine(transform.position, leftPerpPos, Color.green, 0f);
                }
                else if(rawInputX > 0)
                {
                    perpendicularDirection = new Vector2(playerToHookDirection.y, -playerToHookDirection.x);
                    Vector2 rightPerpPos = (Vector2)transform.position + perpendicularDirection * 2f;
                    Debug.DrawLine(transform.position, rightPerpPos, Color.green, 0f);
                }

                Vector2 force = perpendicularDirection * swingForce;
                rb.AddForce(force, ForceMode2D.Force);
            }
            else
            {
                if(reverseDirectionTimer < 1f && partConfiguration == 1 && climbingDismountTimer > 0.1f)
                {
                    reverseDirectionTimer += Time.fixedDeltaTime;
                    reverseDirectionTimer = (reverseDirectionTimer > 1f)? 1f : reverseDirectionTimer;
                    targetVelocity = new Vector2(Mathf.Lerp(targetVelocity.x, inputX * movementSpeed, reverseDirectionTimer), rb.velocity.y);
                }
                else if(climbingDismountTimer <= 0.1f && climbing == false) // if jumping from wall, give some extra force
                {
                    targetVelocity = new Vector2(inputX * movementSpeed * 1.2f, rb.velocity.y);
                    climbingDismountTimer += Time.deltaTime;
                }
                else
                {
                    targetVelocity = new Vector2(inputX * movementSpeed, rb.velocity.y);
                }

                if(scalingWall) // Scaler Climbing and Jumping
                {
                    if(rawMovementInput.y > 0 || InputManager.Climb()) // if the player is in scaler mode, and rolling up a wall, give them a jump boost when moving away from it
                    {
                        if(Mathf.Abs(rawInputX) >= 0.1f)
                        {
                            if((rawInputX == -climbDirection && !ceilingAbove && !InputManager.gamepad) || (InputManager.ButtonA() && InputManager.gamepad)) // wall leap
                            {
                                forceSlaved = true;
                                forceSlavedTimer = 0f;
                                rb.velocity = new Vector2(rawInputX * wallJumpForce.x, wallJumpForce.y);
                                previousVelocity = rb.velocity;
                                playerSound.JumpPlay();
                                if(remainingJumps == maximumJumps)
                                {
                                    remainingJumps --;
                                }
                                jumpDisableTimer = 0f;
                            }
                            else if(jumpDisableTimer > 0.2f && wallHang)
                            {
                                inputX = facingDirection; // apply force towards wall to let player rotate
                                targetVelocity = new Vector2(inputX * movementSpeed, jumpPower);
                            }
                        }
                        else if(jumpDisableTimer > 0.2f && wallHang)
                        {
                            inputX = facingDirection; // apply force towards wall to let player rotate
                            targetVelocity = new Vector2(inputX * movementSpeed, jumpPower);
                        }
                    }
                    else
                    {
                        if(rawMovementInput.y < 1 && wallHang) // roll down the wall
                        {
                            inputX = facingDirection;
                            targetVelocity = new Vector2(inputX * movementSpeed, rb.velocity.y);

                            if(rawMovementInput.y == 0 && inputX == climbDirection)
                            {
                                targetVelocity = new Vector2(inputX * movementSpeed , 0f);
                            }
                        }
                    }
                }
                
                if(forceSlaved && !isSwinging) // in air after elevator slam or wall jump
                {
                    forceSlavedTimer += Time.fixedDeltaTime;
                    if(forceSlavedTimer > 0.2f && isGrounded)
                    {
                        forceSlaved = false;
                    }

                    if(Mathf.Abs(targetVelocity.x) > Mathf.Abs(previousVelocity.x)) // && (facingDirection == Mathf.RoundToInt(Mathf.Sign(previousVelocity.x)) || Mathf.RoundToInt(previousVelocity.x) == 0)) // don't allow speed to be added in the x direction of the force, unless the speed is greater than that provided by the force
                    {
                        rb.velocity = new Vector2(targetVelocity.x , rb.velocity.y);
                    }

                    if(Mathf.Abs(targetVelocity.y) > Mathf.Abs(previousVelocity.y)) // don't allow speed to be added in the y direction of the force, unless the speed is greater than that provided by the force
                    {
                        rb.velocity = new Vector2(rb.velocity.x , targetVelocity.y);
                    }
                    
                    if(facingDirection != Mathf.RoundToInt(Mathf.Sign(previousVelocity.x)) && Mathf.Abs(targetVelocity.x) < Mathf.Abs(previousVelocity.x)) // air control after elevator slam or wall jump
                    {
                        // smoothing
                        float targetClamp = targetVelocity.x;
                        targetVelocity.x = Mathf.Abs(Mathf.Abs(targetVelocity.x) * ((Mathf.Clamp(forceSlavedTimer , 0 , 1)))); // + Mathf.Abs(targetVelocity.x)));
                        Mathf.Clamp(targetVelocity.x , 0f , targetClamp);
                        rb.velocity += new Vector2(facingDirection * targetVelocity.x , 0f);
                    }
                }
                else
                {
                    rb.velocity = targetVelocity;
                }

                //ANIMATION CODE, MOVEMENT
                if (anims != null)
                {
                    anims.HandleMovement(rb.velocity);
                }


    // Non-Scaler JUMPING ------------------------------------------------------------------------------------------------------------------
                if(!jumpBan && !lockController) Jump(); // jump ban is applied by the elevator script to lock the player to it when slamming upwards


                previousFacingDirection = facingDirection;
            }

            speed = rb.velocity.x;
            currentVelocity = rb.velocity;
        }
    }

    public void Jump()
    {
        if(InputManager.Jump() && remainingJumps > 0f && !scalingWall && jumpDisableTimer > 0.2f && !climbing && (!jumpGate || (scaler && partConfiguration == 1)) && !lockController && !isSwinging)
        // this last bit ensures the player can always jump, which is how the spiderclimb works
        {
            if(isGrounded || (coyoteTime && coyoteJump))
            {
                rb.velocity = new Vector2(rb.velocity.x , jumpPower);
                
                //ANIMATION CODE - JUMP
                if (anims!= null)
                    anims.Jump();            
            }
            else if(afterburner == true && !climbing && remainingJumps == 1)
            {
                boostSprites.SetActive(true);
                rb.velocity = new Vector2(rb.velocity.x , jumpPower * 1.1f);
                afterburnerApex = transform.position.y;
            }
            remainingJumps --;
            jumpGateTimer = 0f;
            jumpGate = true;
            justJumped = true;
        }
    }


    void Update()
    {
        if(dying || paused)
        {
            return;
        }

        if(rb.velocity.y < 0f && cameraAdjuster == true)
        {
            cameraScript.SpeedUp();
            cameraAdjuster = false;
        }
        else if(cameraAdjuster == true && isGrounded == true)
        {
            cameraScript.RestoreSpeed();
            cameraAdjuster = false;
        }

        if(rb.velocity.y > 1f || (rawMovementInput.y < 0f && isGrounded == true) && !lockController)
        {
            cameraAdjuster = true;
        }

        boxDetectorCentre = new Vector2(boxDetectorOffsetX * facingDirection + transform.position.x , boxDetectorOffsetY + transform.position.y);

        raycastPos = transform.position; // this can be altered later if you would like it to change

        if(shield && isDeflecting)
        {
            LaserCaster();
        }

        if(climbing == true)
        {
            scalingWall = true;
            climbingDismountTimer = 0f;
            wasClimbing = true;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX;
            rb.constraints = RigidbodyConstraints2D.FreezePositionY;

            if(rawMovementInput.y == 0f && !InputManager.ButtonB() && wallSliding == false)
            {
                rb.constraints = RigidbodyConstraints2D.FreezePositionY;
                transform.rotation = Quaternion.identity;
            }
            else if(!lockController)
            {
                //rb.constraints = RigidbodyConstraints2D.FreezePositionY;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                if(!spaceForJump) rb.velocity = new Vector2(rb.position.x , movementInput.y * climbSpeed);
                else rb.velocity = new Vector2(rb.position.x , climbSpeed);
                transform.rotation = Quaternion.identity;
            }

            if(wallSliding == true) //maybe differentiate bools for ground and side collisions
            {
                if(rb.velocity.y < -wallSlideSpeedMax)
                {
                    rb.velocity = new Vector2(rb.velocity.x , -wallSlideSpeedMax);
                }
            }

            if((climbDirection > 0 && InputManager.JoystickLeftHorizontal() < 0f) || (climbDirection < 0 && InputManager.JoystickLeftHorizontal() > 0f)) // if player leaves the ladder
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

        if(jumpGate == true) // stops repeat jumping
        {
            jumpGateTimer += Time.deltaTime;
            if(jumpGateTimer > jumpGateDuration)
            {
                if(remainingJumps == maximumJumps)
                {
                    remainingJumps --;
                }
                jumpGate = false;
            }
        }

// JUMP TUNING --------------------------------------------------------------------------------------------

        if(transform.position.y <= (maxHeight - groundbreakerDistance))
        {
            if(groundbreaker && !groundBreakerReset)
            {
                timeSlowScript.TimeSlow();
                groundBreakerReset = true;
            }
        }

        afterburnerGlide = false;
        if(afterburner && remainingJumps <= 0)
        {
            if(transform.position.y > afterburnerApex) afterburnerApex = transform.position.y; // set the afterburner apex to be peak of the second jump (equal to height until the player starts descending)
            if(transform.position.y < afterburnerApex) afterburnerGlide = true; // if desecending, turn glide on
        }

        if(rb.velocity.y < 0f && afterburner == true && (InputManager.JoystickLeftVerticalRaw() > 0 || InputManager.Jump() || InputManager.ButtonB() && !lockController && !isSwinging && afterburnerGlide)) // afterburner glide
        {
            rb.gravityScale = 1f;
            rb.velocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.deltaTime * 0.0005f;

            if(boostSprites != null && climbing == false)
            {
                boostSprites.SetActive(true);
            }
        }
        else if(rb.velocity.y < 0f && !wallSliding && !isSwinging) // fast fall for impactful jumping... not great for the knees though (gravity inputs a negative value)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.deltaTime;
            if(boostSprites != null)
            {
                boostSprites.SetActive(false);
            }
        }
        else if (rb.velocity.y > 0f && rawMovementInput.y <= 0 && !InputManager.Jump() && !InputManager.ButtonB() && !lockController && !isSwinging && fastFall) // reduces jump height when button isn't held (gravity inputs a negative value)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (unheldJumpReduction - 1) * Time.deltaTime;
            if(boostSprites != null)
            {
                boostSprites.SetActive(false);
            }       
        }

        BoxInteract(); // check for box pickup or drop prompts

        //ANIM SYNC - use holding boolean
        anims.Hold(holding);

        DetachPart(); // detach part on "space" press

        DeployShield();

        if(!shield || !shieldActive)
        {
            EndDeflect(); // create the shield bubble around the player
        }
    }

    bool GroundCheck()
    {
        jumpDisableTimer += Time.fixedDeltaTime; // jumpDisableTimer is applied by walljumping, preventing the grounded state from triggering interfering jumps for 0.2 seconds

        LayerMask groundLayer = (partConfiguration == 1)? jumpLayer : nonHeadJumpLayer;

        if(jumpBan) // jump ban is applied by the elevator script to lock the player to it when slamming upwards
        {
            return false;
        }

        RaycastHit2D hitC = Physics2D.Raycast(new Vector2(raycastPos.x, raycastPos.y + raycastYOffset), Vector2.down, groundedDistance, groundLayer);
        Debug.DrawRay(new Vector2(raycastPos.x, raycastPos.y + raycastYOffset), Vector2.down * groundedDistance, Color.green);
        wallSliding = false;
        wallHang = false;
        scalingWall = false;
        scalerTrueGrounded = false;

        if(partConfiguration == 1)
        {
            climbing = false;

            if(hitC.collider != null) // central collision raycast
            {
                if((hitC.collider.gameObject.tag == "Legs" || hitC.collider.gameObject.tag == "Arms" || hitC.collider.gameObject.tag == "Parts") && (transform.position.y > (0.1f + lastGroundedHeight) || (transform.position.y < (lastGroundedHeight - 0.08))))
                {
                    return false;
                }
                else
                {
                    scalerTrueGrounded = true;
                    return true;
                }
            }
            else if(scaler)
            {
                RaycastHit2D upHit = Physics2D.Raycast(transform.position , Vector2.up , 0.6f , groundLayer);
                Debug.DrawRay(transform.position, Vector2.up * 0.6f, Color.green);
                if(upHit.collider != null )
                {
                    ceilingAbove = true;
                    forceSlaved = false;
                    jumpDisableTimer = 1f;
                }
                else
                {
                    ceilingAbove = false;
                }

                RaycastHit2D sideHit = Physics2D.Raycast(transform.position , Vector2.right * previousFacingDirection , 0.6f , groundLayer);
                Debug.DrawRay(transform.position, Vector2.right * 0.6f, Color.green);
                if(sideHit.collider != null )
                {
                    scalingWall = true;
                    climbDirection = previousFacingDirection;
                    float wallAngle = Vector2.Angle(sideHit.normal, Vector2.up);
                    if(wallAngle > 80f)
                    {
                        wallHang = true;
                    }
                    return true;
                }

                if(!forceSlaved)
                {
                    if(Physics2D.OverlapCircle(gameObject.transform.position, 0.4f , groundLayer))
                    {
                        if(hitC.collider != null && (hitC.collider.gameObject.tag == "Legs" || hitC.collider.gameObject.tag == "Arms"))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
        }
        else if(partConfiguration == 2 || partConfiguration == 4) // for climbing ladders
        {
            if(inputX > 0 || (facingDirection == 1 && inputX == 0))
            {
                Vector2 raycastAltPosR = new Vector2(raycastPos.x , raycastPos.y - 0.75f);
                RaycastHit2D sideHitR = Physics2D.Raycast(raycastAltPosR , Vector2.right, 0.6f , ladderLayer);
                Debug.DrawRay(raycastAltPosR, Vector2.right * 0.6f, Color.green);
                if(sideHitR.collider != null && sideHitR.collider.gameObject.tag == "Climbable")
                {
                    climbing = true;
                    climbDirection = 1;
                    return true;
                }
                climbing = false;
                if(sideHitR.collider != null && sideHitR.collider.gameObject.tag == "WallJump")
                {
                    wallSliding = true;
                    return true;
                }
            } 
            else if(inputX < 0 || (facingDirection == 1 && inputX == 0))
            {
                Vector2 raycastAltPosL = new Vector2(raycastPos.x , raycastPos.y - 0.75f);
                RaycastHit2D sideHitL = Physics2D.Raycast(raycastAltPosL , Vector2.left, 0.6f , ladderLayer);
                Debug.DrawRay(raycastAltPosL, Vector2.left * 0.6f, Color.green);
                if(sideHitL.collider != null && sideHitL.collider.gameObject.tag == "Climbable")
                {
                    climbing = true;
                    climbDirection = -1;
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
            RaycastHit2D hitL = Physics2D.Raycast(new Vector2(raycastPos.x - raycastXOffset , raycastPos.y + raycastYOffset), Vector2.down, groundedDistance, groundLayer);
            RaycastHit2D hitR = Physics2D.Raycast(new Vector2(raycastPos.x + raycastXOffset , raycastPos.y + raycastYOffset), Vector2.down, groundedDistance, groundLayer);
            Debug.DrawRay(new Vector2(raycastPos.x - raycastXOffset, raycastPos.y + raycastYOffset), Vector2.down * groundedDistance, Color.green);
            Debug.DrawRay(new Vector2(raycastPos.x + raycastXOffset, raycastPos.y + raycastYOffset), Vector2.down * groundedDistance, Color.green);

            if(groundbreaker == true && transform.position.y <= (maxHeight - groundbreakerDistance) && hitC.collider != null)
            {
                if(hitC.collider.gameObject.tag == "Groundbreakable")
                {
                    if(hitC.collider.gameObject.GetComponent<groundbreakable>() != null) hitC.collider.gameObject.GetComponent<groundbreakable>().Groundbreak();
                    else hitC.collider.gameObject.GetComponent<destructiblePlatform>().Groundbreak(hitC.point);
                    return false;
                }
                /*
                else if(hitC.collider.tag == "Gyre")
                {
                    Debug.Log("Hit");
                    hitC.collider.gameObject.GetComponent<Gyre_WeakSpot>().Destruct();
                    return false;
                }
                */
                else
                {
                    groundBreakerReset = false;
                    return true;
                }
            }
            // the following ensure that the player is not grounded when colliding with attachable parts, necessary for the part attacher script
            else if(hitC.collider != null)
            {
                if(hitC.collider.gameObject.tag == "Legs" && (partConfiguration == 1 || partConfiguration == 2) && (transform.position.y > (0.1f + lastGroundedHeight) || (transform.position.y < (lastGroundedHeight - 0.08f))))
                {
                    groundBreakerReset = false;
                    return false;
                }
                else if(hitC.collider.gameObject.tag == "Arms" && (partConfiguration == 1 || partConfiguration == 3) && (transform.position.y > (0.1f + lastGroundedHeight) || (transform.position.y < (lastGroundedHeight - 0.08f))))
                {
                    groundBreakerReset = false;
                    return false;
                }
                    groundBreakerReset = false;
                return true; // if not a part, or a non-attachable part, then act as though it is normal ground
            }
            else if(hitL.collider != null)
            {
                if(hitL.collider.gameObject.tag == "Legs" && (partConfiguration == 1 || partConfiguration == 2) && (transform.position.y > (0.1f + lastGroundedHeight) || (transform.position.y < (lastGroundedHeight - 0.08f))))
                {
                    groundBreakerReset = false;
                    return false;
                }
                else if(hitL.collider.gameObject.tag == "Arms" && (partConfiguration == 1 || partConfiguration == 3) && (transform.position.y > (0.1f + lastGroundedHeight) || (transform.position.y < (lastGroundedHeight - 0.08f))))
                {
                    groundBreakerReset = false;
                    return false;
                }
                groundBreakerReset = false;
                return true; // if not a part, or a non-attachable part, then act as though it is normal ground
            }
            else if(hitR.collider != null)
            {
                if(hitR.collider.gameObject.tag == "Legs" && (partConfiguration == 1 || partConfiguration == 2) && (transform.position.y > (0.1f + lastGroundedHeight) || (transform.position.y < (lastGroundedHeight - 0.08f))))
                {
                    groundBreakerReset = false;
                    return false;
                }
                else if(hitR.collider.gameObject.tag == "Arms" && (partConfiguration == 1 || partConfiguration == 3) && (transform.position.y > (0.1f + lastGroundedHeight) || (transform.position.y < (lastGroundedHeight - 0.08f))))
                {
                    groundBreakerReset = false;
                    return false;
                }
                 groundBreakerReset = false;
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
            if(hitCollider[i].gameObject.tag == "HeavyLiftable" || hitCollider[i].gameObject.tag == "Box" || hitCollider[i].gameObject.tag == "powerCell")
            {
                if(!(hitCollider[i].gameObject.tag == "powerCell" && hitCollider[i].GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Static)) // if not a locked powercell
                {
                    distanceFromPickupPoint = Vector2.Distance(hitCollider[i].gameObject.transform.position , boxHoldPos.position);
                    if(distanceFromPickupPoint < previousDistanceFromPickupPoint || previousDistanceFromPickupPoint == 0)
                    {
                        previousDistanceFromPickupPoint = distanceFromPickupPoint;
                        closestBox = hitCollider[i].gameObject;
                    }
                }
            }
            i++;
        }
    }

    void BoxInteract()
    {
        if(InputManager.ButtonXDown() && (partConfiguration == 2 || partConfiguration == 4) && holding == false) // pick up and drop box while you have arms and press "f"
        {
            BoxAssigner(); // sort through each available box, and find the one closest to the pickup point
            {
                if(closestBox != null)
                {
                    if(((closestBox.tag != "HeavyLiftable") && (closestBox.tag != "powerCell")) || lifter)
                    {
                        playerSound.PickUpPlay();
                        closestBox.transform.parent = this.transform;
                        closestBox.transform.position = boxHoldPos.position;
                        closestBox.GetComponent<Rigidbody2D>().isKinematic = true;
                        closestBox.GetComponent<Collider2D>().enabled = false;
                        closestBox.transform.rotation = Quaternion.identity;
                        holding = true;
                        if(closestBox.tag == "powerCell")
                        {
                            heldPowerCellCol.enabled = true;
                            heldBox = closestBox.gameObject;
                            heldBoxTag = closestBox.tag;
                        }
                        else
                        {
                            heldBoxCol.enabled = true;
                            heldBox = closestBox.gameObject;
                            heldBoxTag = closestBox.tag;
                        }
                    }
                }
            }
        } 
        else if(InputManager.ButtonXDown() && holding == true) // while the player is holding a box, and presses the f key, drop the box
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
            heldPowerCellCol.enabled = false;
            closestBox.GetComponent<Rigidbody2D>().isKinematic = false;
            closestBox.GetComponent<Collider2D>().enabled = true;
            holding = false;
            closestBox = null;
            heldBox = null;
            heldBoxTag = null;
        } 
    }

    void DetachPart()
    {
        if (InputManager.Detach() && partConfiguration != 1) // eventually set this to create prefabs of the part, rather than a detached piece
        {
            jumpDisableTimer = 0f;
            float velX = rb.velocity.x;

            if(partConfiguration > 2)
            {
                legs.GetComponent<Legs>().Detached(maxHeight , groundbreakerDistance);
                remainingJumps --; // consumes jumps but doesn't require them to be used
                UpdateParts();
                rb.velocity = new Vector2(Mathf.Clamp(velX / 2 , - movementSpeed , movementSpeed), jumpForce * 8f);
            }
            else if(partConfiguration == 2)
            {
                arms.GetComponent<Arms>().Detached();
                remainingJumps --; // consumes jumps but doesn't require them to be used
                UpdateParts();
                rb.velocity = new Vector2(Mathf.Clamp(velX / 2 , - movementSpeed , movementSpeed), jumpForce * 8f);
            }

            forceSlaved = true;
        }
    }

    public void DeployShield()
    {
        if(shield && InputManager.Cast() && !shieldBubble.activeSelf)
        {
            shieldBubble.SetActive(true);
            shieldActive = true;
            playerSound.ShieldPlay();
        }
        else if((InputManager.Cast() && shieldBubble.activeSelf))
        {
            DeactivateShield();
        }
    }

    void DeactivateShield()
    {
        shieldActive = false;
        shieldBubble.SetActive(false);
        playerSound.ShieldStop();
    }

    public void InitiateDeflect()
    {
        if(shield && shieldActive)
        {
            isDeflecting = true;
            laserLine.enabled = true;
            laserLine.material = laserMaterialAim;
            if(isDeflecting) collisionEffect.SetActive(true);
        }
    }

    public void DeathRay(bool firing)
    {
        firingLaser = firing;
        if(isDeflecting) collisionEffect.SetActive(true);
        burstEffect.SetActive(true);

        if(firingLaser)
        {
            laserLine.material = laserMaterialFire;
            burstColour.ColourChange("red");
            collisionColour.ColourChange("red");
        }
        else
        {
            laserLine.material = laserMaterialAim;
            burstColour.ColourChange("blue");
            collisionColour.ColourChange("blue");
        }
    }

    public void EndDeflect()
    {
        isDeflecting = false;
        laserLine.enabled = false;
        collisionEffect.SetActive(false);
        burstEffect.SetActive(false);
        burstColour.ColourChange("none");
        collisionColour.ColourChange("none");
        laserLine.material = laserMaterialAim;
    }

    public void LaserCaster()
    {
        Vector2 mousePos = (Vector2)Input.mousePosition;
        if(previousMousePos != mousePos)
        {
            mouseMoved = true;
        }
        else mouseMoved = false;

        previousMousePos = mousePos;


        Vector3 mouseDirection = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x , Input.mousePosition.y , gameObject.transform.position.z)) - transform.position;
        Vector2 laserOriginDirection = new Vector2(mouseDirection.x , mouseDirection.y);


        if(mouseMoved != true)
        {
            if(InputManager.JoystickRight() != Vector2.zero) laserOriginDirection = InputManager.JoystickRight();
        }
        else
        {
            InputManager.previousJoystickRightHorizontal = 0f;
            InputManager.previousJoystickRightVertical = 0f;
        }



        laserOriginDirection.Normalize();
        Vector2 laserOrigin = (Vector2)transform.position + (laserOriginDirection * shieldRadius);

        RaycastHit2D laser = Physics2D.Raycast(laserOrigin, laserOriginDirection, Mathf.Infinity , laserLayer);
        if(laser.collider != null)
        {
            laserEndpoint = laser.point;
            if(isDeflecting) collisionEffect.SetActive(true);

            Vector2 laserCollisionNormal = laser.normal;
            float collisionNormalAngle = Mathf.Atan2(laserCollisionNormal.y , laserCollisionNormal.x);
            if(collisionNormalAngle < 0f)
            {
                collisionNormalAngle = Mathf.PI * 2 + collisionNormalAngle;
            }

            collisionEffect.transform.position = laser.point;
            collisionEffect.transform.up = Quaternion.Euler(0 , 0 , (collisionNormalAngle * Mathf.Rad2Deg)) * Vector2.right;

            if(laser.collider.tag == "LaserRouter")
            {
                if(laserTag != "laserRouter")
                {
                    laserRouter = laser.transform.gameObject.GetComponent<laserRouter>();
                    if(firingLaser)
                    {
                        laserRouter.DeathRay(true);
                        laserRouter.Charged();
                    }
                    else
                    {
                        laserRouter.DeathRay(false);
                    }
                }
            }
            else if(laser.collider.tag == "powerCell")
            {
                if((laserTag != "powerCell" && firingLaser) || (!wasFiringLaser && firingLaser))
                {
                    powerCell = laser.transform.gameObject.GetComponent<powerCell>();
                    powerCell.activated = true;
                }
            }
            else if(laser.collider.tag == "PowerStation")
            {
                if((laserTag != "PowerStation" && firingLaser) || (!wasFiringLaser && firingLaser))
                {
                    powerStation = laser.transform.gameObject.GetComponent<powerStation>();
                    powerStation.activated = true;
                }
            }
            else
            {
                if(laserRouter != null && laserTag == "laserRouter")
                {
                    laserRouter.Drained();
                    laserRouter.DeathRay(false);
                }
            }

            if(firingLaser)
            {
                if(laser.collider.tag == "Enemy" || laser.collider.tag == "Groundbreakable")
                {
                    Destroy(laser.collider.gameObject);
                }
            }
            
            laserTag = laser.collider.tag;
        }
        else
        {
            collisionEffect.SetActive(false);
            laserEndpoint = laserOrigin + (laserOriginDirection * 1000f);
        }
        laserLine.positionCount = 2;
        laserLine.SetPosition(0 , laserOrigin);
        laserLine.SetPosition(1 , laserEndpoint);

        float laserAngle = Mathf.Atan2(laserOriginDirection.y , laserOriginDirection.x); // apply the laser burst effect
        if(laserAngle < 0f)
        {
            laserAngle = Mathf.PI * 2 + laserAngle;
        }
            
        burstEffect.SetActive(true);
        burstEffect.transform.position = (Vector2)transform.position + (laserOriginDirection * (shieldRadius - 0.01f));
        burstEffect.transform.up = Quaternion.Euler(0 , 0 , (laserAngle * Mathf.Rad2Deg)) * Vector2.right;

        if(firingLaser != wasFiringLaser)
        {
            wasFiringLaser = firingLaser;
        }
    }




    //  _______________________________________________________________________  //  ____________________________________________________________________  //





    public void UpdateParts() // increase raycastYOffset, decrease groundcheckerDistance
    // call when acquiring or detaching part - reconfigures scaling, controls and colliders - 1 is head, 2 adds torso, 3 adds legs, 4 adds torso and legs
    {
        // assume that the robot has neither arms or legs, then check for them
        bool hasArms = false;
        bool hasLegs = false;

        AkSoundEngine.PostEvent("ExitFan" , gameObject);

        rb.gravityScale = 2f;
        for(int i = 0; i < transform.childCount; i++) // loop through children to find if it has arms or legs
        {
            if(transform.GetChild(i).tag == "Arms")
            {
                hasArms = true;
                arms = transform.GetChild(i).gameObject;
                armString = arms.name;
                armIdentifier = arms.GetComponent<Arms>().instance;
            }
            else if (transform.GetChild(i).tag == "Legs")
            {
                hasLegs = true;
                legs = transform.GetChild(i).gameObject;
                legString = legs.name;
                legIdentifier = legs.GetComponent<Legs>().instance;
            }
        }

        previousPartConfiguration = partConfiguration;

        Vector2 snapOffsetPos = gameObject.transform.position; // changing offset to cater for original sprites provided - may need to be re-scaled later

       

        if(!hasArms && !hasLegs)
        {
            if(previousPartConfiguration == 2)
            {
                playerSound.AttachPlay();
                transform.position = new Vector2 (snapOffsetPos.x , snapOffsetPos.y + 0.55f); // head snaps up
            }
            else if(previousPartConfiguration > 2)
            {
                playerSound.DetachPlay();
            }

            rb.sharedMaterial = frictionMaterial;
            partConfiguration = 1; // just a head
            movementSpeed = augmentedMovementSpeed * 5f;
            jumpPower = jumpForce * 6f;
            jumpGateDuration = 0.2f;

            // upgrades
            maximumJumps = 1;
            groundbreaker = false;
            afterburner = false;
            lifter = false;
            shield = false;
            DeactivateShield();
            if(boostSprites != null)
            {
                boostSprites.SetActive(false);
                boostSprites = null;
            }

            if(headString == "ScalerHead" || scaler) // if the player already has a head augmenr, they keep it when they get a new one
            {
                scaler = true;
                jumpPower = jumpForce * 6f;
                fallMultiplier = 1f;
            }
            else
            {
                scaler = false;
                jumpPower = jumpForce * 7f;
            }

            if(headString == "HookshotHead" || hookShot) // if the player already has a head augment, they keep it when they get a new one
            {
                hookShot = true;
                hookshotAugment.SetActive(true);
                hookshotScript.enabled = true;
                fallMultiplier = 1f;
                // do stuff here
            }
            else
            {
                hookShot = false;
                hookshotAugment.SetActive(false);
                hookshotAnchor.SetActive(false);
                hookshotScript.enabled = false;
                // cancel stuff here
            }

            if(boxCol != null)
            {
                Destroy(boxCol);
            }
            //boxCol.enabled = false; // don't use the typical vertical standing collider
            if(headCol != null && headCol.enabled != true)
            {
                headCol.enabled = false;
                head.SetActive(false);
                head.SetActive(true);
                headCol.enabled = true;
            }
            rb.constraints = RigidbodyConstraints2D.None; // can roll
            head.transform.position = gameObject.transform.position; // no need for snapOffsetPos here as it is perfectly centred
            legString = "None"; // no legs
            armString = "None"; // no arms
            raycastXOffset = 0.1f;
            raycastYOffset = 0f;
            groundedDistance = 0.33f;

            BoxDrop(); // drops any box immediately
            scalerAugment.transform.localScale = new Vector3(0.35f, 0.35f, 1f); // set the Scaler star/spikes to maximum size
        }
        else if (hasArms && !hasLegs)
        {
            if(previousPartConfiguration > 2)
            {
                AkSoundEngine.PostEvent("DetachLegs" , gameObject);
                playerSound.DetachPlay();
            }
            else if(previousPartConfiguration < 2)
            {
                playerSound.AttachPlay();
            }

            partConfiguration = 2; // just a head and arms
            movementSpeed = augmentedMovementSpeed * 7.5f;
            jumpPower = jumpForce * 8.5f;

            NonHeadConfig();
            fallMultiplier = 3f;
            if(armString == "LifterArms" || armString == "ShieldArms")
            {
                if(armString == "LifterArms")
                {
                    armConfiguration = 2;
                    lifter = true;
                }
                else
                {
                    lifter = false;
                }

                if(armString == "ShieldArms")
                {
                    armConfiguration = 3;
                    shield = true;
                }
                else
                {
                    DeactivateShield();
                }
            }
            else
            {
                armConfiguration = 1;
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
            raycastYOffset = -0.78f;
            shieldBubble.transform.localPosition = Vector3.zero;
            boxCol.size = new Vector2(0.6f , 1.2f);
            boxCol.offset = new Vector2(0f , -0.27f);

        //    boxCol.size = new Vector2(0.6f , 1.6f);
        //    boxCol.offset = new Vector2(0f , 0.03f);
            
            if(holding == true) // keep holding the box if you were
            {
                if(closestBox.tag == "powerCell")
                {
                    heldPowerCellCol.enabled = true;
                }
                else
                {
                    heldBoxCol.enabled = true;
                }
            }
            else
            {
                heldBoxCol.enabled = false;
                heldPowerCellCol.enabled = false;
            }
        }


        else if (!hasArms && hasLegs) // need to change collider
        {
            if(previousPartConfiguration == 1)
            {
                playerSound.AttachPlay();
            }
            else
            {
                playerSound.DetachPlay();
            }

            partConfiguration = 3;
            movementSpeed = augmentedMovementSpeed * 8.5f;
            jumpPower = jumpForce * 11f;
            
            NonHeadConfig();
            armString = "None"; // no arms
            lifter = false;
            shield = false;
            DeactivateShield();

            if(legString == "AfterburnerLegs" || legString == "GroundbreakerLegs")
            {
                if(legString == "AfterburnerLegs")
                {
                    legConfiguration = 3;
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
                    legConfiguration = 2;
                    groundbreaker = true;
                }
                else
                {
                    groundbreaker = false;
                }
            }
            else
            {
                legConfiguration = 1;
            }

            head.transform.position = new Vector2 (snapOffsetPos.x , snapOffsetPos.y + 0.155f); // head snaps up... legs stay where they are
            raycastYOffset = -0.95f;


            boxCol.size = new Vector2(0.6f , 1.79f);
            boxCol.offset = new Vector2(0f , -0.16f);

        //    boxCol.size = new Vector2(0.6f , 1.45f);
        //    boxCol.offset = new Vector2(0f , -0.27f);
            BoxDrop(); // no arms, so drop the box
        }


        else if(hasArms && hasLegs) // need to change collider
        {
            playerSound.AttachPlay(); // this is always going to come out of a part being attached

            partConfiguration = 4; // has all parts
            movementSpeed = augmentedMovementSpeed * 8.5f;
            jumpPower = jumpForce * 11f;

            NonHeadConfig();
            if(armString == "LifterArms" || armString == "ShieldArms")
            {
                if(armString == "LifterArms")
                {
                    armConfiguration = 2;
                    lifter = true;
                }
                else
                {
                    lifter = false;
                }

                if(armString == "ShieldArms")
                {
                    armConfiguration = 3;
                    shield = true;
                }
                else
                {
                    DeactivateShield();
                }
            }
            else
            {
                armConfiguration = 1;
            }

            if(legString == "AfterburnerLegs" || legString == "GroundbreakerLegs")
            {
                if(legString == "AfterburnerLegs")
                {
                    legConfiguration = 3;
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
                    legConfiguration = 2;
                    groundbreaker = true;
                }
                else
                {
                    groundbreaker = false;
                }
            }
            else
            {
                legConfiguration = 1;
            }

            head.transform.position = new Vector2(snapOffsetPos.x , snapOffsetPos.y + 0.755f); // head snaps up
            arms.transform.position = new Vector2(snapOffsetPos.x , snapOffsetPos.y); // arms share the complete character's origin
            raycastYOffset = -0.95f;
            shieldBubble.transform.localPosition = new Vector3(0f , 0.135f, 0f);
            boxCol.size = new Vector2(0.6f , 2.37f);
            boxCol.offset = new Vector2(0f , 0.14f);
        //    boxCol.size = new Vector2(0.6f , 2.08f);
        //   boxCol.offset = new Vector2(0f , 0.03f);

            if(holding == true) // keep holding the box if you were
            {
                if(closestBox.tag == "powerCell")
                {
                    heldPowerCellCol.enabled = true;
                }
                else
                {
                    heldBoxCol.enabled = true;
                }
            }
            else
            {
                heldBoxCol.enabled = false;
                heldPowerCellCol.enabled = false;
            }
        }

// Manage Head Augments Here

        if(headString == "ScalerHead" || scaler) // changes whether the Scaler Augment is visible or not - no mechanical difference
        {
            scalerAugment.SetActive(true);
            headConfiguration = 2;
        }

        if(headString == "HookshotHead" || hookShot)
        {
            hookShot = true;
            hookshotAugment.SetActive(true);
            headConfiguration = 3;
        }
        
        if(headString == "BasicHead" && !scaler && !hookShot)
        {
            scaler = false;
            scalerAugment.SetActive(false);
            hookShot = false;
            hookshotAugment.SetActive(false);
            hookshotScript.enabled = false;
            hookshotAnchor.SetActive(false);
            headConfiguration = 1;
        }

        if(scaler && hookShot)
        {
            headString = "FullHead";
            headConfiguration = 4;
        }

        // ANIMS: Set animation states
        if (anims != null)
        {
            anims.SetConfigurations(legConfiguration, armConfiguration, headConfiguration);
            //Update the animation state with arms/legs
            anims.SetParts(hasLegs ? legs : null, hasArms ? arms : null, head);
        }
        camera.GetComponent<Camera2DFollow>().Resize(partConfiguration , cameraScript.standardResizeDuration , 1f);
    }

    void NonHeadConfig()
    {
        headCol.enabled = false; // disable the rolling head collider
        if(boxCol != null)
        {
            boxCol.enabled = true; // use the capsule collider instead
        }
        else
        {
            boxCol = gameObject.AddComponent<BoxCollider2D>();
        }
        rb.constraints = RigidbodyConstraints2D.FreezePositionY; // freeze movement temporarily
        rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        rb.sharedMaterial = slipperyMaterial;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // can no longer roll
        scalerAugment.transform.localScale = new Vector3(0.25f, 0.25f, 1f); // shrink the scaler star to signify it is no longer usable
        transform.rotation = Quaternion.identity; // lock rotation to 0;
        hookshotScript.enabled = false;
        hookshotAnchor.SetActive(false);
        isGrounded = false;
        fallMultiplier = 4f;
        rb.velocity = Vector2.zero;
        inputX = 0f;
        jumpGate = true;
        jumpGateDuration = 0.6f;
        jumpGateTimer = jumpGateDuration - 0.12f;
        raycastXOffset = 0.27f;
        groundedDistance = 0.15f;
        maxHeight = transform.position.y;
        isSwinging = false;
        hookshotScript.enabled = false;
        if(facingDirection == 1)
        {
            hookshotAugment.transform.eulerAngles = new Vector3(0f, 0f , 45f);
        }
        else
        {
            hookshotAugment.transform.eulerAngles = new Vector3(0f, 0f, -45f);
        }
    }

    public void Die(float time) // this should eventually be moved to the scene manager
    {
        if(!dying)
        {
            AkSoundEngine.PostEvent("PlayerDeath" , gameObject);
            deathTimer = time;
            isGrounded = false;
            cameraScript.TriggerShake(15f , true , 4);
            dying = true;
            if(partConfiguration == 1 && hookshotScript.enabled)
            {
                hookshotScript.DetachRope();
            }
        }
    }
}