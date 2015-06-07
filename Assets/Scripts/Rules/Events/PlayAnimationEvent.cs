using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimationEvent : GameRuleEvent
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

	// --------------------------------------------------------------------------
    // DATA MEMBERS
	
	public EntityTag entity = null;
	public string path = string.Empty;
    public string animation = null;
	public bool forceThrustWhileAnimating = false;

	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS
	
	public override void Reset()
	{
		GameManager.Instance.StopCoroutine( "PlayAnimation" );
	}

    public override void OnLoad()
    {
		
    } 
   
    public override void OnUnload()
    {
    }

    public override void CreateInputs()
    {
    }

    public override void CreateOutputs()
    {
    }

    public override void OnRaise()
    {
		GameObject instance = GameManager.Instance.FindInstanceByTag( this.entity );
		if( instance != null )
		{
			if( path != string.Empty )
			{
				Transform child = instance.transform.Find ( path );
				if( child == null )
				{
					/// Debug.LogError( "Unable to find child (\"" + path + "\") of entity \"" + instance.name + "\"." );
					return;
				}
				instance = child.gameObject;
			}
			
			GameManager.Instance.StartCoroutine( this.PlayAnimation( instance ) );
		}
    }
	
	public IEnumerator PlayAnimation( GameObject instance )
	{
		// /// Debug.Log ( "PlayAnimationEvent: " + this.name + ", tag=" + this.entity.name + ", entity=" + instance.name );
		
		EntityManager entity = instance.GetComponent<EntityManager>();
		if( entity )
		{
			entity.controlMode = EntityManager.ControlMode.Animation;
			
			//! @note Simulate live input engage forward thrusters.
			if( this.forceThrustWhileAnimating ) 
			{
				entity.inputHandler.direction = Vector3.forward;
			}
		}
		
		// Play animation
		AnimationState animationState = instance.animation[ this.animation ];
    	instance.animation.Play( this.animation );
		while( animationState.normalizedTime < 1.0f ) 
		{
			if( instance == null )
			{
				yield break;
			}
			yield return null; 
		}
		
		// Reset animation state
		instance.animation.Stop ();
		animationState.weight = 0.0f;
		animationState.normalizedTime = 0.0f;
		
		if( entity )
		{
			entity.controlMode = EntityManager.ControlMode.PlayerInput;
			if( this.forceThrustWhileAnimating ) 
			{
				//! @note Simulate live input to disengage all thrusters.
				entity.inputHandler.direction = Vector3.zero;
			}
		}
		
		this.OnComplete();
	}
}

