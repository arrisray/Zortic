using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputCondition : GameRuleCondition
{
	// --------------------------------------------------------------------------
    // MEMBERS
	
	public UnityEngine.Object entity = null;
	public InputManager.ActionType action = InputManager.ActionType.Unknown;
	
	// --------------------------------------------------------------------------
    // PROPERTIES
	
	// --------------------------------------------------------------------------
    // METHODS
	
	public override void OnLoad()
    {
		MessageManager.Instance.RegisterMessageHandler<InputEventMessage>( this.OnInputReceived );
    } 
   
    public override void OnUnload()
    {
		MessageManager.Instance.UnregisterMessageHandler<InputEventMessage>( this.OnInputReceived );
    }

	public override void CreateInputs()
	{
	}

	public override void CreateOutputs()
	{
	}
	
	public void OnInputReceived( Message message )
    {
    	InputEventMessage derivedMessage = message as InputEventMessage;
        GameObject[] instances = GameManager.Instance.FindInstancesByTag( this.entity );
		if( instances == null ) 
		{
			/// Debug.LogWarning ( "Unable to find instance by tag: " + this.entity );
			return; 
		}
		
		foreach( GameObject instance in instances )
		{
			if( instance == derivedMessage.inputHandler.entity.gameObject )
			{
				this.isSatisfied = this.IsInputActivated( derivedMessage.inputHandler );
				return;
			}
		}
    }
	
	protected bool IsInputActivated( EntityInputHandler inputHandler )
	{
		switch( this.action )
		{
			case InputManager.ActionType.Brake:
			{
				return inputHandler.isBraking;
			}
			case InputManager.ActionType.Thrust:
			{
				return inputHandler.isThrusting;
			}
			case InputManager.ActionType.PrimaryWeapon:
			{
				return inputHandler.isFiringPrimaryWeapon;
			}
			case InputManager.ActionType.SecondaryWeapon:
			{
				return inputHandler.isFiringSecondaryWeapon;
			}
			case InputManager.ActionType.Strafe:
			{
				return inputHandler.isStrafing;
			}
			case InputManager.ActionType.Horizontal:
			{
				return inputHandler.direction.x != 0.0f;
			}
			case InputManager.ActionType.Vertical:
			{
				return inputHandler.direction.z != 0.0f;
			}
			default: 
			{
				break;
			}
		}
		return false;
	}
}

