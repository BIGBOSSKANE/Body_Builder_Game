using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newBladeBot : MonoBehaviour
{
    public GameObject startPos;

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

    float resetSpeed;

    public GameObject RightDestination;
    public GameObject LeftDestination;
    public GameObject UpDestination;
    public GameObject DownDestination;

    // Start is called before the first frame update
    void Start()
    {
        resetSpeed = speed;

        playerScript = GameObject.Find("Player").gameObject.GetComponent<playerScript>(); // we can swap this out for the scene manager once it has been added

        rb2d = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.position == startPos.transform.position)
        {
            movingBack = false;
            dU.SetActive(true);
            dD.SetActive(true);
            dL.SetActive(true);
            dR.SetActive(true);
        }

        else
        {
            dU.SetActive(false);
            dD.SetActive(false);
            dL.SetActive(false);
            dR.SetActive(false);
        }

        if (down)
        {
            transform.position = Vector2.MoveTowards(transform.position, DownDestination.transform.position, speed += speedAccelerationPerSecond * Time.deltaTime);
        }

        if (up)
        {
            transform.position = Vector2.MoveTowards(transform.position, UpDestination.transform.position, speed += speedAccelerationPerSecond * Time.deltaTime);
        }

        if (left)
        {
            transform.position = Vector2.MoveTowards(transform.position, LeftDestination.transform.position, speed += speedAccelerationPerSecond * Time.deltaTime);
        }

        if (right)
        {
            transform.position = Vector2.MoveTowards(transform.position, RightDestination.transform.position, speed += speedAccelerationPerSecond * Time.deltaTime);
        }

        if (movingBack)
        {
            transform.position = Vector2.MoveTowards(transform.position, startPos.transform.position, 5 * Time.deltaTime);
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
        yield return new WaitForSeconds(2);
        movingBack = true;
        speed = resetSpeed;
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

        if (col.gameObject.tag == "Environment" || col.gameObject.tag == "Enemy" || col.gameObject.tag == "Box" || col.gameObject.tag == "Object")
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