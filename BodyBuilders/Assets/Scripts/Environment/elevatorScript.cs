// get the direction between the two elevator points, and then apply the force that way (angling slightly up if it is just horizontal)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevatorScript : MonoBehaviour
{
    public bool activated = true;
    public float moveTime = 5f;
    public bool jumpBooster = false;
    public float launchForce = 3f;
    public float jumpTimeOffset = 0.2f;
    public float jumpLaunchForce = 6f;
    public float slamUpTime = 0.8f;
    public LayerMask liftableObjects;
    public float moveTimer = 0f;
    bool ascending = true;
    float triggerCheckTime = 0.3f;
    bool playerOnboard = false;
    bool slam;
    Vector2 lowPoint;
    Vector2 highPoint;
    Vector2 slamDirection;
    GameObject player;
    GameObject holder;
    playerScript playerScript;
    BoxCollider2D pickupPoint;
    
    void Start()
    {
        lowPoint = gameObject.transform.Find("lowPoint").gameObject.transform.position;
        highPoint = gameObject.transform.Find("highPoint").gameObject.transform.position;
        pickupPoint = gameObject.GetComponent<BoxCollider2D>();
        transform.position = lowPoint;
        moveTimer = 0f;
        ascending = true;
        playerOnboard = false;
        player = GameObject.Find("Player").gameObject;
        holder = gameObject.transform.parent.gameObject.transform.Find("ElevatorHolder").gameObject;
        holder.transform.position = Vector3.zero;
        holder.transform.localScale = new Vector3(1f , 1f, 1f);
        holder.transform.rotation = Quaternion.Euler(0f , 0f , 0f);
        playerScript = player.GetComponent<playerScript>();

        slamDirection = highPoint - lowPoint;
        slamDirection = slamDirection.normalized;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(lowPoint, highPoint);
    }

    void FixedUpdate()
    {
        holder.transform.position = transform.position;

        if(!activated)
        {
            return;
        }

        if(moveTimer >= 1f)
        {
            ascending = !ascending;
            moveTimer = 0f;
        }

        if(moveTimer > (1f - jumpTimeOffset))
        {
            slam = true;
        }

        playerScript.jumpBan = false;
        if(!ascending)
        {
            slam = false;
        }
        else
        {
            if(playerOnboard)
            {
                playerScript.jumpBan = true;
            }
        }

        if(ascending)
        {
            if(jumpBooster)
            {
                moveTimer += Time.fixedDeltaTime / slamUpTime;
                if(playerOnboard && moveTimer >= (moveTime - jumpTimeOffset) && Input.GetAxis("Vertical") > 0f && ascending == true)
                {
                    playerScript.slamVector = slamDirection * jumpLaunchForce;
                    playerScript.slam = 0f;
                    player.GetComponent<Rigidbody2D>().AddForce(slamDirection * jumpLaunchForce , ForceMode2D.Impulse);
                }
            }
            else
            {   
                moveTimer += Time.fixedDeltaTime / moveTime;
            }
            transform.position = new Vector2(Mathf.SmoothStep(lowPoint.x , highPoint.x , moveTimer) , Mathf.SmoothStep(lowPoint.y , highPoint.y , moveTimer));
        }
        else
        {
            moveTimer += Time.fixedDeltaTime / moveTime;
            transform.position = new Vector2(Mathf.SmoothStep(highPoint.x , lowPoint.x , moveTimer) , Mathf.SmoothStep(highPoint.y , lowPoint.y , moveTimer));
        }
    }

    void Update()
    {
        if(Input.GetKey("space") && playerOnboard == true)
        {
            pickupPoint.enabled = false;
            pickupPoint.enabled = true;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Parent(col);
    }

    void Parent(Collider2D col)
    {
        if(Input.GetAxis("Vertical") < 0.1f && (col.tag != "Untagged" || col.tag != "PassThroughPlatform" || col.tag != "Environment"))
        {
            if(col.tag == "Player" && playerOnboard == false)
            {
                playerOnboard = true;
                
                player.transform.parent = holder.transform;
                
                if(playerScript.partConfiguration == 1)
                {
                    Destroy(player.GetComponent<BoxCollider2D>());
                }
            }
            else
            {
                if(col.gameObject.transform.parent == null)
                {
                    col.gameObject.transform.parent = holder.transform;
                }
                else if(col.gameObject.transform.parent.parent == null)
                {
                    col.gameObject.transform.parent.parent = holder.transform;
                }
                else if(col.gameObject.transform.parent.parent.parent == null)
                {
                    col.gameObject.transform.parent.parent.parent = holder.transform;
                }
                else if(col.gameObject.transform.parent.parent.parent.parent == null)
                {
                    col.gameObject.transform.parent.parent.parent.parent = holder.transform;
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if(slam == true)
        {
            if(col.tag == "Player")
            {
                playerScript.slamVector = slamDirection * launchForce;
                playerScript.slam = 0f;
                player.GetComponent<Rigidbody2D>().AddForce(slamDirection * launchForce , ForceMode2D.Impulse);
            }
            else if(col.gameObject.tag == "Legs" || col.gameObject.tag == "Arms")
            {
                col.transform.parent.gameObject.GetComponent<Rigidbody2D>().AddForce(slamDirection * launchForce , ForceMode2D.Impulse);
            }
            else
            {
                col.gameObject.GetComponent<Rigidbody2D>().AddForce(slamDirection * launchForce , ForceMode2D.Impulse);
            }

            if(col.transform.position.y <= holder.transform.position.y)
            {
                col.transform.position = new Vector2(col.transform.position.x , holder.transform.position.y);
            }
        }

        if(moveTimer % triggerCheckTime <= 0.0005f && moveTimer % triggerCheckTime >= -0.0005f) // check the trigger periodically in case anything has dodged the TriggerEnter window
        {
            Parent(col);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.tag != "Untagged" || col.tag != "PassThroughPlatform")
        {
            if(col.tag == "Player") // && playerOnboard == true)
            {
                playerOnboard = false;
                var OriginalConstraints = player.GetComponent<Rigidbody2D>().constraints;
                playerOnboard = false;
                player.transform.parent = null;
                player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                player.GetComponent<Rigidbody2D>().constraints = OriginalConstraints;

                if(playerScript.partConfiguration == 1)
                {
                    Destroy(player.GetComponent<BoxCollider2D>());
                }
            }
            else
            {
                if(col.gameObject.transform.parent != null && col.gameObject.transform.parent == holder)
                {
                    col.gameObject.transform.parent = null;
                }
                else if(col.gameObject.transform.parent.parent != null && col.gameObject.transform.parent.parent == holder)
                {
                    col.gameObject.transform.parent.gameObject.transform.parent = null;
                }
            }
        }
    }
}