using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(EntityManager) )]
public class EntityNavigation : MonoBehaviour
{
    // --------------------------------------------------------------------------
    // DATA MEMBERS

    private EntityManager m_entity = null;
    private Vector3 m_instantaneousVelocity = Vector3.zero;
    private float m_currentTurnAngle = 0.0f;
    private Quaternion m_deltaRotation = Quaternion.identity;
	private EntityBanking m_tilt = null;
	private WrapAround m_screenWrap = null;

    // --------------------------------------------------------------------------
    // PROPERTIES
	
	public EntityBanking banking { get { return this.m_tilt; } }
    public EntityData data 
	{ 
		get 
		{ 
			//! @hack No clue why entity data is sporadically inaccessible during Setup()...
			//  best guess is some kinda timing issues on Unity Instantiate()...???
			if( this.m_entity.data == null )
			{
				return this.m_entity.GetComponent<EntityData>();
			}
			return this.m_entity.data; 
		} 
	}
    public EntityInputHandler inputHandler { get { return this.m_entity.inputHandler; } }
    public Vector3 instantaneousVelocity { get { return this.m_instantaneousVelocity; } }
    public float currentSpeed { get { return this.enabled ? this.m_entity.data.rigidbody.velocity.magnitude : 0.0f; } }
	public float maxSpeed { get { return this.m_entity.data.movement.maxSpeed; } }
	public float maxAcceleration { get { return this.m_entity.data.movement.maxAcceleration; } }
	public bool isMoving 
	{ 
		get 
		{ 
			return this.enabled
				? this.m_entity.data.rigidbody.velocity.sqrMagnitude != 0.0f 
				: false; 
		} 
	}
    public bool isAccelerating { get { return this.instantaneousVelocity != Vector3.zero; } }
    public float currentTurnAngle 
    { 
        get { return this.m_currentTurnAngle; } 
        protected set { this.m_currentTurnAngle = value; }
    }
    protected Transform translationNode { get { return this.data.movement.rootNode; } }
    protected Transform rotationNode { get { return this.data.movement.rootNode; } }
	public int currentDirection
    {
        get 
        {
			int direction = (int)Utils.Direction.Unknown;

			if( this.inputHandler.direction.x > 0.0f ) 
			{
				direction |= (int)Utils.Direction.Right;
			}
			if( this.inputHandler.direction.x < 0.0f ) 
			{
				direction |= (int)Utils.Direction.Left;
			}

			if( this.inputHandler.direction.z > 0.0f ) 
			{
				direction |= (int)Utils.Direction.Forward;
			}
			if( this.inputHandler.direction.z < 0.0f ) 
			{
				direction |= (int)Utils.Direction.Reverse;
			}

			return direction;
        }
    }

    // --------------------------------------------------------------------------
    // METHODS

    public void OnEnable()
    {
    }

    // --------------------------------------------------------------------------
    //! @brief 
    public void Setup() 
    {
        this.m_entity = this.gameObject.GetComponent<EntityManager>();
		if( this.m_entity.data.rigidbody == null )
		{
			this.enabled = false;
			return;
		}

        if( this.data.movement.isBankingEnabled )
        {
            this.m_tilt = this.gameObject.AddComponent<EntityBanking>();
			this.m_tilt.Setup ();
        }

        if( this.data.movement.allowScreenWrap )
        {
            this.m_screenWrap = this.gameObject.AddComponent<WrapAround>();
        }

        //! @todo Handle init'ing more completely...
        float speed = (UnityEngine.Random.value * (this.data.movement.initialSpeedRange.y - this.data.movement.initialSpeedRange.x)) + this.data.movement.initialSpeedRange.x;
        if( this.data.movement.rigidbody )
        {
            Vector3 initialForce = Vector3.forward * speed;
            this.data.movement.rigidbody.AddRelativeForce( initialForce, ForceMode.Impulse );
        }
    }
	
	public void ManagedUpdate()
	{
		if( this.m_screenWrap != null )
		{
			this.m_screenWrap.ManagedUpdate();
		}
	}

    // --------------------------------------------------------------------------
    //! @brief 
    public void ManagedFixedUpdate()
    {
		if( this.m_tilt )
		{
			this.m_tilt.ManagedFixedUpdate();
		}

        this.UpdateVelocity();
        this.UpdateDirection();
        this.Apply();

        //! @hack Prevent translations on y-axis!
		//! @note Why isn't this being caught by constraints on the Rigidbody component?
        Vector3 position = this.data.movement.rigidbody.position;
        position.y = 0.0f; 
        this.data.movement.rigidbody.position = position;
    }

    // --------------------------------------------------------------------------
    //! @brief 
    public void Reset()
    {
        this.m_instantaneousVelocity = Vector3.zero;
        this.m_currentTurnAngle = 0.0f;
		
		this.data.movement.rigidbody.velocity = Vector3.zero;
		
		if( this.m_tilt != null )
		{
			this.m_tilt.Reset ();
		}
    }

	public float GetMagnitudeInDirection( Utils.Direction direction )
	{
		float result = 0.0f;
		switch ( direction )
		{
			case Utils.Direction.Forward:
			{
			    result = ( this.m_entity.inputHandler.direction.z > 0.0f ) ? this.m_entity.inputHandler.direction.z : 0.0f;
			    break;
			}
			case Utils.Direction.Reverse:
			{
			    result = ( this.m_entity.inputHandler.direction.z < 0.0f ) ? Mathf.Abs(this.m_entity.inputHandler.direction.z) : 0.0f;
			    break;
			}
			case Utils.Direction.Left:
			{
			    result = ( this.m_entity.inputHandler.direction.x < 0.0f ) ? Mathf.Abs(this.m_entity.inputHandler.direction.x) : 0.0f;
			    break;
			}
			case Utils.Direction.Right:
			{
			    result = ( this.m_entity.inputHandler.direction.x > 0.0f ) ? this.m_entity.inputHandler.direction.x : 0.0f;
			    break;
			}
			default: 
			{
			    break;
			}
		}
		// /// Debug.Log( direction + " amount => " + result );
		return result;
	}

    // --------------------------------------------------------------------------
    protected void Apply()
    {
		// Apply instantaneous acceleration
        if( this.m_instantaneousVelocity != Vector3.zero )
        {
			this.data.movement.rigidbody.AddForce( this.m_instantaneousVelocity * Time.fixedDeltaTime, ForceMode.Force );
            // /// Debug.Log( this.name + " applied force: " + this.m_instantaneousVelocity * Time.fixedDeltaTime );
        }
		// Special case: Snap to zero when braking
		//! @note ...only if the rigidbody is not friction-less, i.e. drag != 0
		else if( this.isMoving 
			&& !this.isAccelerating 
			&& (this.currentSpeed < this.data.movement.fullStopThreshold ) 
			&& (this.data.movement.rigidbody.drag != 0.0f) )
		{
			// /// Debug.Log( this.name );
			this.data.movement.rigidbody.velocity = Vector3.zero;
		}

		// Apply speed limit
		//! @note ...only if max speed has been set!
		if( (this.data.movement.maxSpeed != 0.0f)
			&& (this.data.movement.rigidbody.velocity.magnitude > this.data.movement.maxSpeed) )
		{
			this.data.movement.rigidbody.velocity = this.data.movement.rigidbody.velocity.normalized * this.data.movement.maxSpeed;
		}
	
		// Apply instantaneous rotation 
        this.data.movement.rigidbody.MoveRotation( this.data.movement.rigidbody.rotation * this.m_deltaRotation );
        this.m_deltaRotation = Quaternion.identity;
		
		// /// Debug.Log( this.name + ": moving=" + this.isMoving + ", accelerating=" + this.isAccelerating + ", speed=" + this.currentSpeed );
        // /// Debug.Log( this.name + ", instant-velocity=" + this.m_instantaneousVelocity + ", rigidbody-rot=" + this.data.movement.rigidbody.rotation );
    }

    // --------------------------------------------------------------------------
    //! @brief Update calculation of entity's current speed.
    //! @note Currently handling all input modes with the same approach for updating entity speed.
    public void UpdateVelocity()
    { 
        // Local vars
        float acceleration = this.m_entity.data.movement.maxAcceleration * Time.fixedDeltaTime; //! @note Variable acceleration is not currently supported!
        Vector3 thrustDirection = Vector3.zero;

        // Adjust thrust direction for special cases
        if( this.inputHandler.isThrusting )
        {
            thrustDirection = this.transform.forward;
        }
        else if( this.inputHandler.isBraking )
        {
            thrustDirection = -this.rigidbody.velocity / this.rigidbody.velocity.magnitude;
            // /// Debug.Log( thrustDirection );
            acceleration = this.m_entity.data.movement.deceleration;
        }
        else if( this.inputHandler.isStrafing )
        {
            float strafeAngle = this.m_entity.inputHandler.strafe * 90.0f;
            thrustDirection = Quaternion.AngleAxis(strafeAngle, Vector3.up) * this.transform.forward;
            // /// Debug.Log( "Strafe thrust direction: " + thrustDirection );
        }
  
        // Calculate instantaneous velocity
        //! @note [T]he unit of the force parameter is applied to the rigidbody as mass*distance/time^2.
        //! @ref http://docs.unity3d.com/Documentation/ScriptReference/ForceMode.Force.html?from=Rigidbody
        this.m_instantaneousVelocity = ( thrustDirection * acceleration * this.rigidbody.mass ) / ( Time.fixedDeltaTime * Time.fixedDeltaTime );   
    }

    // --------------------------------------------------------------------------
    //! @brief Update calculation of PC's current direction.
    public void UpdateDirection()
    { 
        this.currentTurnAngle = this.UpdateDirectionDefault();
        // /// Debug.Log( this.currentTurnAngle );
    }

    // --------------------------------------------------------------------------
    //! @brief Update calculation of PC's current direction for mouse/keyboard input.
    public float UpdateDirectionDefault() 
    {
        float deltaTime = Time.fixedDeltaTime;
        if (this.m_entity.inputHandler.direction == Vector3.zero)
        {
            return 0.0f;
        }
        if (this.m_entity.inputHandler.strafe != 0.0f)
        {
            return 0.0f; 
        }

        float forwardThrust = this.m_entity.inputHandler.direction.z;
        float rotationalThrust = this.m_entity.inputHandler.direction.x;

        var lerpFactor = (forwardThrust != 0.0f)
            ? this.data.movement.rotationSpeedPowered
            : this.data.movement.rotationSpeedIdle;
        var deltaAngle = rotationalThrust * lerpFactor * deltaTime;

        this.m_deltaRotation = Quaternion.AngleAxis(deltaAngle, Vector3.up);

        return rotationalThrust * 90.0f; // @hack !!!
    }

    // --------------------------------------------------------------------------
    //! @brief Update calculation of PC's current direction for gamepad input.
    /*
    public void UpdateDirectionGamepad() : float
    {
        float angle = 0.0f;
        float forwardThrust = this.m_entity.inputHandler.Direction.z;
        float rotationalThrust = this.m_entity.inputHandler.Direction.x;
        if (this.m_entity.inputHandler.Direction == Vector3.zero)
        {
            return;
        }

        angle = Vector3.Angle(Vector3.forward, this.m_inputHandler.Direction);
        Vector3 cross = Vector3.Cross(Vector3.forward, this.m_inputHandler.Direction);
        if (cross.y < 0.0f)
        {
            angle *= -1.0f;
        }

        Quaternion toRotation = Quaternion.AngleAxis(angle, Vector3.up);
        float deltaAngle = Quaternion.Angle(this.transform.rotation, toRotation);

        float lerpFactor = deltaAngle;
        lerpFactor /= (this.IsMoving)
            ? this.rotationSpeedPowered
            : this.rotationSpeedIdle;

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, toRotation, Time.deltaTime / lerpFactor);

        int forwardSign = (Vector3.Dot(Vector3.forward, this.transform.forward) < 0.0f) ? -1.0f : 1.0f;
        int rotationalSign = rotationalThrust / Mathf.Abs(rotationalThrust);
        return forwardSign * rotationalSign * deltaAngle;
    }
    //*/
	
	public void UpdateControlMode( EntityManager.ControlMode controlMode )
	{
		switch( controlMode )
		{
			case EntityManager.ControlMode.Animation:
			{
				this.m_tilt.enabled = false;
				this.m_screenWrap.enabled = false;
				break;
			}
			case EntityManager.ControlMode.PlayerInput:
			default:
			{
				this.m_tilt.enabled = true;
				this.m_screenWrap.enabled = true;
				break;
			}
		}
	}
}

