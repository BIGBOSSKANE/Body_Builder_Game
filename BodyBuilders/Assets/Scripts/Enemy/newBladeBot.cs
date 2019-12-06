//Created by Kane Girvan
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newBladeBot : MonoBehaviour
{
    public GameObject startPos;

    public bool playerDetect;
    public bool movingBack = false;
    bool collided = false;

    playerScript playerScript;

    public GameObject dD; //detect down
    public GameObject dU; //detect up
    public GameObject dL; //detect left
    public GameObject dR; //detect right
    public GameObject blade;

    bool up = false;
    bool down = false;
    bool left = false;
    bool right = false;

    [Tooltip("The amount of speed that is gained per frame.")]
    public float speedAccelerationPerSecond = 1f;
    [Tooltip("The highest limit the speed the bladebot can go")]
    public float maxSpeed = 1.39f;
    [Tooltip("The speed tthat the blade bot starts with.")]
    public float speed = 0.07f;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (playerDetect)
        {
            if (speed > maxSpeed)
            {
                speed = maxSpeed;
            }
            else if (speed < maxSpeed && !collided)
            {
                speed += Time.deltaTime * speedAccelerationPerSecond;
            }
        }

        blade.gameObject.transform.Rotate(Vector3.forward , 10 * speed);

        if (gameObject.transform.position == startPos.transform.position)
        {
            movingBack = false;
            dU.SetActive(true);
            dD.SetActive(true);
            dL.SetActive(true);
            dR.SetActive(true);
        }

        else
        {
            if (!collided && movingBack)
            {
                movingBack = true;
                dU.SetActive(false);
                dD.SetActive(false);
                dL.SetActive(false);
                dR.SetActive(false);
            }
        }

        if (gameObject.transform.position != startPos.transform.position && !playerDetect && !movingBack)
        {
            movingBack = true;
            dU.SetActive(false);
            dD.SetActive(false);
            dL.SetActive(false);
            dR.SetActive(false);
        }

        if (down)
        {
            transform.position = Vector2.MoveTowards(transform.position, DownDestination.transform.position, speed * Time.deltaTime);
            blade.gameObject.transform.Rotate(Vector3.forward, 10 * speed * 2);
        }

        if (up)
        {
            transform.position = Vector2.MoveTowards(transform.position, UpDestination.transform.position, speed * Time.deltaTime);
            blade.gameObject.transform.Rotate(Vector3.forward, 10 * speed * 2);
        }

        if (left)
        {
            transform.position = Vector2.MoveTowards(transform.position, LeftDestination.transform.position, speed * Time.deltaTime);
            blade.gameObject.transform.Rotate(Vector3.forward, 10 * speed * 2);
        }

        if (right)
        {
            transform.position = Vector2.MoveTowards(transform.position, RightDestination.transform.position, speed * Time.deltaTime);
            blade.gameObject.transform.Rotate(Vector3.forward, 10 * speed * 2);
        }

        if (movingBack)
        {
            transform.position = Vector2.MoveTowards(transform.position, startPos.transform.position, 5 * Time.deltaTime);
            blade.gameObject.transform.Rotate(Vector3.forward, 5 * maxSpeed);
        }
    }

    public void ChargeU()
    {
        dU.SetActive(false);
        dD.SetActive(false);
        dL.SetActive(false);
        dR.SetActive(false);
        up = true;
        playerDetect = true;
    }

    public void ChargeD()
    {
        dU.SetActive(false);
        dD.SetActive(false);
        dL.SetActive(false);
        dR.SetActive(false);
        down = true;
        playerDetect = true;
    }

    public void ChargeL()
    {
        dU.SetActive(false);
        dD.SetActive(false);
        dL.SetActive(false);
        dR.SetActive(false);
        left = true;
        playerDetect = true;
    }

    public void ChargeR()
    {
        dU.SetActive(false);
        dD.SetActive(false);
        dL.SetActive(false);
        dR.SetActive(false);
        right = true;
        playerDetect = true;
    }

    IEnumerator WaitASec()
    {
        yield return new WaitForSeconds(2);
        playerDetect = false;
        movingBack = true;
        collided = false;
        speed = resetSpeed;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerScript.Die(0.2f);
            AkSoundEngine.PostEvent("GyreSlice" , gameObject);
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("Groundbreakable"))
        {
            Destroy(col.gameObject);
        }

        if (col.gameObject.tag == "Environment" || col.gameObject.tag == "Enemy" || col.gameObject.tag == "Box" || col.gameObject.tag == "Object" || col.gameObject.tag == "Legs" || col.gameObject.tag == "Arms")
        {
            collided = true;
            speed = 0;
            up = false;
            down = false;
            left = false;
            right = false;
            StartCoroutine(WaitASec());
        }
    }
}