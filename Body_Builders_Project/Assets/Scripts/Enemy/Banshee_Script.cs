using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banshee_Script : MonoBehaviour
{
    bool spotted = false;
    public GameObject target;
    public float laserCharge = 1.5f;
    public float laserFireTime = 3f;

    // Start is called before the first frame update
    void Start()
    {
        spotted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (spotted == true)
        {
            Vector3 targ = target.transform.position;
            targ.z = 0f;

            Vector3 objectPos = transform.position;
            targ.x = targ.x - objectPos.x;
            targ.y = targ.y - objectPos.y;

            float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
    void OnTriggerEnter2D(Collider2D Player)
    {
        if (Player.gameObject) // if the player's object
        {
            Debug.Log("Player has been spotted");
            spotted = true;
        }
    }
}