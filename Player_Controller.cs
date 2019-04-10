using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    public int partConfiguration = 1; // 1 is head, 2 adds arms, 3 adds legs, 4 adds arms and legs
    public float movementSpeed = 10f;
    public float speed;
    public float jumpForce = 10f;
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
        
        if(Input.GetButton("Jump") && extraJumps > 0)
        {
            rb.velocity = Vector2.up * jumpForce;
            extraJumps --;
        }
        else if(Input.GetButton("Jump") && extraJumps == 0 && isGrounded == true)
        {
            rb.velocity = Vector2.up * jumpForce;
        }

        if(rb.velocity.y < 0f) // fast fall for nice jumping
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0f && !Input.GetButton("Jump"))
        {
             rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;           
        }

        if (Input.GetKeyDown("space") && partConfiguration != 1)
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
        Transform head = gameObject.transform.Find("Head");
        head.position = thisPos;

        if(!hasArms && !hasLegs)
        {
            partConfiguration = 1;
            movementSpeed = 5f;
            jumpForce = 5f;
            capCol.enabled = false;
            headCol.enabled = true;
            rb.constraints = RigidbodyConstraints2D.None;
            groundChecker.transform.localPosition = new Vector2(0f, 0.47f); // issue if player is rotating, the ground checker should shift to become the entire player rigidbody
        }
        else if (hasArms && !hasLegs) // need to change collider
        {
            partConfiguration = 2;
            movementSpeed = 7.5f;
            jumpForce = 7.5f;
            headCol.enabled = false;
            capCol.enabled = true;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            groundChecker.transform.localPosition = new Vector2(0f, -0.76f);
        }
        else if (!hasArms && hasLegs) // need to change collider
        {
            partConfiguration = 3;
            movementSpeed = 10f;
            jumpForce = 10f;
            headCol.enabled = false;
            capCol.enabled = true;
            thisPos.y -= 0.6f;
            head.position = thisPos;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            groundChecker.transform.localPosition = new Vector2(0f, -0.97f);
        }
        else if(hasArms && hasLegs) // need to change collider
        {
            partConfiguration = 4;
            movementSpeed = 10f;
            jumpForce = 10f;
            headCol.enabled = false;
            capCol.enabled = true;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            groundChecker.transform.localPosition = new Vector2(0f, -0.97f);
        }
        // 1 is head, 2 adds torso, 3 adds legs, 4 adds torso and legs
    }
}
