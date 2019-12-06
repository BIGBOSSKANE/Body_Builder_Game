/*
Creator: Daniel
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSound : MonoBehaviour
{
    Animator animator;
    int partConfiguration; // 1 is head, 2 is arms, 3 is legs, 4 is all
    bool playingMusic;


    // Jumping

    bool jumpSoundUnavailable;
    float timeLimitSinceLastJump = 1f;
    float timerSinceLastJump = 0f;

    void Start()
    {
        //animator = GetComponent<Animator>();
        partConfiguration = GetComponent<playerScript>().partConfiguration;
    }

    void Update()
    {
        // right click in the animator, add event calling for footstep play

        /*
        
            AkSoundEngine.SetState();

        */


        if(jumpSoundUnavailable)
        {
            timerSinceLastJump += Time.deltaTime / timeLimitSinceLastJump;
            if(timerSinceLastJump >= 1)
            {
                timerSinceLastJump = 0;
                jumpSoundUnavailable = false;
            }
        }
    }

    void FoostepPlay()
    {
        AkSoundEngine.PostEvent("Foostep" , gameObject);
    }

    public void JumpPlay()
    {
        if(!jumpSoundUnavailable)
        {
            AkSoundEngine.PostEvent("Jump" , gameObject);
            jumpSoundUnavailable = true;
        }
    }

    public void AttachPlay()
    {
        AkSoundEngine.PostEvent("Attach" , gameObject);
    }

    public void DetachPlay()
    {
        AkSoundEngine.PostEvent("Detach" , gameObject);
    }

    public void LandingPlay()
    {
        AkSoundEngine.PostEvent("Landing" , gameObject);
    }

    public void HeavyLandingPlay()
    {
        AkSoundEngine.PostEvent("HeavyLanding" , gameObject);
    }

    public void DeathPlay()
    {
        AkSoundEngine.PostEvent("Death" , gameObject);
    }

    public void Respawn()
    {
        AkSoundEngine.PostEvent("Respawn" , gameObject);
    }

    public void RollPlay()
    {
        AkSoundEngine.PostEvent("Roll" , gameObject);
    }

    public void RollStop()
    {
        AkSoundEngine.PostEvent("RollStop" , gameObject);
    }

    public void PickUpPlay()
    {
        AkSoundEngine.PostEvent("PickUpBox" , gameObject);
    }

    public void DropPlay()
    {
        AkSoundEngine.PostEvent("DropBox" , gameObject);
    }

    public void ShootPlay()
    {
        AkSoundEngine.PostEvent("Shoot" , gameObject);
    }

    public void ShieldPlay()
    {
        AkSoundEngine.PostEvent("Shield" , gameObject);
    }

    public void ShieldStop()
    {
        AkSoundEngine.PostEvent("ShieldStop" , gameObject);
    }
}
