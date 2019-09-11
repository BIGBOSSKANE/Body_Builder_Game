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


    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.Find("Player").gameObject.GetComponent<playerScript>(); // we can swap this out for the scene manager once it has been added
        rb2d = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb2d.velocity = new Vector2(velocity, rb2d.velocity.y);

        colliding = Physics2D.Linecast(sightStart.position, sightEnd.position, DetectWhat);

        if (colliding)
        {
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            velocity *= -1;
        }
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerScript.Die(0.2f);
        }
    }
}
