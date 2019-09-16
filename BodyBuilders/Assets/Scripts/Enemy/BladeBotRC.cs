using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeBotRC : MonoBehaviour
{
    public Transform originPointL;
    public Transform originPointR;
    public Transform originPointU;
    public Transform originPointD;

    private Vector2 dirL = new Vector2(-1, 0);
    private Vector2 dirR = new Vector2(1, 0);
    private Vector2 dirU = new Vector2(0, 1);
    private Vector2 dirD = new Vector2(0, -1);

    public float range = 7.5f;

    Rigidbody2D rb2d;

    playerScript playerScript;

    bool movingBack = false;

    public bool playerDetect;

    public float speed;

    public Vector2 startPos;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        playerScript = GameObject.Find("Player").gameObject.GetComponent<playerScript>(); // we can swap this out for the scene manager once it has been added
        startPos = gameObject.transform.position;
        originPointL = GameObject.Find("OriginPointL").transform;
        originPointR = GameObject.Find("OriginPointR").transform;
        originPointU = GameObject.Find("OriginPointU").transform;
        originPointD = GameObject.Find("OriginPointD").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(originPointL.position, dirL * range);
        RaycastHit2D hitL = Physics2D.Raycast(originPointL.position, Vector2.left, range);

        if (hitL.collider.CompareTag("Player"))
           {
            Debug.Log("hit left");
            ChaseL();
           }

        Debug.DrawRay(originPointR.position, dirR * range);
        RaycastHit2D hitR = Physics2D.Raycast(originPointR.position, dirR, range);
        
        if (hitR.collider.CompareTag("Player"))
            {
            Debug.Log("hit right");
            ChaseR();
            }
        
        Debug.DrawRay(originPointU.position, dirU * range);
        RaycastHit2D hitU = Physics2D.Raycast(originPointU.position, dirU, range);
        
        if (hitU.collider.CompareTag("Player"))
            {
            Debug.Log("hit up");
            ChaseU();
            }
        
        Debug.DrawRay(originPointD.position, dirD * range);
        RaycastHit2D hitD = Physics2D.Raycast(originPointD.position, dirD, range);
        
        if (hitD.collider.CompareTag("Player"))
            {
            Debug.Log("hit down");
            ChaseD();
            }

        speed = 4 * Time.time;
    }

    void ChaseL()
    {
        rb2d.AddRelativeForce(Vector2.left * speed);
    }

    void ChaseR()
    {
        rb2d.AddRelativeForce(Vector2.right * speed);
    }

    void ChaseU()
    {
        rb2d.AddRelativeForce(Vector2.up * speed);
    }

    void ChaseD()
    {
        rb2d.AddRelativeForce(Vector2.down * speed);
    }

    IEnumerator WaitASec()
    {
        yield return new WaitForSeconds(2);
        movingBack = true;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Groundbreakable"))
        {
            Destroy(col.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerScript.Die(0.2f);
        }

        if (col.gameObject.tag == "Environment")
        {
            Debug.Log("colided with environment");
            playerDetect = false;
            WaitASec();
        }
    }
}
