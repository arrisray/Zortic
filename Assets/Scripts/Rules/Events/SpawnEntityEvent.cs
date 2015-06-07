using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//! @brief Spawning an entity consists of the following steps:
//  1. Choose a random 3D vector within a unit sphere. 
//  2. The x-z vector components will determine where the entity is spawned 
//     just outside of the screen border.
//  3. The entity will (by default) be oriented to face the screen center point.
//     The y vector component will determine how much off of center the entity
//     will orient. The [cos(y), sin(y)] * scalar value will yield a 2D offset
//     from the center point.
//  4. 
public class SpawnEntityEvent : GameRuleEvent
{
	// --------------------------------------------------------------------------
    // ENUMS
	
	public enum CoordinateSpace
	{
		World,
		View
	}
	
	// --------------------------------------------------------------------------
    // DATA MEMBERS
	
	public int limit = 0; //! @note 0 = No spawn limit.	
    public UnityEngine.Object entity = null;
    public Vector2 initialSpeedRange = Vector2.zero;
    public Vector2 initialSpinForceRange = Vector2.zero;
	public bool isStaticPosition = false;
	public CoordinateSpace coordinateSpace = CoordinateSpace.View;
	public Vector3 initialPosition = Vector3.zero;
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
		Port port = new Port();
		port.id = "position";
		port.type = EditorDataType.Vector3;
		this.m_inputs.Add( port );
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
			GameObject prefab = GameManager.Instance.FindPrefabByTag( this.entity );
			if( !prefab )
			{
				/// Debug.LogWarning( "'" + this.id + "' game event failed to spawn an entity." );
				continue;
			}

            // Spawn entity
			// /// Debug.Log( "Spawning prefab (=" + prefab + ") from tag ([" + spawnIndex + "]=" + this.entities[spawnIndex] + ")." ); 
			this.SpawnEntity( prefab );
        }
		
		this.OnComplete();
    }

	protected void SpawnEntity( GameObject prefab )
    {
        // Local vars
        Vector3 r = UnityEngine.Random.onUnitSphere;

        // Position
        Vector3 instancePosition = this.GetSpawnPosition();

        // Rotation
		Quaternion instanceRotation = Quaternion.identity;
		if( !this.isStaticPosition )
		{
	        float maxScreenDimension = Mathf.Max( Screen.height, Screen.width ); //! @note Handle cases where screen orientation may change...
	        Vector3 lookAtPosition = new Vector3( Mathf.Cos(r.y * TWO_PI), 0.0f, Mathf.Sin(r.y * TWO_PI) ); 
	        lookAtPosition *= ( UnityEngine.Random.value * maxScreenDimension * 0.25f );
	        instanceRotation = Quaternion.LookRotation( lookAtPosition - instancePosition );
		}

        GameObject instance = GameManager.Instance.UnityInstantiate(prefab, instancePosition, instanceRotation) as GameObject;
		this.m_instances.Add ( instance );
		// /// Debug.Log ("Spawned entity: " + instance.name );
		
        EntityManager entity = instance.GetComponent<EntityManager>();
        if( entity && entity.data )
        {
            // Speed
            float instanceSpeed = UnityEngine.Random.value * ( (this.initialSpeedRange.y - this.initialSpeedRange.x) + this.initialSpeedRange.x );

            // Spin
            float spinForce = ( Mathf.Abs(r.z) * (initialSpinForceRange.y  - initialSpinForceRange.x) ) + initialSpinForceRange.x;
            Vector3 instanceSpinAxis = UnityEngine.Random.onUnitSphere; 

            // Apply entity movement params
            entity.data.movement.initialPosition = instancePosition;
            entity.data.movement.initialSpeedRange = new Vector2( instanceSpeed, instanceSpeed );
            entity.data.movement.initialSpinForceRange = new Vector2( spinForce, spinForce );
            entity.data.movement.initialSpinAxis = instanceSpinAxis;
            entity.InitMovement();
        }
    }

	protected Vector3 GetSpawnPosition()
	{
		Vector3 instancePosition = Vector3.zero;
		if( this.isStaticPosition )
		{
			switch( this.coordinateSpace )
			{
				case CoordinateSpace.World:
				{
					instancePosition = this.initialPosition;
					break;
				}
				case CoordinateSpace.View:
				default:
				{
					instancePosition = GameManager.Instance.sceneCamera.ViewportToWorldPoint( this.initialPosition );
					break;
				}
			}
		}
		else if( !string.IsNullOrEmpty( this["position"].data ) )
		{
			instancePosition = Utils.ToVector3( this["position"].data );
			// /// Debug.Log( "...from input!" );
		}
		else
		{
			// Local vars
	        Vector3 r = UnityEngine.Random.onUnitSphere;
	
			float fudgeFactor = 0.5f;
			Ray borderRay = new Ray( Vector3.zero, new Vector3(Mathf.Cos(r.x * TWO_PI), 0.0f, Mathf.Sin(r.x * TWO_PI)) );
	        Bounds bounds = new Bounds( Vector3.zero, new Vector3(Screen.width * fudgeFactor, 0.0f, Screen.height * fudgeFactor) );
	
	        float intersectDistance = 0.0f;
	        if( bounds.IntersectRay( borderRay, out intersectDistance ) )
	        {
	            instancePosition = borderRay.GetPoint( intersectDistance );
	        }
			// /// Debug.Log( "...from random!" );
		}
		
		// /// Debug.Log( "Spawn position: " + instancePosition );
		return instancePosition;
	}

    protected Vector2 OffsetHeading(float value)
    {
        float c = UnityEngine.Random.value;
        Vector2 v = new Vector2( Mathf.Cos(value), Mathf.Sin(value) );
        return (v * c);
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
