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

    GameObject lastArms;
    GameObject lastLegs;
    GameObject lastHead;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        SetParts(null, null, null);
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
        anim.SetTrigger("isIdle");
        anim.ResetTrigger("isRun");
    }

    public void Run()
    {
        anim.SetTrigger("isRun");
        anim.ResetTrigger("isIdle");
    }

    public void HandleMovement(Vector2 movement)
    {
        if (Mathf.Abs(movement.x) < 0.05f)
        {
            Idle();
        }
        else
        {
            Run();
        }
    }

    public void HandleOffset(Vector2 offset)
    {
        transform.localPosition = new Vector2(0, -1.1f);
    }
    
    public void ShowPart(GameObject part, bool shouldShow = true)
    {
        if (!part)
            return;
        part.GetComponent<SpriteRenderer>().enabled = shouldShow;
    }

    public void SetParts(GameObject legs, GameObject arms, GameObject head)
    {
        string animstate = "Base Layer.";
        lastHead = head;
        if (legs && arms)
        {
            Debug.Log("Full Body");
            lastLegs = legs;
            lastArms = arms;
            gameObject.SetActive(true); //Show ourselves
            ShowPart(lastLegs, false); //turn old legs off
            ShowPart(lastArms, false); //turn old arms off
            ShowPart(lastHead, false);
            animstate += animFullbody;
        }
        else if (legs) //legs only
        {
            Debug.Log("Legs Only");
            lastLegs = legs;
            gameObject.SetActive(true); //show ourselves
            ShowPart(lastLegs, false); //turn old legs off
            ShowPart(lastHead, false);
            if (lastArms) //we must have dumped our old arms
            {
                ShowPart(lastArms, true);
                lastArms = null; //forget them
            }

            animstate += animLegs;
        }
        else if (arms) //arms only
        {
            Debug.Log("Arms Only");
            lastArms = arms;
            gameObject.SetActive(true); //show ourselves
            ShowPart(lastArms, false); //hide old arms
            ShowPart(lastHead, false);
            if (lastLegs) //must have dumped old legs
            {
                ShowPart(lastLegs, true); //show old legs
                lastLegs = null; //forget them
            }
            animstate += animArms;
        }
        else //head only
        {
            Debug.Log("Head Only");
            gameObject.SetActive(false);
            if (lastArms) //we must have dumped our old arms
            {
                ShowPart(lastArms, true);
                lastArms = null; //forget them
            }
            if (lastLegs) //must have dumped old legs
            {
                ShowPart(lastLegs, true); //show old legs
                lastLegs = null; //forget them
            }
            ShowPart(lastHead, true);
            return; //don't play any additional anim state
        }
        Debug.Log("Playing " + animstate);
        anim.Play(animstate);
    }
}
