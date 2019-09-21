using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGyre : MonoBehaviour
{
    public float velocity = 2.5f;

    public Transform sightStart;
    public Transform sightEnd;

    public LayerMask DetectWhat;

    public bool colliding;

    Rigidbody2D rb2d;
    playerScript playerScript;

    public GameObject transformation;

    public float range = 10f;

    public Vector3 pos;

    public bool patrolling = true;

    public bool detected = false;

    public float distance;

    public Transform groundDetection;


    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.Find("Player").gameObject.GetComponent<playerScript>(); // we can swap this out for the scene manager once it has been added
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        Vector2 pos = new Vector3(0f, 0.465f,0f);
        patrolling = true;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distance);

        if (patrolling)
        {
            rb2d.velocity = new Vector2(velocity, rb2d.velocity.y);
        }

        colliding = Physics2D.Linecast(sightStart.position, sightEnd.position, DetectWhat);

        if (colliding || groundInfo.collider == false)
        {
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            velocity *= -1;
        }

        if(detected)
        {
            StartCoroutine(Processing());
        }
    }

    public IEnumerator Processing()
    {
        patrolling = false;
        yield return new WaitForSeconds(1);
        Transforming();
    }

    public void Transforming()
    {
        Destroy(gameObject);
        Instantiate(transformation, gameObject.transform.position + pos, Quaternion.identity);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerScript.Die(0.2f);
        }
    }
}