using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newBladeBot : MonoBehaviour
{
    public Vector3 startPos;

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

    public float speedAccelerationPerSecond = 1f;

    Rigidbody2D rb2d;

    bool movingBack = false;

    public float speed;

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

        startPos = GameObject.Find("StartPosition").transform.position;

        playerScript = GameObject.Find("Player").gameObject.GetComponent<playerScript>(); // we can swap this out for the scene manager once it has been added

        rb2d = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.position == startPos)
        {
            Debug.Log("reset");
            movingBack = false;
            dU.SetActive(true);
            dD.SetActive(true);
            dL.SetActive(true);
            dR.SetActive(true);
        }

        else
        {
            Debug.Log("moving");
            dU.SetActive(false);
            dD.SetActive(false);
            dL.SetActive(false);
            dR.SetActive(false);
        }

                if (down)
        {
            Debug.Log("down detected");
            transform.position = Vector2.MoveTowards(transform.position, DownDestination, speed += speedAccelerationPerSecond * Time.deltaTime);
        }

        if (up)
        {
            Debug.Log("up detected");
            transform.position = Vector2.MoveTowards(transform.position, UpDestination, speed += speedAccelerationPerSecond * Time.deltaTime);
        }

        if (left)
        {
            Debug.Log("left detected");
            transform.position = Vector2.MoveTowards(transform.position, LeftDestination, speed += speedAccelerationPerSecond * Time.deltaTime);
        }

        if (right)
        {
            Debug.Log("right detected");
            transform.position = Vector2.MoveTowards(transform.position, RightDestination, speed += speedAccelerationPerSecond * Time.deltaTime);
        }

        if (movingBack)
        {
            transform.position = Vector2.MoveTowards(transform.position, startPos, 5 * Time.deltaTime);
        }
    }

    public void ChargeU()
    {
        dU.SetActive(false);
        dD.SetActive(false);
        dL.SetActive(false);
        dR.SetActive(false);
        up = true;
    }

    public void ChargeD()
    {
        dU.SetActive(false);
        dD.SetActive(false);
        dL.SetActive(false);
        dR.SetActive(false);
        down = true;
    }

    public void ChargeL()
    {
        dU.SetActive(false);
        dD.SetActive(false);
        dL.SetActive(false);
        dR.SetActive(false);
        left = true;
    }

    public void ChargeR()
    {
        dU.SetActive(false);
        dD.SetActive(false);
        dL.SetActive(false);
        dR.SetActive(false);
        right = true;
    }

    IEnumerator WaitASec()
    {
        Debug.Log("waiting");
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

        if (col.gameObject.tag == "Environment" || col.gameObject.tag == "Enemy")
        {
            Debug.Log("colided with environment");
            rb2d.velocity = Vector2.zero;
            up = false;
            down = false;
            left = false;
            right = false;
            playerDetect = false;
            StartCoroutine(WaitASec());
        }
    }
}