using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Animation_Catcher : MonoBehaviour
{
    public bool Open;
    public bool doorIsOpening;
    public Animator Animation;
    public GameObject SecretDoor;
    public AudioClip switchSound;
    public AudioClip DoorOpen;
    // Use this for initialization
    void Start()
    {
        Animation = GetComponent<Animator>();
        Open = false;
    }
    private void Update()
    {
        if (doorIsOpening == true)
        {
            Open = true;
        }
    }
    public void openSecretRoom()
    {
        doorIsOpening = true;
    }
}