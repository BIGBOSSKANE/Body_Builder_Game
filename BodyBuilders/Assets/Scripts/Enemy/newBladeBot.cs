using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newBladeBot : MonoBehaviour
{
    public Vector2 startPos;

    public bool playerDetect;

    playerScript playerScript;

    public GameObject dD; //detect down
    public GameObject dU; //detect up
    public GameObject dL; //detect left
    public GameObject dR; //detect right

    bool up = false;
    bool down = false;
    bool left = false;
    bool right = false;

    Rigidbody2D rb2d;

    bool movingBack = false;

    float speed;

    Vector2 RightDestination;
    Vector2 LeftDestination;
    Vector2 UpDestination;
    Vector2 DownDestination;

    // Start is called before the first frame update
    void Start()
    {
        dD = GameObject.Find("Detect Down");
        dU = GameObject.Find("Detect Up");
        dL = GameObject.Find("Detect Left");
        dR = GameObject.Find("Detect Right");

        RightDestination = GameObject.Find ("RightDestination").transform.position;
        LeftDestination = GameObject.Find("LeftDestination").transform.position;
        UpDestination = GameObject.Find("UpDestination").transform.position;
        DownDestination = GameObject.Find("DownDestination").transform.position;

        startPos = gameObject.transform.position;

        playerScript = GameObject.Find("Player").gameObject.GetComponent<playerScript>(); // we can swap this out for the scene manager once it has been added

        rb2d = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (dD.GetComponent<BladeBotDetect>().playerDetect == true)
        {
            Debug.Log("down detected");
            playerDetect = true;
            if (playerDetect)
            {
                ChargeD();
            }
        }
        if (dU.GetComponent<BladeBotDetect>().playerDetect == true)
        {
            Debug.Log("up detected");
            playerDetect = true;
            if (playerDetect)
            {
                ChargeU();
            }
        }
        if (dL.GetComponent<BladeBotDetect>().playerDetect == true)
        {
            Debug.Log("left detected");
            playerDetect = true;
            if (playerDetect)
            {
                ChargeL();
            }
        }
        if (dR.GetComponent<BladeBotDetect>().playerDetect == true)
        {
            Debug.Log("right detected");
            playerDetect = true;
            if (playerDetect)
            {
                ChargeR();
            }
        }
        if (movingBack)
        {
            transform.position = Vector2.MoveTowards(transform.position, startPos, speed * Time.time);
            dU.SetActive(true);
            dD.SetActive(true);
            dL.SetActive(true);
            dR.SetActive(true);
        }
        speed = 2.5f * Time.time;
    }

    void ChargeU()
    {
        dU.SetActive(false);
        dD.SetActive(false);
        dL.SetActive(false);
        dR.SetActive(false);
        transform.position = Vector2.MoveTowards(transform.position, UpDestination, speed);
    }

    void ChargeD()
    {
        dU.SetActive(false);
        dD.SetActive(false);
        dL.SetActive(false);
        dR.SetActive(false);
        transform.position = Vector2.MoveTowards(transform.position, DownDestination, speed);
    }

    void ChargeL()
    {
        dU.SetActive(false);
        dD.SetActive(false);
        dL.SetActive(false);
        dR.SetActive(false);
        transform.position = Vector2.MoveTowards(transform.position, LeftDestination, speed);
    }

    void ChargeR()
    {
        dU.SetActive(false);
        dD.SetActive(false);
        dL.SetActive(false);
        dR.SetActive(false);
        transform.position = Vector2.MoveTowards(transform.position, RightDestination, speed);
    }

    IEnumerator WaitASec()
    {
        yield return new WaitForSeconds(2);
        movingBack = true;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerScript.Die(0.2f);
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("Groundbreakable"))
        {
            Destroy(col.gameObject);
        }

        if (col.gameObject.tag == "Environment")
        {
            Debug.Log("colided with environment");
            playerDetect = false;
            WaitASec();
        }
    }
}