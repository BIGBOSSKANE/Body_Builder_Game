using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RobotAnimations : MonoBehaviour
{
    public string animFullbody = "default_fullbody";
    public string animLegs = "default_legs";
    public string animArms = "default_arms";
    
    Animator anim;
    float speed = 0.0f;
    bool isJumping = false;
    bool isLanding = false;
    bool hasLegs = false;
    bool hasArms = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Land()
    {
        anim.ResetTrigger("isJump");
        anim.SetTrigger("isLand");
    }

    public void Jump()
    {
        anim.ResetTrigger("isLand");
        anim.SetTrigger("isJump");
    }

    public void Idle()
    {

    }

    public void Run()
    {

    }
    
    public void SetParts(bool legs, bool arms)
    {
        if (legs && arms)
        {
            Debug.Log("Full Body");
            gameObject.SetActive(true);
            anim.Play("Base Layer/" + animFullbody);
        }
        else if (legs) //legs only
        {
            Debug.Log("Legs Only");
            gameObject.SetActive(true);
            anim.Play("Base Layer/" + animLegs);
        }
        else if (arms) //arms only
        {
            Debug.Log("Arms Only");
            gameObject.SetActive(true);
            anim.Play("Base Layer/" + animArms);
        }
        else //head only
        {
            Debug.Log("Head Only");
            gameObject.SetActive(false);
        }
    }
}
