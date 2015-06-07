using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// --------------------------------------------------------------------------
//! @brief The instantaneous (per frame) attributes of the PC character.
//! @note Try to constrain all Input.Get*() requests to this public class' Update method.
public abstract class EntityInputHandler : MonoBehaviour
{
	// --------------------------------------------------------------------------
	// ENUMS

	public enum Direction
	{
		Forward,
		Back,
		Right,
		Left,
	};

	// --------------------------------------------------------------------------
	// DATA MEMBERS

	public bool isThrusting = false;
	public float strafe = 0.0f;
	public Vector3 direction = Vector3.zero;
	public bool isBraking = false;
	public bool isFiringPrimaryWeapon = false;
	public bool isFiringSecondaryWeapon = false;
	public InputManager inputManager = null;
	public EntityManager entity = null;

	// --------------------------------------------------------------------------
	// PROPERTIES

	public InputManager.ModeType currentMode
	{ 
		get { return this.inputManager.CurrentMode; }
	}
	public bool isStrafing
	{ 
		get { return this.strafe != 0.0f; }
	}

	// --------------------------------------------------------------------------
	// METHODS

	public virtual void Reset()
	{
		this.direction = Vector3.zero; 
		this.isFiringPrimaryWeapon = false;
		this.isFiringSecondaryWeapon = false;
		this.isBraking = false;
		this.isThrusting = false;
		this.strafe = 0.0f;
	}

	public abstract void ManagedUpdate();
} // end public class EntityInputHandler
