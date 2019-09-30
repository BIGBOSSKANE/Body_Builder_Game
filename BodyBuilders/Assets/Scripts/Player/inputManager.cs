/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class inputManager
{
    public static bool controller;
    public static float joystickDeadzone = 0.1f;
    public static float jumpDeadzone = 0.75f;
    public static float joystickSensitivity = 1f;
    public static float mouseSensitivity = 1f;

    void Start()
    {
        paused = false;
    }

    void Update()
    {
        Pause();
        Movement();
        Jump();
        AimDirection();
        Scout();
        Interact();
        Fire();
    }

    void Pause()
    {

    }

    void Movement()
    {
        float upwards = Input.GetAxis("Vertical");
        float sideways = Input.GetAxis("Horizontal");

        // In case the controller joystick is slightly off-centre
        if(sideways < joystickDeadzone && sideways > -joystickDeadzone) sideways = 0;
        if(upwards < jumpDeadzone && upwards > -jumpDeadzone) upwards = 0;

        Vector2 direction = new Vector2(sideways , upwards);
        if(direction != Vector2.zero)
        {

        }
    }

    void Jump()
    {
        if(Input.GetButton("Jump") || Input.GetButton("Controller Jump") || direction.y > 0.8f)
        {

        }
    }

    void AimDirection()
    {
        float rotationX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float rotationY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        float controllerRotX = Input.GetAxis("Right Joystick X");
        float controllerRotY = Input.GetAxis("Right Joystick Y");

        if(controllerRotX > joystickDeadzone || controllerRotX < -joystickDeadzone) rotationX = controllerRotX * joystickSensitivity;
        if(controllerRotY > joystickDeadzone || controllerRotY < -joystickDeadzone) rotationY = controllerRotY * joystickSensitivity;

        Vector2 aimDirection = new Vector2(rotationX , rotationY);
        if(aimDirection != Vector2.zero)
        {

        }
    }

    void Scout()
    {
        // left trigger
    }

    void Interact()
    {

    }

    void Fire()
    {
        // right trigger
    }
}

// for aim direction, use the vector supplied by the right thumbstick (make sure there is next to no deadzone)
// get it to hold at that angle
*/