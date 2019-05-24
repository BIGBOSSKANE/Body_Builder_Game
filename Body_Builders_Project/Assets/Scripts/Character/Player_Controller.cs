/*
Creator: Daniel
Created: 09/04/2019
Laste Edited by: Daniel
Last Edit: 16/05/2019
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    public int partConfiguration = 1; // 1 is head, 2 adds arms, 3 adds legs, 4 adds arms and legs
    public float movementSpeed = 10f;
    float speed; // current speed
    public float jumpForce = 10f;
    public float jumpsRemaining;
    bool jumpGate;
    private float moveInput;
    private bool facingRight = true; // used for flipping the character visuals (and arm interaction area)
    public GameObject pickupBox; // the box that the player is currently picking up
    public Transform boxHoldPos;
    public bool boxInRange = false;
    private Vector2 mousePos;
    public bool holding = false;
    public CircleCollider2D heldBoxCol;
    private Rigidbody2D rb;
    public CapsuleCollider2D capCol; // collider used and adjusted when player is more than a head
    public CircleCollider2D headCol; // collider used when the player is just a head
    public BoxCollider2D pickupBoxCol;
    public bool isGrounded; //is the character on the ground?
    public GameObject groundChecker; // the ground checker object (used for the Scaler Augment)
    public Transform groundCheck; // transform of the ground checker object (used for the Scaler Augment)
    public float checkRadius; // radius of the ground checker (for Scaler Augment)
    public LayerMask JumpLayer1;
    public LayerMask JumpLayer2;
    private LayerMask canJumpOn; // combines the 2 layers that can be jumped on
    public float groundedDistance = 0.3f; // distance of the grounded raycast
    public GameObject scalerStar;

    private int extraJumps;
    public int numberOfJumps = 1;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public string armString; // will be used later to recall arm loadout
    public string legString; // will be used later to recall leg loadout
    public string headString; // will be used later to recall head loadout


    void Start()
    {
        groundChecker = this.transform.Find("Ground Checker").gameObject;
        rb = GetComponent<Rigidbody2D>();
        capCol = GetComponent<CapsuleCollider2D>();
        headCol = this.transform.Find("Head").gameObject.GetComponent<CircleCollider2D>();
        pickupBoxCol = GetComponent<BoxCollider2D>();
        extraJumps = numberOfJumps;
        canJumpOn = JumpLayer1 | JumpLayer2;
        UpdateParts();
        heldBoxCol.enabled = false;
    }

    void FixedUpdate()
    {
        
        // the checkRadius is currently too large for the head - it sets up for spider climb though
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, canJumpOn);
        
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
        if((facingRight == false && moveInput > 0) || (facingRight == true && moveInput < 0))
        {
            Flip(); // if changing directions, flip sprite
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
        // Allow the below line of code to check the groundcheck distance
        Debug.DrawRay(transform.position, Vector2.down * groundedDistance, Color.green);

        if(groundCheckRaycast() || (headString == "Scaler" && isGrounded == true)) // used to be isGrounded == true
        {
            extraJumps = numberOfJumps;
        }

        if (Input.GetKeyDown("space") && partConfiguration != 1)// && extraJumps > 0)
        {
            if(partConfiguration > 2)
            {
                Legs legsScript = gameObject.GetComponentInChildren<Legs>();
                Transform legs = gameObject.transform.Find("Legs");
                legs.parent = null;
                legsScript.Detached();
                extraJumps --;
                UpdateParts();
            }
            else if(partConfiguration == 2)
            {
                Arms armsScript = gameObject.GetComponentInChildren<Arms>();
                Transform arms = gameObject.transform.Find("Arms");
                arms.parent = null;
                armsScript.Detached();
                extraJumps --;
                UpdateParts();
            }
        } 

        if(Input.GetButton("Jump") && extraJumps > 0 && jumpGate == false)
        {
            rb.velocity = Vector2.up * jumpForce;
            extraJumps --;
        }
        else if(Input.GetButton("Jump") && extraJumps == 0 && groundCheckRaycast() && jumpGate == false) //&& isGrounded == true)
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
            if(Input.GetKeyDown("f") && (partConfiguration == 2 || partConfiguration == 4)) // pick up and throw box
            {
                if(holding == false && boxInRange == true) // may potentially need to adjust collider to account for box
                {
                    pickupBox.transform.parent = this.transform;
                    BoxSnap();
                    holding = true;
                } 
                else
                {
                    BoxDrop();
                    /*
                    mousePos = Input.mousePosition;
                    var mouseDir = mousePos - pickupBox.transform.position;
                    mouseDir.Normalize();
                    */
                }
            }
        }

    }

    void BoxSnap() // when picking up the box
    {
        pickupBox.transform.position = boxHoldPos.position;
        pickupBox.GetComponent<Rigidbody2D>().isKinematic = true;
        pickupBox.GetComponent<Collider2D>().enabled = false;
        heldBoxCol.enabled = true;

// Offset x 0.48
// Size x 1.74
        //Adjust the player collider

// Standard Offset x 0.3
// Standard Size x 1.44

        //Destroy(pickupBox.GetComponent<Rigidbody2D>());
    }

    void BoxDrop()
    {
        if(pickupBox != null)
        {
            pickupBox.transform.parent = null;
            /*
            pickupBox.AddComponent<Rigidbody2D>();
            Rigidbody2D pickupRigi = pickupBox.GetComponent<Rigidbody2D>();
            pickupRigi.isKinematic = false;
            pickupRigi.mass = 4;
            pickupRigi.collisionDetectionMode.Continuous;
            */
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

    bool groundCheckRaycast()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        
        RaycastHit2D hit = Physics2D.Raycast(position, direction, groundedDistance, canJumpOn);
        if(hit.collider != null)
        {
            return true;
        }
        return false;
    }









    public void UpdateParts() // set the part configuration value - call when acquiring or detaching part
    // arms need to have "Arms" tag, and legs must have the "Legs" tag
    {
        /*
        Legs legs;
        legs = GameObject.GetComponent<Legs>(); // this is updated by the rocketlegs version of the ability
        legs.Special();
        */

        // assume that the robot has neither arms or legs before checking
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

        Vector2 thisPos = gameObject.transform.position;
        thisPos.y += 0.755f;
        // all other thisPos calculations need to take this into account, probably better to just change the head starting position
        Transform head = gameObject.transform.Find("Head");
        head.position = thisPos;

        if(!hasArms && !hasLegs)
        {
            partConfiguration = 1;
            pickupBoxCol.enabled = false;
            movementSpeed = 5f;
            if(headString == "Scaler")
            {
                //scalerStar.SetActive(true);
                jumpForce = 5f;
            }
            else
            {
                //scalerStar.SetActive(false);
                jumpForce = 7f;
            }
            capCol.enabled = false;
            headCol.enabled = true;
            checkRadius = 0.5f; // this check radius allows for the spider climb
            rb.constraints = RigidbodyConstraints2D.None;
            groundChecker.transform.localPosition = new Vector2(0f, 0.47f); // issue if player is rotating, the ground checker should shift to become the entire player rigidbody
            head.position = gameObject.transform.position;
            groundChecker.transform.position = gameObject.transform.position;
            legString = "None";
            armString = "None";
            groundedDistance = 0.34f;
            BoxDrop();
            scalerStar.transform.localScale = new Vector3(0.35f, 0.35f, 1f);
            heldBoxCol.enabled = false;
        }
        else if (hasArms && !hasLegs) // need to change collider
        {
            pickupBoxCol.enabled = true;
            Transform arms = gameObject.transform.Find("Arms");
            thisPos.y -= 0.24f;  
            head.position = thisPos;
            thisPos.y -= 0.76f;
            arms.position = thisPos; 
            partConfiguration = 2;
            movementSpeed = 7.5f;
            jumpForce = 8.5f;
            headCol.enabled = false;
            capCol.enabled = true;            
            capCol.size = new Vector2(0.6f , 1.6f);
            capCol.offset = new Vector2(0f , 0.03f);
            checkRadius = 0.25f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            groundChecker.transform.localPosition = new Vector2(0f, -0.76f);
            legString = "None";
            groundedDistance = 0.83f;
            scalerStar.transform.localScale = new Vector3(0.25f, 0.25f, 1f);
            if(holding == true)
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
            pickupBoxCol.enabled = false;
            partConfiguration = 3;
            movementSpeed = 8.5f;
            jumpForce = 11f;
            headCol.enabled = false;
            capCol.enabled = true;
            capCol.size = new Vector2(0.6f , 1.45f);
            capCol.offset = new Vector2(0f , -0.27f);
            checkRadius = 0.25f;
            thisPos.y -= 0.6f;
            head.position = thisPos;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            groundChecker.transform.localPosition = new Vector2(0f, -0.97f);
            armString = "None";
            groundedDistance = 1.04f;
            BoxDrop();
            scalerStar.transform.localScale = new Vector3(0.25f, 0.25f, 1f);
        }
        else if(hasArms && hasLegs) // need to change collider
        {
            pickupBoxCol.enabled = true;
            thisPos.y -= 0.755f;
            Transform arms = gameObject.transform.Find("Arms");
            arms.position = thisPos;
            partConfiguration = 4;
            movementSpeed = 8.5f;
            jumpForce = 11f;
            headCol.enabled = false;
            capCol.enabled = true;
            capCol.size = new Vector2(0.6f , 2.08f);
            capCol.offset = new Vector2(0f , 0.03f);
            checkRadius = 0.25f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            groundChecker.transform.localPosition = new Vector2(0f, -0.97f);
            groundedDistance = 1.07f;
            scalerStar.transform.localScale = new Vector3(0.25f, 0.25f, 1f);
            if(holding == true)
            {
                heldBoxCol.enabled = true;
            }
            else
            {
                heldBoxCol.enabled = false;
            }
        }
        if(headString == "Scaler")
        {
            scalerStar.SetActive(true);
        }
        else
        {
            scalerStar.SetActive(false);
        }
        // 1 is head, 2 adds torso, 3 adds legs, 4 adds torso and legs
    }
}
