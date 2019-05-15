
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    public int partConfiguration = 1; // 1 is head, 2 adds arms, 3 adds legs, 4 adds arms and legs
    public float movementSpeed = 10f;
    public float speed;
    public float jumpForce = 10f;
    bool jumpGate;
    private float moveInput;
    private bool facingRight = true;
    private Rigidbody2D rb;
    public CapsuleCollider2D capCol;
    public CircleCollider2D headCol;
    private bool isGrounded;
    public GameObject groundChecker;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;

    private int extraJumps;
    public int numberOfJumps = 1;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public Transform armPos;
    public Transform legPos;


    public string armString;
    public string legString;


    void Start()
    {
        groundChecker = this.transform.Find("Ground Checker").gameObject;
        rb = GetComponent<Rigidbody2D>();
        capCol = GetComponent<CapsuleCollider2D>();
        headCol = GetComponentInChildren<CircleCollider2D>();
        extraJumps = numberOfJumps;
        UpdateParts();
    }

    void FixedUpdate()
    {
        // the checkRadius is currently too large for the head - it sets up for spider climb though
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        if(!isGrounded) // slow the player's horizontal movement in the air
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

    void Update()
    {
        if(isGrounded == true)
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
        else if(Input.GetButton("Jump") && extraJumps == 0 && isGrounded == true && jumpGate == false)
        {
            rb.velocity = Vector2.up * jumpForce;
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
    }

    void Flip() // reverses the character sprite on turn
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
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
            movementSpeed = 5f;
            jumpForce = 5f;
            capCol.enabled = false;
            headCol.enabled = true;
            checkRadius = 0.5f; // this check radius allows for the spider climb
            rb.constraints = RigidbodyConstraints2D.None;
            groundChecker.transform.localPosition = new Vector2(0f, 0.47f); // issue if player is rotating, the ground checker should shift to become the entire player rigidbody
            head.position = gameObject.transform.position;
            groundChecker.transform.position = gameObject.transform.position;
        }
        else if (hasArms && !hasLegs) // need to change collider
        {
            Transform arms = gameObject.transform.Find("Arms");
            thisPos.y -= 0.24f;  
            head.position = thisPos;
            thisPos.y -= 0.76f;
            arms.position = thisPos; 
            partConfiguration = 2;
            movementSpeed = 7.5f;
            jumpForce = 10f;
            headCol.enabled = false;
            capCol.enabled = true;            
            capCol.size = new Vector2(0.6f , 1.6f);
            capCol.offset = new Vector2(0f , 0.03f);
            checkRadius = 0.25f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            groundChecker.transform.localPosition = new Vector2(0f, -0.76f);
        }
        else if (!hasArms && hasLegs) // need to change collider
        {
            partConfiguration = 3;
            movementSpeed = 10f;
            jumpForce = 13f;
            headCol.enabled = false;
            capCol.enabled = true;
            capCol.size = new Vector2(0.6f , 1.45f);
            capCol.offset = new Vector2(0f , -0.27f);
            checkRadius = 0.25f;
            thisPos.y -= 0.6f;
            head.position = thisPos;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            groundChecker.transform.localPosition = new Vector2(0f, -0.97f);
        }
        else if(hasArms && hasLegs) // need to change collider
        {
            thisPos.y -= 0.755f;
            Transform arms = gameObject.transform.Find("Arms");
            arms.position = thisPos;
            partConfiguration = 4;
            movementSpeed = 10f;
            jumpForce = 13f;
            headCol.enabled = false;
            capCol.enabled = true;
            capCol.size = new Vector2(0.6f , 2.08f);
            capCol.offset = new Vector2(0f , 0.03f);
            checkRadius = 0.25f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            groundChecker.transform.localPosition = new Vector2(0f, -0.97f);
        }
        // 1 is head, 2 adds torso, 3 adds legs, 4 adds torso and legs
    }
}
