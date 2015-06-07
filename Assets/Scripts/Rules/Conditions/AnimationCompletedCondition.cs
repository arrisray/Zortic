using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationCompletedCondition : GameRuleCondition
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS
	
	public EntityTag entity = null;
    public string animation = null;
	private List<AnimationState> m_registeredAnimationEvents = new List<AnimationState>();
	
	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS
	
	private void OnEntityAnimationEventTriggered( Message message )
	{
		EntityAnimationEventMessage derivedMessage = message as EntityAnimationEventMessage;
		// /// Debug.Log ( "EntityAnimationEvent Called: " + this.name + ", " + derivedMessage.entity );
		EntityManager entity = derivedMessage.entity.GetComponent<EntityManager>();
		if( (entity == null) || (this.animation != derivedMessage.animation) )
		{
			return;
		}
		
		GameObject go = entity.gameObject;
		AnimationState clip = go.animation[derivedMessage.animation];

		// /// Debug.Log ( "EntityAnimationEvent Triggered: " + clip.name + ", " + clip.time + "/" + clip.length );
		if( entity.entityTags.Contains( this.entity ) && (clip.time >= clip.length) )
		{
			this.isSatisfied = true;
		}
	}
	
	private void OnEntitySetup( Message message )
	{
		EntitySetupMessage derivedMessage = message as EntitySetupMessage;
		EntityManager entity = derivedMessage.entity.GetComponent<EntityManager>();
		if( entity == null )
		{
			return;
		}
		
		if( entity.entityTags.Contains( this.entity ) )
		{
			GameObject gameObject = entity.gameObject;
			AnimationState animationState = gameObject.animation[this.animation];
			if( !this.m_registeredAnimationEvents.Contains( animationState ) )
			{
				AnimationEvent animationEvent = new AnimationEvent();
				animationEvent.time = animationState.length;
				animationEvent.functionName = "OnAnimationEvent";
				animationEvent.stringParameter = this.animation;
				animationState.clip.AddEvent( animationEvent );
				this.m_registeredAnimationEvents.Add ( animationState );
				// /// Debug.Log ( "Adding event " + animationEvent.functionName + " to animation " + this.animation );
			}
		}
	}

    public override void OnLoad()
    {
		MessageManager.Instance.RegisterMessageHandler<EntitySetupMessage>( this.OnEntitySetup );
		MessageManager.Instance.RegisterMessageHandler<EntityAnimationEventMessage>( this.OnEntityAnimationEventTriggered );
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
} // class EventCompletedCondition

