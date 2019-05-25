/*

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    public int partConfiguration = 1; // 1 is head, 2 adds arms, 3 adds legs, 4 adds arms and legs
    public float movementSpeedAdjuster = 10f;
    float movementSpeed; // how fast can you move?
    float speed; // current speed
    public float jumpForceAdjuster = 10f; // control the level of jumps
    private float jumpForce; // how powerful is your jump? altered from the jumpForceAdjuster by different part combinations
    bool jumpGate; // prevent the character from jumping while this is true (set to disable corner jumps eventually)
    private float moveInput; // get player Input value
    private bool facingRight = true; // used for flipping the character visuals (and arm interaction area)
    public GameObject pickupBox; // the box that the player is currently picking up
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
    bool wasBoxHeld; // checks if the player was holding a box before updating parts, so that they continue to hold if they retain their arms
    public bool isGrounded; //is the character on the ground?
    public GameObject groundChecker; // the ground checker object (used for the Scaler Augment)
    public Transform groundCheck; // transform of the ground checker object (used for the Scaler Augment)
    public LayerMask JumpLayer1; // what can the player jump on?
    public LayerMask JumpLayer2; // had 2 layers and combined them, because the Unity editor wasn't allowing multiple selections at an earlier dev stage
    private LayerMask canJumpOn; // combines the 2 layers that can be jumped on
    public float groundedDistance = 0.3f; // distance of the grounded raycast
    public GameObject scalerStar; // the sprite for the Scaler Augment - starts disabled
    public Pass_Through_Platform_Script passThroughScript;

    private int extraJumps; // how many jumps beyond 1 does the player currently have?
    public int numberOfJumps = 1; // how many jumps does the player have?

    public float fallMultiplier = 2.5f; // increase fall speed on downwards portion of jump arc
    public float lowJumpMultiplier = 2f; // alters jump height based on duration of the jump button being held

    public string armString; // will be used later to recall arm loadout - will be using later to instantiate prefabs for checkpoints
    public string legString; // will be used later to recall leg loadout - will be using later to instantiate prefabs for checkpoints
    public string headString; // will be used later to recall head loadout - will be using later to instantiate prefabs for checkpoints
    public bool frozen; // use this when connecting to stop input


    void Start()
    {
        groundChecker = this.transform.Find("Ground Checker").gameObject;
        rb = GetComponent<Rigidbody2D>();
        capCol = GetComponent<CapsuleCollider2D>();
        head = this.transform.Find("Head").gameObject;
        headCol = head.GetComponent<CircleCollider2D>();
        pickupBoxCol = GetComponent<BoxCollider2D>();
        extraJumps = numberOfJumps;
        canJumpOn = JumpLayer1 | JumpLayer2;
        jumpForce = jumpForceAdjuster;
        movementSpeed = movementSpeedAdjuster;
        UpdateParts();
        heldBoxCol.enabled = false;
        wasBoxHeld = false;
    }

    void FixedUpdate()
    {        
        if(!groundCheckRaycast()) // slow the player's horizontal movement in the air
        {
            speed = movementSpeed / 1.2f;
        }
        else
        {
            speed = movementSpeed;
        }

        moveInput = Input.GetAxisRaw("Horizontal"); // left or a is -1 , right or d is +1
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        if(headString == "Scaler" && partConfiguration == 1) // change ground collision detection if you have the Scaler augment
        {
            isGrounded = Physics2D.OverlapCircle(transform.position, 0.5f , canJumpOn); // returns true if circular ground checker overlaps a jumpable layer
        }
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
        if((facingRight == false && moveInput > 0) || (facingRight == true && moveInput < 0))
        {
            Flip(); // if changing directions, flip sprite
        }

        // Allow the below line of code to check the groundcheck distance
        Debug.DrawRay(transform.position, Vector2.down * groundedDistance, Color.green);

        if(TrueGroundCheck()) // || (headString == "Scaler" && isGrounded == true)) // used to be isGrounded == true
        {
            extraJumps = numberOfJumps;
        }

        if (Input.GetKeyDown("space") && partConfiguration != 1)// && extraJumps > 0)
        // eventually set this to create prefabs of the part, rather than a detached piece
        {
            if(partConfiguration > 2)
            {
                legs.transform.parent = null;
                legs.GetComponent<Legs>().Detached();
                extraJumps --;
                UpdateParts();
            }
            else if(partConfiguration == 2)
            {
                arms.transform.parent = null;
                arms.GetComponent<Arms>().Detached();
                extraJumps --;
                UpdateParts();
            }
        } 

        if(Input.GetButton("Jump") && extraJumps > 0 && jumpGate == false)
        {
            rb.velocity = Vector2.up * jumpForce;
            extraJumps --;
        }
        else if(Input.GetButton("Jump") && extraJumps == 0 && TrueGroundCheck() && jumpGate == false) //&& isGrounded == true)
        {
            //rb.velocity = Vector2.up * jumpForce;
            jumpGate = true;
        }

        if(Input.GetButtonUp("Jump"))
        {
            jumpGate = false;
        }

        if(rb.velocity.y < 0f) // fast fall for nice jumping
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0f && !Input.GetButton("Jump"))
        {
             rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;           
        }
    

        if(pickupBox != null)
        {
            if(Input.GetKeyDown("f") && (partConfiguration == 2 || partConfiguration == 4)) // pick up and drop box while you have arms and press "f"
            {
                if(holding == false && boxInRange == true)
                // currently the player character chooses from all boxes in range at random
                // this will need to be changed if they can have multiple boxes at a time
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

    void Flip() // reverses the character sprite when the player turns
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    bool groundCheckRaycast() // instantly returns "true" if raycast hits a layer in the canJumpOn layermask, also sets isGrounded to true
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        
        RaycastHit2D hit = Physics2D.Raycast(position, direction, groundedDistance, canJumpOn);
        if(hit.collider != null)
        {
            isGrounded = true;
            return true;
        }
        return false;
    }

    public bool TrueGroundCheck() // checks if either the raycast or the overlap circle is detecting ground
    {
        if(groundCheckRaycast() || isGrounded == true)
        {
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
            if(headString == "Scaler") // reduces jump power if you have the Scaler Augment as a trade-off
            {
                jumpForce = jumpForceAdjuster * 0.5f;
            }
            else
            {
                jumpForce = jumpForceAdjuster * 0.7f;
            }

            capCol.enabled = false; // don't use the typical vertical standing collider
            headCol.enabled = true; // use the circle collider instead
            rb.constraints = RigidbodyConstraints2D.None; // can roll
            head.transform.position = gameObject.transform.position; // no need for snapOffsetPos here as it is perfectly centred
            groundChecker.transform.position = gameObject.transform.position; // centre the groundChecker
            legString = "None"; // no legs
            armString = "None"; // no arms
            groundedDistance = 0.34f; // shorter ground check distance
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
            arms = gameObject.transform.Find("Arms").gameObject;
            legString = "None"; // no legs
           
            // adjust height of other parts
            head.transform.position = new Vector2 (snapOffsetPos.x , snapOffsetPos.y + 0.55f); // head snaps up
            arms.transform.position = new Vector2 (snapOffsetPos.x , snapOffsetPos.y - 0.255f); // arms snap down relative to the head, maintaining their original height
            groundChecker.transform.localPosition = new Vector2(0f, -0.76f);
            groundedDistance = 0.83f;

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
            legs = gameObject.transform.Find("Legs").gameObject;
            armString = "None"; // no arms

            head.transform.position = new Vector2 (snapOffsetPos.x , snapOffsetPos.y + 0.155f); // head snaps up... legs stay where they are
            groundChecker.transform.localPosition = new Vector2(0f, -0.97f);
            groundedDistance = 1.04f;

            capCol.size = new Vector2(0.6f , 1.45f);
            capCol.offset = new Vector2(0f , -0.27f);
            pickupBoxCol.enabled = true;
            BoxDrop(); // no arms, so drop the box
        }


        else if(hasArms && hasLegs) // need to change collider
        {
            partConfiguration = 4;
            movementSpeed = movementSpeedAdjuster * 0.85f;
            jumpForce = jumpForceAdjuster * 1.1f;

            NonHeadConfig();
            arms = gameObject.transform.Find("Arms").gameObject;
            legs = gameObject.transform.Find("Legs").gameObject;

            head.transform.position = new Vector2(snapOffsetPos.x , snapOffsetPos.y + 0.755f); // head snaps up
            arms.transform.position = new Vector2(snapOffsetPos.x , snapOffsetPos.y); // arms share the complete character's origin
            groundChecker.transform.localPosition = new Vector2(0f, -0.97f);
            groundedDistance = 1.07f;

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

        rb.velocity = Vector2.zero; // stops the player from connecting to a part and continuing to jump upwards
    }

    void NonHeadConfig() // Generic changes for non-head part updates (referenced in UpdateParts)
    {
        headCol.enabled = false; // disable the rolling head collider
        capCol.enabled = true; // use the capsule collider instead
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // can no longer roll
        scalerStar.transform.localScale = new Vector3(0.25f, 0.25f, 1f); // shrink the scaler star to signify it is no longer usable
        transform.rotation = Quaternion.identity; // lock rotation to 0;
    }
}
*/