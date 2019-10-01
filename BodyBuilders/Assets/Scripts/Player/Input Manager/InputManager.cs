using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{
    // Axis //============================================================================
    public static bool gamepad;

    public static float JoystickLeftHorizontal() // right joystick horizontal
    {
        if(Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Abs(Input.GetAxis("Left Joystick Horizontal"))) gamepad = false;
        else gamepad = true;
        return Mathf.Clamp(Input.GetAxis("Left Joystick Horizontal") + Input.GetAxis("Horizontal") + Input.GetAxis("D Pad Horizontal"), -1 , 1);
    }

    public static float JoystickLeftVertical() // right joystick vertical
    {
        if(Mathf.Abs(Input.GetAxis("Vertical")) > Mathf.Abs(Input.GetAxis("Left Joystick Vertical"))) gamepad = false;
        else gamepad = true;
        return Mathf.Clamp(Input.GetAxis("Left Joystick Vertical") + Input.GetAxis("Vertical") + Input.GetAxis("D Pad Vertical"), -1 , 1);
    }

    public static Vector2 JoystickLeft() // combined left joystick vector
    {
        return new Vector2(JoystickLeftHorizontal() , JoystickLeftVertical());
    }


    public static float JoystickLeftHorizontalRaw() // right joystick horizontal
    {
        return Mathf.Clamp(Input.GetAxisRaw("Left Joystick Horizontal") + Input.GetAxisRaw("Horizontal") + Input.GetAxisRaw("D Pad Horizontal"), -1 , 1);
    }

    public static float JoystickLeftVerticalUnclamped()
    {
        return Input.GetAxis("Left Joystick Vertical") + Input.GetAxis("Vertical") + Input.GetAxis("D Pad Vertical");
    }

    public static float JoystickLeftVerticalRaw() // right joystick vertical
    {
        return Mathf.Clamp(Input.GetAxisRaw("Left Joystick Vertical") + Input.GetAxisRaw("Vertical") + Input.GetAxisRaw("D Pad Vertical"), -1 , 1);
    }

    public static Vector2 JoystickLeftRaw() // combined left joystick vector
    {
        return new Vector2(JoystickLeftHorizontalRaw() , JoystickLeftVerticalRaw());
    }

// ======================================================================================

    public static float previousJoystickRightHorizontal = 0f;

    public static float JoystickRightHorizontal() // right joystick horizontal
    {
        if(Mathf.Clamp(Input.GetAxis("Right Joystick Horizontal"), -1 , 1) != 0)
        {
            previousJoystickRightHorizontal = Mathf.Clamp(Input.GetAxis("Right Joystick Horizontal"), -1 , 1);
            return previousJoystickRightHorizontal;
        }
        else return previousJoystickRightHorizontal;
    }

    public static float previousJoystickRightVertical = 0f;

    public static float JoystickRightVertical() // right joystick vertical
    {
        if(Mathf.Clamp(Input.GetAxis("Right Joystick Vertical"), -1 , 1) != 0)
        {
            previousJoystickRightVertical = Mathf.Clamp(Input.GetAxis("Right Joystick Vertical"), -1 , 1);
            return previousJoystickRightVertical;
        }
        else return previousJoystickRightVertical;
    }

    public static Vector2 JoystickRight() // combined right joystick vector
    {
        return new Vector2(JoystickRightHorizontal() , JoystickRightVertical());
    }

    // Buttons //=========================================================================

    public static bool ButtonA() // Return true while A is held
    {
        return Input.GetButton("A Button");
    }

    public static bool ButtonADown() // Return true on the frame that button A is pressed
    {
        return Input.GetButtonDown("A Button");
    }

    public static bool ButtonB() // Return true while B is held
    {
        return Input.GetButton("B Button");
    }

    public static bool ButtonBDown() // Return true on the frame that button B is pressed
    {
        return Input.GetButtonDown("B Button");
    }

    public static bool ButtonX() // Return true while X is held
    {
        return Input.GetButton("X Button");
    }

    public static bool ButtonXDown() // Return true on the frame that button X is pressed
    {
        return Input.GetButtonDown("X Button");
    }

    public static bool ButtonY() // Return true while Y is held
    {
        return Input.GetButton("Y Button");
    }

    public static bool ButtonYDown() // Return true on the frame that button Y is pressed
    {
        return Input.GetButtonDown("Y Button");
    }


    public static bool wasTriggerLeft = false;

    public static bool TriggerLeft() // Return true while trigger left is held
    {
        if(Input.GetAxisRaw("Trigger Left") != 0)
        {
            if(!wasTriggerLeft)
            {
                wasTriggerLeft = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            wasTriggerLeft = false;
            return false;
        }
    }

    public static bool wasTriggerRight = false;

    public static bool TriggerRight() // Return true while the trigger right is held
    {
        if(Input.GetAxisRaw("Trigger Right") != 0)
        {
            if(!wasTriggerRight)
            {
                wasTriggerRight = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            wasTriggerRight = false;
            return false;
        }
    }

    public static bool BumperLeft() // Return true while bumper left is held
    {
        return Input.GetButton("Bumper Left");
    }

    public static bool BumperLeftDown() // Return true on the frame that bumper left is pressed
    {
        return Input.GetButtonDown("Bumper Left");
    }

    public static bool BumperRight() // Return true while bumper right is held
    {
        return Input.GetButton("Bumper Right");
    }

    public static bool BumperRightDown() // Return true on the frame that bumper right is pressed
    {
        return Input.GetButtonDown("Bumper Right");
    }

    public static bool StartDown() // Return true on the frame that the start button is pressed
    {
        return Input.GetButtonDown("Start Button");
    }

    public static bool SelectDown() // Return true on the frame that the select button is pressed
    {
        return Input.GetButtonDown("Select Button");
    }

    // Specified Inputs //==================================================================

    public static bool ToggleScout()
    {
        return TriggerLeft();
    }

    public static bool Detach()
    {
        if(ButtonBDown() || Input.GetMouseButtonDown(0)) return true;
        else return false;
    }

    public static bool Jump()
    {
        if(JoystickLeftRaw().y > 0.75f || ButtonA()) return true;
        else return false;
    }

    public static bool Cast()
    {
        if(TriggerRight() || Input.GetMouseButtonDown(1)) return true;
        else return false;
    }

    public static bool Climb()
    {
        if(JoystickLeft().y > 0.2f || ButtonA()) return true;
        else return false;
    }
}
