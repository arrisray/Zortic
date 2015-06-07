using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(MeshFilter) )]
[RequireComponent( typeof(MeshRenderer) )]
public class EntityThruster : EntityAttachment
{
	// --------------------------------------------------------------------------
    // ENUM
	
	public enum EventType
	{
		Idle,
		StartThrust,
		EndThrust,
		StartStabilize,
		EndStabilize,
	}
	
    // --------------------------------------------------------------------------
    // DATA MEMBERS
	
	private volatile bool m_isStabilizing = false;
    private EntityManager m_entity = null;
    private EntityThrusterData m_data = null;

    // --------------------------------------------------------------------------
    // PROPERTIES

    public EntityInputHandler inputHandler 
	{ 
		get { return this.m_entity.inputHandler; } 
	}

    // --------------------------------------------------------------------------
    // METHODS

    public void Init(EntityManager entity, EntityThrusterData data)
    {
    	this.m_entity = entity;
    	this.m_data = data;

    	this.Engage(false);
    }

    public void ManagedUpdate()
    {
		if( this.m_isStabilizing )
		{
			return;
		}

		List<Utils.Direction> directions = new List<Utils.Direction>();
		if( this.m_entity.inputHandler.isStrafing )
		{
			directions.Add( this.m_data.strafeDirection );
		}
		else
		{
			directions.AddRange( this.m_data.thrustDirection );
		}
			
		bool isEngaged = false;
		for( int i = 0; i < directions.Count; ++i )
		{
			Utils.Direction activeDirection = directions[i];
			float normalizedDirectionalMagnitude = this.m_entity.navigation.GetMagnitudeInDirection( activeDirection );

	        isEngaged = ((int)activeDirection & this.m_entity.navigation.currentDirection) != 0;
	        isEngaged = isEngaged 
				&& ( normalizedDirectionalMagnitude >= this.m_data.activationSpeedRange.x ) 
				&& ( normalizedDirectionalMagnitude <= this.m_data.activationSpeedRange.y );
			// /// Debug.Log( this.name + ": " + activeDirection + " & " + this.m_entity.navigation.currentDirection + " = " + isEngaged );
			if( isEngaged )
			{
				break;
			}
		}
		
        this.Engage( isEngaged );
    }

	public void Stabilize()
	{
		if( this.m_data.isStabilizer && !this.m_isStabilizing )
		{
			this.m_isStabilizing = true;
			StartCoroutine( this.EngageStabilizer() );
		}
	}

    private void Engage(bool value)
    {
        this.gameObject.renderer.enabled = value;
		if( this.m_data.particles != null )
		{
			this.m_data.particles.enableEmission = value;
		}
		
		// Emit event message
		if( this.m_data.isAudioMainThruster )
		{
			EntityThruster.EventType eventType = value ? EntityThruster.EventType.StartThrust : EntityThruster.EventType.EndThrust;
			MessageManager.Instance.RaiseEvent( new EntityThrusterEventMessage( this.m_entity.gameObject, eventType ) ); 
		}
    }

	private IEnumerator EngageStabilizer()
	{
		this.m_isStabilizing = this.gameObject.renderer.enabled = true;
		if( this.m_data.isAudioRetroThruster )
		{
			MessageManager.Instance.RaiseEvent( new EntityThrusterEventMessage( this.m_entity.gameObject, EntityThruster.EventType.StartStabilize ) ); 
		}
		// /// Debug.Log( "[" + Time.realtimeSinceStartup + "] " + this.name + ": Start stabilization..." );

		yield return new WaitForSeconds( this.m_data.stabilizeDuration );
		
		this.m_isStabilizing = this.gameObject.renderer.enabled = false;
		if( this.m_data.isAudioRetroThruster )
		{
			MessageManager.Instance.RaiseEvent( new EntityThrusterEventMessage( this.m_entity.gameObject, EntityThruster.EventType.EndStabilize ) ); 
		}
		// /// Debug.Log( "[" + Time.realtimeSinceStartup + "] " + this.name + ": End stabilization..." );
	}
}