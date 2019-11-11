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

    Dictionary<SpriteRenderer, Sprite> allSprites;
    bool shouldRestore;
    bool shouldTransition;
    bool shouldBecomeActive;
    string nextAnimationState;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        allSprites = new Dictionary<SpriteRenderer, Sprite>();
        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in srs)
        {
            allSprites[sr] = sr.sprite;
            //Debug.Log("" + sr + ", " + sr.sprite);
        }
        Debug.Log("Catalogued " + srs.Length + " sprites");
        shouldRestore = false;
        shouldBecomeActive = false;
        SetParts(null, null, null);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            shouldTransition = true;
        }
    }

    
    void LateUpdate()
    {
        //if (shouldRestore)
        //{
        //    RestoreSprites();
        //    shouldRestore = false;
        //} 

        if (shouldBecomeActive && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            //return for now. Next lateupdate we will transition
            Debug.Log("Made Game Object Active. Transition next update");
            return;
        }
        /* 
        if (shouldTransition && !shouldBecomeActive && gameObject.isActiveSelf)
        {
            //don't switch off yet. play enable all.
            anim.Play("Base Layer.EnableAll");
        }
        */
        if (shouldTransition)
        {
            Debug.Log("Transition");
            anim.Play("Base Layer.EnableAll");
            //if (nextAnimationState != "")
            //{
                //anim.Play(nextAnimationState);
                //nextAnimationState = "";
            //    shouldTransition = false;
            //}
        }
    }

    void CompleteTransition()
    {
        Debug.Log("Complete Transition here");
        //Code for turning off.
        if (shouldTransition && !shouldBecomeActive && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        shouldTransition = false;
        //anim.Play(nextAnimationState);
        //shouldTransition = true;
    }

    void TransitionTo(string nextState)
    {
        Debug.Log("Setting transition to " + nextState);
        nextAnimationState = nextState;
        //shouldTransition = true;
        anim.Play("Base Layer.EnableAll");
        //anim.enabled = false;
        //anim.enabled = true;
        return;
        //anim.Play(animFullbody);
        
        foreach (SpriteRenderer sr in allSprites.Keys)
        {
            sr.enabled = true;
        }
        
        return;
        anim.enabled = false;
        
        
        foreach (SpriteRenderer sr in allSprites.Keys)
        {
            sr.sprite = allSprites[sr];
            //Debug.Log("Set sprite for " + sr + " to " + sr.sprite);
        }
        anim.enabled = true;
        
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
            //gameObject.SetActive(true); //Show ourselves
            shouldBecomeActive = true;
            ShowPart(lastLegs, false); //turn old legs off
            ShowPart(lastArms, false); //turn old arms off
            ShowPart(lastHead, false);
            animstate += animFullbody;
        }
        else if (legs) //legs only
        {
            Debug.Log("Legs Only");
            lastLegs = legs;
            //gameObject.SetActive(true); //show ourselves
            shouldBecomeActive = true;
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
            //gameObject.SetActive(true); //show ourselves
            shouldBecomeActive = true;
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
            //gameObject.SetActive(false);
            shouldBecomeActive = false;
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
            //return; //don't play any additional anim state
        }
        
        
        //RestoreSprites();
        //shouldRestore = true;
        
        //anim.Play("Base Layer.EnableAll");
        //anim.Play(animstate);
        //RestoreSprites();
        if (shouldBecomeActive && !gameObject.activeSelf)
            gameObject.SetActive(true);
        
        anim.SetBool("hasArms", arms);
        anim.SetBool("hasLegs", legs);
        //TransitionTo(animstate);
        shouldTransition = true;
        //anim.Play("Base Layer.EnableAll");
    }
}
