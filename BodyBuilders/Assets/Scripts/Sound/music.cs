/*
Creator: Daniel
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class music : MonoBehaviour
{
    bool alive;
    bool heroic;
    int threat; // 0 is atmospheric, 1 is enemies nearby, 2 is danger,3 is boss fight
    int partConfiguration; // 1 is head, 2 is head and arms, 3 is head and legs, 4 is full body


    void Start()
    {
        AkSoundEngine.PostEvent("PlayGameMusic" , gameObject);
    }

    public void PlayerKilled()
    {
        AkSoundEngine.PostEvent("PlayerDeath" , gameObject);
    }

    public void BansheeScream()
    {
        AkSoundEngine.PostEvent("BansheeScream" , gameObject);
    }
}
