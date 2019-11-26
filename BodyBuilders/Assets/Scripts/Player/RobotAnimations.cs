using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RobotAnimations : MonoBehaviour
{
    Animator anim;
    float speed = 0.0f;
    GameObject lastArms;
    GameObject lastLegs;
    GameObject lastHead;
    public float moveThreshold = 0.05f;
    public float legsOffset = -1.1f;
    public float armsOffset = -0.8f;
    bool holding;
    int armConfig = 1; //2=lifter, 3=deflector
    int legConfig = 1; //2=groundbreakers, 3=afterburners
    int headConfig = 1; //not used yet, 2=scaler, 3=hookshot, 4=all
    
 
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        SetParts(null, null, null);
    }

    void CompleteTransition()
    {
        //No need for this function any more, but it's an anim trigger.
    }

    public void Hold(bool val)
    {
        //Only make the change if there's a differenc
        if (holding != val)
        {
            holding = val;
            anim.SetBool("holding", holding);
        }
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
        if (Mathf.Abs(movement.x) < moveThreshold)
        {
            Idle();
        }
        else
        {
            Run();
        }
    }

    public void HandleOffset(bool arms, bool legs)
    {
        float offset = 0.0f;
        if (arms && legs)
        {
            offset = legsOffset;
        }
        else if (legs)
        {
            offset = legsOffset;
        }
        else if (arms)
        {
            offset = armsOffset;
        }
        transform.localPosition = new Vector2(0, offset);
    }
    
    public void ShowPart(GameObject part, bool shouldShow = true)
    {
        if (!part)
            return;
        part.GetComponent<SpriteRenderer>().enabled = shouldShow;
    }

    public void SetConfigurations(int legs, int arms, int head)
    {
        legConfig = legs;
        armConfig = arms;
        headConfig = head;
    }

    public void SetParts(GameObject legs, GameObject arms, GameObject head)
    {
        lastHead = head;
        if (legs && arms)
        {
            //Debug.Log("Full Body");
            lastLegs = legs;
            lastArms = arms;
            ShowPart(lastLegs, false); //turn old legs off
            ShowPart(lastArms, false); //turn old arms off
            ShowPart(lastHead, false);
        }
        else if (legs) //legs only
        {
            //Debug.Log("Legs Only");
            lastLegs = legs;
            ShowPart(lastLegs, false); //turn old legs off
            ShowPart(lastHead, false);
            if (lastArms) //we must have dumped our old arms
            {
                ShowPart(lastArms, true);
                lastArms = null; //forget them
            }
        }
        else if (arms) //arms only
        {
            //Debug.Log("Arms Only");
            lastArms = arms;
            ShowPart(lastArms, false); //hide old arms
            ShowPart(lastHead, false);
            if (lastLegs) //must have dumped old legs
            {
                ShowPart(lastLegs, true); //show old legs
                lastLegs = null; //forget them
            }
            
        }
        else //head only
        {
            //Debug.Log("Head Only");
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
        }
       
        HandleOffset(arms, legs);
        anim.SetBool("hasArms", arms);
        anim.SetBool("hasLegs", legs);
        anim.Play("Base Layer.Root");
    }
}
