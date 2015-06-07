using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAtEntityEvent : GameRuleEvent
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS
	
	public bool allowEntityMovement = true;
	public int limit = 0; //! @note 0 = No spawn limit.	
    public UnityEngine.Object spawnEntity = null;
	public UnityEngine.Object locationEntity = null;
	public Vector3 spawnOffset = Vector3.zero;
	private const float TWO_PI = Mathf.PI * 2.0f;
	private List<GameObject> m_instances = new List<GameObject>();
	private int m_numPendingRequests = 0;

	// --------------------------------------------------------------------------
    // PROPERTIES
	
	private int spawnCount
	{
		get
		{
			// Should we spawn one or more entities?
			int count = ( this.m_numPendingRequests > 0 ) ? this.m_numPendingRequests : 1;
			
			// Should we limit the number of entities to spawn?
			if( this.limit > 0 ) 
			{
				int allowance = this.limit - this.m_instances.Count;
				count = Mathf.Min( count, allowance );
			}
			return count;
		}
	}

	// --------------------------------------------------------------------------
    // METHODS
	
	public override void Reset()
	{
		this.m_instances.Clear ();
	}

    public override void OnLoad()
    {
		MessageManager.Instance.RegisterMessageHandler<EntityDestroyedMessage>( this.OnEntityDestroyed );
    }
	
	public override void OnUnload()
    {
		this.m_numPendingRequests = 0;
		this.m_instances.Clear();
    }

	public override void CreateInputs()
	{
	}

	public override void CreateOutputs()
	{
	}
	
	public override void OnUpdate() 
    {   
		// Service pending spawn requests that are eligible
		if( (this.m_numPendingRequests > 0) && (this.m_instances.Count < this.limit) )
		{
			this.OnRaise();
		}
    }
    
    public override void OnRaise()
    {
		// Check for spawn limit
		if( this.limit > 0 )
		{
			if( this.m_instances.Count > this.limit )
			{
				/// Debug.LogError( "Spawn limit constraint is broken for event: " + this.name );
				return;
			}
			else if( this.m_instances.Count == this.limit )
			{
				++this.m_numPendingRequests;
				// /// Debug.LogWarning( "Spawn limit is in effect; incrementing spawn request count (=" + this.m_numPendingRequests + "." );
				return;
			}
		}
		
		this.m_numPendingRequests -= this.spawnCount;
        for( int i = 0; i < this.spawnCount; ++i )
        {
			// Get the prefab to spawn
			GameObject prefab = GameManager.Instance.FindPrefabByTag( this.spawnEntity );
			if( !prefab )
			{
				/// Debug.LogWarning( "'" + this.id + "' game event failed to spawn an entity." );
				continue;
			}

            // Spawn entity
			// /// Debug.Log( "Spawning prefab (=" + prefab + ") from tag ([" + this.spawnEntity + "])." ); 
			this.SpawnEntity( prefab );
        }
		
		this.OnComplete();
    }

	protected void SpawnEntity( GameObject prefab )
    {
        // Position
        Vector3 instancePosition = this.GetSpawnPosition();

        // Rotation
		Quaternion instanceRotation = Quaternion.identity;
		
		// Spawn instance
        GameObject instance = GameManager.Instance.UnityInstantiate(prefab, instancePosition, instanceRotation) as GameObject;
		this.m_instances.Add( instance );
		// /// Debug.Log ("Spawned entity: " + instance.name );
		
		// Init instance movement params
        EntityManager entity = instance.GetComponent<EntityManager>();
        if( entity && entity.data )
        {
            entity.data.movement.initialPosition = instancePosition;
            entity.data.movement.initialSpeedRange = Vector2.zero;
            entity.data.movement.initialSpinForceRange = Vector2.zero;
            entity.data.movement.initialSpinAxis = Vector3.up;
			if( this.allowEntityMovement )
			{
            	entity.InitMovement();
			}
        }
    }

	protected Vector3 GetSpawnPosition()
	{
		// Get the instance to spawn
		GameObject instance = GameManager.Instance.FindInstanceByTag( this.locationEntity );
		if( !instance )
		{
			/// Debug.LogWarning( "Unable to find SpawnAtEntityEvent's location entity: " + this.locationEntity );
			return Vector3.zero;
		}
		
		Vector3 instancePosition = instance.transform.TransformPoint( this.spawnOffset );
		// /// Debug.Log( "Spawn position: " + instancePosition );
		return instancePosition;
	}
	
	public void OnEntityDestroyed( Message message )
	{
		EntityDestroyedMessage derivedMessage = message as EntityDestroyedMessage;
		if( this.m_instances.Contains ( derivedMessage.victim ) )
		{
			this.m_instances.Remove( derivedMessage.victim );
		}
	}
} // class SpawnEntityEvent 
