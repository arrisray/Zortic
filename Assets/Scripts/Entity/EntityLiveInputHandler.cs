using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityLiveInputHandler : EntityInputHandler
{
	// --------------------------------------------------------------------------
	// DATA MEMBERS

	// --------------------------------------------------------------------------
	// PROPERTIES

	// --------------------------------------------------------------------------
	// METHODS

	//! @brief Handles updating character attributes per-frame from player input.
	//! @note Should be called from a MonoBehaviour's OnGUI() method because Unity's Event object is used.
	public override void ManagedUpdate()
	{
		if( this.enabled )
		{
			// Check for any pending input for all of known game events
			// switch (this.m_inputManager.CurrentMode)
			// {
				// case InputManager.ModeType.Default:
					this.direction = new Vector3(cInput.GetAxisRaw("Horizontal"), 0.0f, cInput.GetAxisRaw("Vertical"));
					this.isFiringPrimaryWeapon = cInput.GetButton("PrimaryWeapon");
					this.isFiringSecondaryWeapon = cInput.GetButton("SecondaryWeapon");
					this.isBraking = (cInput.GetAxisRaw("Vertical") < 0.0f);
					this.isThrusting = (cInput.GetAxisRaw("Vertical") > 0.0f);
					float strafeValue = cInput.GetButton("Strafe") ? 1.0f : 0.0f;
					this.strafe = strafeValue * cInput.GetAxisRaw("Horizontal");
					// break;
				/*
				// @note Input interpreted in world space.
				case InputManager.ModeType.Gamepad:
					this.m_direction = new Vector3(Input.GetAxis("Mac-Gamepad-Horizontal"), 0.0f, Input.GetAxis("Mac-Gamepad-Vertical"));
					this.m_isFiringPrimaryWeapon = (Input.GetAxis("Mac-Gamepad-PrimaryWeapon") > 0);
					this.m_isFiringSecondaryWeapon = (Input.GetAxis("Mac-Gamepad-SecondaryWeapon") > 0);
					this.m_isBraking = Input.GetButton("Mac-Gamepad-Brake");
					this.m_isThrusting = Input.GetButton("Mac-Gamepad-Thrust");
					this.m_strafe = Input.GetAxis("Mac-Gamepad-Strafe");
					break;
				*/
			// }
		}
		
		// Raise input event
		MessageManager.Instance.RaiseEvent( new InputEventMessage( this ) );
	}
	
	public override void Reset()
	{
		base.Reset ();
	}
	
	public void UpdateControlMode( EntityManager.ControlMode controlMode )
	{
		this.Reset ();
		switch( controlMode )
		{
			case EntityManager.ControlMode.Animation:
			{
				this.enabled = false;
				break;
			}
			case EntityManager.ControlMode.PlayerInput:
			default:
			{
				this.enabled = true;
				break;
			}
		}
	}
} 
