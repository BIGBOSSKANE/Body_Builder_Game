﻿/*
Creator: Brad
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RobotAnimations : MonoBehaviour
{
    public float moveThreshold = 0.05f;
    public float legsOffset = -1.1f;
    public float armsOffset = -0.8f;
    public SpriteRenderer[] defaultLegs; //should include lower torso
    public SpriteRenderer[] afterburnerLegs; //shouldn't include lower torso
    public SpriteRenderer[] groundbreakerLegs; //shouldn't include lower torso.
    public SpriteRenderer[] defaultArms;
    public SpriteRenderer[] shieldArms;
    public SpriteRenderer[] lifterArms;
    public SpriteRenderer[] animatedHead;
    public SpriteRenderer[] lowerTorso;
    public SpriteRenderer[] augmentedLowerTorso;

    Animator anim;
    float speed = 0.0f;
    GameObject lastArms;
    GameObject lastLegs;
    GameObject lastHead;
    bool holding;
    int armConfig = 1; //2=lifter, 3=deflector
    int legConfig = 1; //2=groundbreakers, 3=afterburners
    int headConfig = 1; //not used yet, 2=scaler, 3=hookshot, 4=all
    
 
    // Start is called before the first frame update
    
    void Start()
    {
        anim = GetComponent<Animator>();
        /*Disabled as this overwrites the spawn values
        SetConfigurations(1,1,1);
        SetParts(null, null, null);
        */
        //ToggleObjects(animatedHead, false );
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
    
    public void DrillTransition(bool drill) // not currently working, the player head is just vanishing and no animation is occuring
    {
        //anim.SetTrigger("isDrill");
    }    

    public void SetConfigurations(int legs, int arms, int head)
    {
        legConfig = legs;
        armConfig = arms;
        headConfig = head;
    }


    void UpdateVisibility()
    {
        //only show head if you have arms
        bool showhead = (lastArms != null);
        //Debug.Log("Showhead: " + showhead);
        ToggleObjects(animatedHead, showhead, showhead);
        ShowArms(lastArms != null);
        ShowLegs(lastLegs != null); 

        //This is for the 'torso head', but we have to disable augment sprites.
        if (lastLegs != null)
        {
            ToggleObjects(lowerTorso, true, true);
            if (lastArms == null)
            {
                ToggleObjects(augmentedLowerTorso, false, true);
            }
        }
    }

    public void SetParts(GameObject legs, GameObject arms, GameObject head)
    {
        lastHead = head;
        
        if (legs || arms)
        {
            if (legs && arms)
            {
                //Debug.Log("Full Body");
                lastLegs = legs;
                lastArms = arms;
                ShowPart(lastLegs, false); //turn old legs off
                ShowPart(lastArms, false); //turn old arms off
                ShowPart(lastHead, false);
                
            }
            else if (legs)
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
       
        UpdateVisibility();
        HandleOffset(arms, legs);
        anim.SetBool("hasArms", arms);
        anim.SetBool("hasLegs", legs);
        //anim.SetInteger("armConfig", armConfig);
        //anim.SetInteger("legConfig", legConfig);
        anim.Play("Base Layer.Root");
    }

    //This is for showing / hiding parts we pick up, not our own body parts.
    public void ShowPart(GameObject part, bool shouldShow = true)
    {
        if (!part)
            return;
        part.GetComponent<SpriteRenderer>().enabled = shouldShow;
    }

    void ShowArms(bool shouldShow)
    {
        //if hide arms, just turn off all the gameobjects
        //Debug.Log("Show Arms " + armConfig + ", " + shouldShow);
        if (!shouldShow)
        {
            ToggleObjects(defaultArms, false, false);
            return;
        }
        
        //otherwise enable the base arm gameobjects, and hide sprite
        //renderers

        //Step1: disable all renderers, but turn default game objects on.
        ToggleObjects(defaultArms, false, true); //needs to come first
        ToggleObjects(lifterArms, false);
        ToggleObjects(shieldArms, false);
        

        //Step2: enable renderers as necessary
        switch(armConfig)
        {
            case 1:
            {
                ToggleObjects(defaultArms, true);
                break;
            }
            case 2:
            {
                ToggleObjects(lifterArms, true);
                break;
            }
            case 3:
            {
                ToggleObjects(shieldArms, true);
                break;
            }
        }
    }

    void ShowLegs(bool shouldShow)
    {
        //if hide legs, just turn off all the gameobjects
        if (!shouldShow)
        {
            ToggleObjects(defaultLegs, false, false);
            return;
        }

        //otherwise, enable the base leg gameobjects, and hide sprite
        //renderers.

        //Step 1 disable all renderers, but turn default game objects on.
        ToggleObjects(groundbreakerLegs, false);
        ToggleObjects(afterburnerLegs, false);
        ToggleObjects(defaultLegs, false, true);
        
        //Step 2 enable sprite renderers as necessary.
        switch(legConfig)
        {
            case 1:
            {    
                ToggleObjects(defaultLegs, true, true);
                break;
            }
            case 2:
            {   
                ToggleObjects(groundbreakerLegs, true, true);
                break;
            }
            case 3:
            {   
                ToggleObjects(afterburnerLegs, true, true);
                break;
            }
        }
    }

    //This is for showing / hiding our own body parts.
    //First argument is a list of sprite renderers
    //Second argument is whether they should be shown or not.
    //Third argument is whether their game object should be enabled.
    void ToggleObjects(SpriteRenderer[] sprites, bool shouldShow, bool setActive = true)
    {
        //Debug.Log("Toggle Objects: " + shouldShow);
        foreach (SpriteRenderer s in sprites)
        {
            s.enabled = shouldShow;
            if (s.gameObject.activeSelf != setActive)
                s.gameObject.SetActive(setActive);
        }
    }
}
