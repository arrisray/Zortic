using System;
using System.Collections.Generic;
using UnityEngine;

// ----------------------------------------------------------------------------
//! @brief This is a helper class used to determine what type of device we are receiving input from per-frame.
public class InputManager : Singleton<InputManager>
{
	public enum ActionType
	{
		Unknown = 0,
		Horizontal = 1<<1,
		Vertical = 1<<2,
		Thrust = 1<<3,
		Brake = 1<<4,
		PrimaryWeapon = 1<<5,
		SecondaryWeapon = 1<<6,
		Strafe = 1<<7,
		Count = 1<<8
	};

	// --------------------------------------------------------------------------
	//! @brief The types of devices we currently handle.
	public enum ModeType
	{
		Default = 0,    // Mouse & Keyboard
		Gamepad,        // Microsoft Xbox 360(-like) Controller
		AI,
		Count
	}; // enum ModeType

	// --------------------------------------------------------------------------
	//! @brief The current type of device we are expecting to receive input from.
	[HideInInspector]
	public ModeType CurrentMode;

	// --------------------------------------------------------------------------
	//! @brief Constructor.
	//! @hack ...!
	public void Setup ()
	{
		this.CurrentMode = ModeType.Default;
		this.CreateControlScheme ();
	}

	public void Teardown ()
	{
    
	}

	public void CreateControlScheme ()
	{
		if( !cInput.IsKeyDefined( "Left" ) ) cInput.SetKey ("Left", Keys.A, Keys.LeftArrow);
		if( !cInput.IsKeyDefined( "Right" ) ) cInput.SetKey ("Right", Keys.D, Keys.RightArrow);
		if( !cInput.IsKeyDefined( "Thrust" ) ) cInput.SetKey ("Thrust", Keys.W, Keys.UpArrow);
		if( !cInput.IsKeyDefined( "Brake" ) ) cInput.SetKey ("Brake", Keys.S, Keys.DownArrow);
		if( !cInput.IsKeyDefined( "PrimaryWeapon" ) ) cInput.SetKey ("PrimaryWeapon", Keys.Mouse0);
		if( !cInput.IsKeyDefined( "SecondaryWeapon" ) ) cInput.SetKey ("SecondaryWeapon", Keys.Mouse1);
		if( !cInput.IsKeyDefined( "Strafe" ) ) cInput.SetKey ("Strafe", Keys.LeftShift, Keys.RightShift);
		if( !cInput.IsKeyDefined( "Horizontal" ) ) cInput.SetAxis ("Horizontal", "Left", "Right");
		if( !cInput.IsKeyDefined( "Vertical" ) ) 
		{
			cInput.SetAxis ("Vertical", "Thrust", "Brake");
			cInput.AxisInverted ("Vertical", true);
		}

		/* @todo ...
    cInput.SetKey("Mac-Gamepad-Left", Keys.A, Keys.LeftArrow);
    cInput.SetKey("Mac-Gamepad-Right", Keys.D, Keys.RightArrow);
    cInput.SetKey("Mac-Gamepad-Thrust", Keys.W, Keys.UpArrow);
    cInput.SetKey("Mac-Gamepad-Brake", Keys.S, Keys.DownArrow);
    cInput.SetKey("Mac-Gamepad-PrimaryWeapon", Keys.MouseLeft);
    cInput.SetKey("Mac-Gamepad-SecondaryWeapon", Keys.MouseRight);
    cInput.SetKey("Mac-Gamepad-Strafe", Keys.LeftShift, Keys.RightShift);
    cInput.SetAxis("Mac-Gamepad-Horizontal", "Any-Default-Left", "Any-Default-Right");
    cInput.SetAxis("Mac-Gamepad-Vertical", "Any-Default-Thrust", "Any-Default-Brake");
    */
	}
  
	// --------------------------------------------------------------------------
	//! @brief Handles detecting the current device type from player input.
	//! @note Should be called from a MonoBehaviour's OnGUI() method because Unity's Event object is used.
	public void Update ()
	{
		/*
    switch (this.CurrentMode)
    {
      case ModeType.Default:
        this.TestGamepad();
        break;
      case ModeType.Gamepad:
        this.TestDefault();
        break;
      default:
        this.TestDefault();
        break;
    }
    */

		this.CurrentMode = ModeType.Default;
	}

	// --------------------------------------------------------------------------
	//! @brief Checks for any pending input from a mouse and/or keyboard.
	private bool TestDefault ()
	{
		// Mouse & Keyboard Buttons
		if (Event.current.isKey || Event.current.isMouse) {
			this.CurrentMode = ModeType.Default;
			return true;
		}

		// Mouse Movement
		if (cInput.GetAxis ("Horizontal") != 0.0f || cInput.GetAxis ("Vertical") != 0.0f) {
			this.CurrentMode = ModeType.Default;
			return true;
		}
		return false;
	}

	// --------------------------------------------------------------------------
	//! @brief Checks for any pending input from a Microsoft Xbox 360(-like) controller.
	private bool TestGamepad ()
	{
		// Gamepad Buttons
		if (Input.GetButton ("Thrust")
      || Input.GetButton ("Brake")) {
			this.CurrentMode = ModeType.Gamepad;
			return true;
		}

		// Gamepad Axes
		if ((Mathf.Abs (Input.GetAxis ("Horizontal")) > 0.75f)
      || (Mathf.Abs (Input.GetAxis ("Vertical")) > 0.75f)
      || (Mathf.Abs (Input.GetAxis ("PrimaryWeapon")) > 0.75f)
      || (Mathf.Abs (Input.GetAxis ("SecondaryWeapon")) > 0.75f)
      || (Input.GetAxis ("Strafe") != 0.0f)) { 
			this.CurrentMode = ModeType.Gamepad;
			return true;
		}

		return false;
	}
} // end class InputMana