using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAiInputHandler : EntityInputHandler
{
	// --------------------------------------------------------------------------
	// DATA MEMBERS
	
	public Rigidbody m_rigidbody = null;
	private AiPlayerData m_data = null;
	private List<Steering> m_steerings = new List<Steering>();
	private Steering m_currentSteering = null;

	// --------------------------------------------------------------------------
	// PROPERTIES
	
	public AiPlayerData data
	{
		get { return this.m_data; }
		set { this.m_data = value; }
	}
	
	public List<Steering> steerings
	{
		get { return this.m_steerings; }
		set { this.m_steerings = value; }
	}
	
	public GameObject target
	{
		get
		{
			return GameManager.Instance.FindInstanceByTag( this.m_data.game.target );
		}
	}

	// --------------------------------------------------------------------------
	// METHODS
	
	public void Start()
	{
	}

	public void OnDisable()
	{
		this.StopAllCoroutines();
	}
	
	public override void ManagedUpdate()
	{
		if( this.m_currentSteering == null )
		{
			this.StartCoroutine( this.DoSteering() );
		}
		
		if( !this.isFiringPrimaryWeapon && this.CanAttack() )
		{
			this.StartCoroutine( this.DoAttack() );
		}
	}
	
	public IEnumerator DoSteering() 
	{ 
		float elapsed = 0.0f;
		float duration = UnityEngine.Random.value * this.m_data.game.steeringPersistence;
		this.m_currentSteering = this.SelectNextSteeringBehavior();
		
		while( elapsed < duration )
		{
			Vector3 force = Vector3.zero;
			this.m_currentSteering.Vehicle = this.entity as Vehicle;
			if( !this.m_currentSteering.enabled )
			{
				break;
			}
			
			// force += steering.WeighedForce;
			force = this.m_currentSteering.Force;
			force = this.m_rigidbody.transform.InverseTransformPoint( force );
			force = force.normalized;
			
			this.direction.x = force.x;
			this.direction.z = force.z;
			this.isThrusting = ( this.direction.z > 0.0f );
			this.isBraking = ( this.direction.z < 0.0f );
			
			yield return null;
			
			elapsed += Time.deltaTime;
		} // end while
		
		this.m_currentSteering = null;
	}
	
	private Steering SelectNextSteeringBehavior()
	{
		float randomValue = UnityEngine.Random.value;
		int index = (int)Mathf.Round( randomValue * (float)( this.m_steerings.Count - 1 ) );
		Steering steering = this.m_steerings[ index ];
		// /// Debug.Log( "New steering: " + steering );
		steering.Vehicle = this.entity as Vehicle;
		return steering;
	}
	
	private bool CanAttack()
	{
		RaycastHit hitInfo;
		Physics.Raycast( 
			new Ray( this.transform.position, this.transform.forward ),
			out hitInfo,
			this.data.game.threatDistance );
		if( hitInfo.rigidbody != null )
		{
			if( hitInfo.rigidbody.gameObject == this.target )
			{
				return true;
			}
		}
		return false;
	}
	
	private IEnumerator DoAttack()
	{
		float elapsed = 0.0f;
		float duration = UnityEngine.Random.value * this.m_data.game.attackPersistence;
		this.isFiringPrimaryWeapon = true;
		
		while( elapsed < duration )
		{
			yield return null;
			elapsed += Time.deltaTime;
		} // end while
		
		this.isFiringPrimaryWeapon = false;
	}
	
	/*
	// --------------------------------------------------------------------------
	//! @brief 
	public IEnumerator Wait(AiPlayerBehaviour behaviour) 
	{ 
		float numSeconds = (UnityEngine.Random.value * (behaviour.frequency.y - behaviour.frequency.x)) + behaviour.frequency.x;
		yield return new WaitForSeconds(numSeconds);

		this.StartCoroutine( this.Execute(behaviour) );
	}

	// --------------------------------------------------------------------------
	//! @brief 
	public IEnumerator Execute(AiPlayerBehaviour behaviour) 
	{ 
		foreach( InputManager.ActionType action in behaviour.actions )
		{
			switch( action )
			{
			    case InputManager.ActionType.Horizontal:
			    	this.direction.x = (UnityEngine.Random.value * 2.0f) - 1.0f; //! @note [-1.0, 1.0]
				    // /// Debug.Log( "AI Input, Horizontal: " + this.direction.x );
			    	break;
			    case InputManager.ActionType.Vertical:
			    	this.direction.z = (UnityEngine.Random.value * 2.0f) - 1.0f; //! @note [-1.0, 1.0]
			    	break;
			    case InputManager.ActionType.Thrust:
			    	// /// Debug.Log ("Start thrust...");
			    	this.isThrusting = true;
			    	break;
			    case InputManager.ActionType.Brake:
			    	break;
			    case InputManager.ActionType.PrimaryWeapon:
	                // /// Debug.Log ("Firing primary weapon...");
			    	this.isFiringPrimaryWeapon = true;
			    	break;
			    case InputManager.ActionType.SecondaryWeapon:
			    	break;
			    case InputManager.ActionType.Strafe:
			    	break;
				case InputManager.ActionType.Unknown:
					break;
			    case InputManager.ActionType.Count:
					break;
			    default:
			    	break;
			} // end switch
		} // end foreach

		float numSeconds = (UnityEngine.Random.value * (behaviour.duration.y - behaviour.duration.x)) + behaviour.duration.x;
		yield return new WaitForSeconds(numSeconds);
		
		foreach( InputManager.ActionType action in behaviour.actions )
		{
			switch( action )
			{
			    case InputManager.ActionType.Horizontal:
			    	this.direction.x = 0.0f;
			    	break;
			    case InputManager.ActionType.Vertical:
			    	this.direction.z = 0.0f;
			    	break;
			    case InputManager.ActionType.Thrust:
			    	// /// Debug.Log ("Stop thrust...");
			    	this.isThrusting = false;
			    	break;
			    case InputManager.ActionType.Brake:
			    	break;
			    case InputManager.ActionType.PrimaryWeapon:
			    	this.isFiringPrimaryWeapon = false;
			    	break;
			    case InputManager.ActionType.SecondaryWeapon:
			    	break;
			    case InputManager.ActionType.Strafe:
			    	break;
				case InputManager.ActionType.Unknown:
					break;
			    case InputManager.ActionType.Count:
					break;
			    default:
			    	break;
			} // end switch
		} // end foreach 
		
		// Wait for and then execute the next round of AI behaviour
		this.StartCoroutine( this.Wait(behaviour) );
	}
	*/
} // public class EntityAiInputHandler
