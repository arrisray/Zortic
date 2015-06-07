using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;
using TSDC;

public class SimpleTransform
{
    public Vector3 position;
    public Quaternion rotation;
}

// ----------------------------------------------------------------------------
//! @brief Represents a game entity in the scene.
[RequireComponent( typeof(EntityData) )]
public class EntityManager : BaseManager
{
    // --------------------------------------------------------------------------
    // ENUMS
	
	public enum ControlMode
	{
		PlayerInput = 0,
		Animation,
		Count
	};

    public enum ThrustDirection
    {
        Forward = 0,
        Reverse,
        Left,
        Right,
        Count,
    };

    // --------------------------------------------------------------------------
    // DATA MEMBERS
	
    // Scene
    private EntityData m_data = null;

    // Components
    private EntityInputHandler m_inputHandler = null;
    private EntityNavigation m_navigation = null;
    private EntityThrusterManager m_thrusterManager = null;
	private EntityWeaponManager m_weaponManager = null;
	private SegmentedCircle m_healthIndicator = null;
	private SegmentedCircle m_powerIndicator = null;
	private EntityAudioManager m_audioManager = null;

    // Game
    private bool m_isDead = false;
    private int m_currentHealth = 0;
	private int m_currentLives = 0;
    private Material[] m_storedMaterials = null;
    private Hashtable initialTransforms = null;
    private int livesCurrent; 
	private List<GameObject> m_ignoreColliders = new List<GameObject>();
	private ControlMode m_controlMode = ControlMode.PlayerInput;
	private RigidbodyConstraints m_rigidbodyConstraints = RigidbodyConstraints.None;

    // --------------------------------------------------------------------------
    // PROPERTIES
 
	#region Properties

    public EntityManager identity
    {
        get
        {
            return ( this.data.game.proxyForEntity != null ) ? this.data.game.proxyForEntity : this;
        }
    }

    public int currentHealth
    {
        get { return this.m_currentHealth; }
		set
		{
			// Clamp health value to valid range
			this.m_currentHealth = value;
	        this.m_currentHealth = Mathf.Max( this.m_currentHealth, 0 );
	        this.m_currentHealth = Mathf.Min( this.m_currentHealth, this.data.game.health );

			this.m_healthIndicator.numVisibleSegments = this.m_currentHealth;
		}
    }

	public int currentLives
    {
        get { return this.m_currentLives; }
    }

    public EntityManager proxyForEntity 
    { 
        get { return this.m_data.game.proxyForEntity; } 
        set 
        { 
            // Utils.Assert( value != null ); 
            this.m_data.game.proxyForEntity = value; 
        }
    }

    public PlayerManager proxyForPlayer 
    { 
        get { return this.m_data.game.proxyForPlayer; } 
        set
        {
            // Utils.Assert( value != null ); 
            this.m_data.game.proxyForPlayer = value; 
        }
    }

    public EntityData data 
	{ 
		get 
		{ 
			return this.m_data; 
		} 
	}
    public EntityInputHandler inputHandler { get { return this.m_inputHandler; } }
	public bool enableInput
	{
		set
		{
			this.inputHandler.enabled = value;
		}
	}
    public EntityNavigation navigation { get { return this.m_navigation; } }
    public Vector2 CurrentScreenPosition
    {
        get
        {
            //! @todo It appears that there are cases in the first few seconds when this public void is invoked and Camera.current is null...why?!
            if ( !Camera.current )
            {
                return Vector2.zero;    
            }

            Vector2 pos = Camera.current.WorldToScreenPoint(this.m_data.movement.rootNode.position);    
            pos.y = Screen.height - pos.y;
            return pos;
        }
    }

	public EntityWeaponManager weaponManager
	{
		get { return this.m_weaponManager; }
	}
	
	public GameObject indicatorNode
	{
		get
		{
			Transform indicatorsTransform = this.transform.FindInChildren( "indicators" );
			if( indicatorsTransform != null )
			{
				return indicatorsTransform.gameObject;
			}
			return null;
		}
	}
	
	public ControlMode controlMode
	{
		get
		{
			return this.m_controlMode;
		}
		set
		{
			this.m_controlMode = value;
			this.UpdateControlMode( value );
		}
	}

	#endregion

    // --------------------------------------------------------------------------
    // METHODS

	public void Start()
	{
		GameManager.Instance.RegisterEntity( this );
		
		// Emit event message
		MessageManager.Instance.RaiseEvent( new EntitySetupMessage( this.gameObject ) ); 
	}

    public override void Setup()
    {
        base.Setup();

        this.m_data = this.GetComponent<EntityData>();
        this.proxyForEntity = this;
        this.gameManager = GameManager.Instance;
		this.m_currentHealth = this.m_data.game.health;
		this.m_currentLives = this.m_data.game.lives;
		if( this.m_data && this.m_data.rigidbody )
		{
			this.m_rigidbodyConstraints = this.m_data.rigidbody.constraints;
		}
		
		// Find the player's visual component
		this.m_data.visual.renderer = this.GetComponentInChildren<MeshRenderer>();

        // Register message handlers
        MessageManager.Instance.RegisterMessageHandler<AssignPlayerControlMessage>( this.OnPlayerControlAssignmentChanged );
		MessageManager.Instance.RegisterMessageHandler<EntityWrappedScreenMessage>( this.OnEntityWrappedScreen );

        // Enable physics
        if( this.collider )
        {
            this.collider.enabled = true;
        }

        // Add components
		this.m_navigation = this.gameObject.AddComponent<EntityNavigation>();
		this.m_audioManager = this.gameObject.AddComponent<EntityAudioManager>();
		this.m_audioManager.Init( this );
		
		// Setup attachments
        this.SetupAttachments();
		
		// Init health indicator
		this.m_data.visual.healthIndicator.numSegments = this.m_currentHealth;
		this.m_data.visual.healthIndicator.center = this.transform;
		this.m_data.visual.healthIndicator.attach = this.transform.FindChild( "indicators" );
		this.m_healthIndicator = this.gameObject.AddComponent<SegmentedCircle>();
		this.m_healthIndicator.Init( this.m_data.visual.healthIndicator );
		
		// Init power indicator
		foreach( EntityWeapon weapon in this.m_weaponManager.weapons )
		{
			if( weapon.data.game.actionType == EntityWeapon.ActionType.Primary )
			{
				weapon.PowerChanged += new EntityWeapon.PowerChangedEventHandler( this.OnEntityWeaponPowerChanged );
				weapon.PowerChangeWarning += new EntityWeapon.PowerChangeWarningEventHandler( this.OnEntityWeaponPowerChangeWarning );
				this.m_data.visual.powerIndicator.numSegments = weapon.data.game.maxDamage;
				break;
			}
		}
		this.m_data.visual.powerIndicator.center = this.transform;
		this.m_data.visual.powerIndicator.attach = this.transform.FindChild( "indicators" );
		this.m_powerIndicator = this.gameObject.AddComponent<SegmentedCircle>();
		this.m_powerIndicator.Init( this.m_data.visual.powerIndicator );
		
		
		if( this.m_data == null )
		{
			return;
		}
		this.InitMovement();
		this.m_navigation.Setup();
		
		//! @note AI must be initialized after movement.
		this.InitAI();

        if( this.data.visual.outlineMaterial && this.m_data.visual.renderer )
        {
            // Assign the Entity its copy-constructed outline material (so we don't make permanent changes to the version that exists in the project...)
            // @url http://answers.unity3d.com/questions/192561/renderermaterials-leaking-materials.html
            this.m_storedMaterials = this.m_data.visual.renderer.materials;
            for (int i = 0; i < this.m_storedMaterials.Length; ++i)
            {
                if (this.m_storedMaterials[i].name == (this.data.visual.outlineMaterial.name + " (Instance)")) //! @hack Hardcoded name check!!!
                {
                    this.data.visual.currentOutlineMaterial = this.m_storedMaterials[i]; 
                }
            }
        }

        // Setup initial transforms
        //! @note Order-dependent; must occur after InitMovement() because we might auto-generate a rotation node
		//  for banking that we'll want to include in our cache of initial transforms...
        this.SetupInitialTransforms();

        // Initialize input handling
        if( !this.m_inputHandler )
        {
            this.SetPlayerControlled( false );
        }
		
		if( this.m_data.game.timeToLive > 0.0f )
        {
            this.StartCoroutine( this.Expire() );
        }
    }
	
	public bool InitAI()
	{
		Transform nodeTransform = null;
		foreach( Transform xform in this.transform )
		{
			nodeTransform = xform.Find( "ai" );
			if( nodeTransform == null ) { continue; }
		}
		if( nodeTransform == null )
		{
			// /// Debug.LogWarning ( "Unable to find 'ai' node." );
			return false;
		}
		GameObject node = nodeTransform.gameObject;
		
		AiPlayerData data = node.GetComponent<AiPlayerData>();
		if( data == null )
		{
			// /// Debug.LogWarning ( "Unable to find AiPlayerData." );
			return false;
		}
		
		EntityAiInputHandler inputHandler = node.AddComponent<EntityAiInputHandler>();
		inputHandler.data = data;
		inputHandler.entity = this;
		inputHandler.m_rigidbody = this.data.movement.rigidbody;
		this.m_inputHandler = inputHandler;
			
		List<Steering> steerings = new List<Steering>( node.GetComponents<Steering>() );
		foreach( Steering steering in steerings )
		{
			if( data.game.target == null )
			{
				continue;
			}
			
			GameObject goTarget = GameManager.Instance.FindInstanceByTag( data.game.target );
			if( goTarget == null )
			{
				/// Debug.LogWarning ( "Steering pursuit quarry is not a GameObject: " + data.game.target );
				continue;
			}
			
			EntityManager entity = goTarget.GetComponent<EntityManager>();
			if( entity == null )
			{
				/// Debug.LogWarning ( "Steering pursuit quarry is not an Entity: " + data.game.target );
				continue;
			}
		
			if( steering as SteerForPursuit )
			{
				SteerForPursuit pursuitSteering = steering as SteerForPursuit;
				pursuitSteering.Quarry = entity;
			}
			else if( steering as SteerForEvasion )
			{
				SteerForEvasion evasionSteering = steering as SteerForEvasion;
				evasionSteering.Menace = entity;
			}
		}
		
		inputHandler.steerings = steerings;
		// inputHandler.Run();
		return true;
	}

    public void InitMovement()
    {
        if( !this.m_data.movement.rigidbody )
        {
            this.m_data.movement.rigidbody = this.rigidbody;
        }

        EntityData.MovementData movement = this.m_data.movement;

        // Position
        Vector3 position = ( movement.initialPosition != Vector3.zero ) ? movement.initialPosition : this.transform.position;
        if( movement.randomInitialPosition )
        {
            position = UnityEngine.Random.onUnitSphere;
            position.x = Mathf.Abs( position.x ); 
            position.y = Mathf.Abs( position.y ); 
            position.z = 1000.0f; //! @hack Magic number!!! Distance of game plane from camera.
            position = Camera.main.ViewportToWorldPoint( position );
        }
        this.transform.position = position;

        // Rotation
        Vector3 eulers = ( movement.initialRotation != Vector3.zero ) ? movement.initialRotation : this.transform.rotation.eulerAngles;
        Quaternion rotation = Quaternion.identity;
        if( movement.randomInitialRotation )
        {
            eulers = 360.0f * UnityEngine.Random.onUnitSphere;
        }
        rotation.eulerAngles = eulers;
        this.transform.rotation = rotation;

        // Physics Properties
        if ( movement.rigidbody )
        {
            // Speed
            float speed = ( UnityEngine.Random.value * (movement.initialSpeedRange.y - movement.initialSpeedRange.x) ) + movement.initialSpeedRange.x;
            Vector3 force = movement.randomInitialRotation
                ? UnityEngine.Random.onUnitSphere
                : this.transform.forward;
            force *= speed;

            movement.rigidbody.AddForce( force, ForceMode.Impulse );
            // /// Debug.Log( this.name + ", Init force: " + force );

            // Spin
            Vector3 torqueAxis = movement.initialSpinAxis;
            if( movement.randomInitialSpinAxis )
            {
                torqueAxis = UnityEngine.Random.onUnitSphere;
            }

            float spinForce = ( UnityEngine.Random.value * (movement.initialSpinForceRange.y - movement.initialSpinForceRange.x) ) + movement.initialSpinForceRange.x;
            Vector3 torque = spinForce * torqueAxis;
            movement.rigidbody.AddTorque( torque , ForceMode.Impulse );
            // /// Debug.Log( this.name + ", Init torque: " + torque );
        }

        if( !this.m_data.movement.rootNode )
        {
            // this.m_data.movement.rootNode = this.transform;
            this.m_data.movement.rootNode = this.transform.parent ? this.transform.parent : this.transform;
        }
		
		//! @hack Force EVERYTHING to exist on the xz-plane where y = 0!
		Vector3 p = this.transform.position;
		p.y = 0.0f;
		this.transform.position = p;
    }

    public void SetupInitialTransforms()
    {	
        // Create initial transforms
        SimpleTransform xform = null; 
        if( this.initialTransforms == null )
        {
            //! @todo Make this recursive child search a static utility function!
            this.initialTransforms = new Hashtable();
            foreach( Transform trans in this.GetComponentsInChildren<Transform>() )
            { 
                xform = new SimpleTransform();
                xform.position = trans.position;
                xform.rotation = trans.rotation;
                this.initialTransforms.Add(trans.gameObject, xform);
                // /// Debug.Log( trans.gameObject.name );
            }
        }
		
		this.RestoreInitialTransforms();
	}
	
	public void RestoreInitialTransforms()
	{
		// Apply initial transforms
        //! @note We traverse the transform hierarchy instead of the Hashtable's keys because
        //  the keys are sorted arbitrarily. If we use this ordering to reapply transforms, placement
        //  of things will get knocked out of whack in unpredictable ways!
		SimpleTransform xform = null; 
        foreach( Transform trans in this.GetComponentsInChildren<Transform>() )
        { 
            GameObject go = trans.gameObject;
            xform = this.initialTransforms[go] as SimpleTransform;
            go.transform.position = xform.position;
            go.transform.rotation = xform.rotation;
        }
    }

    public void SetupAttachments() 
    {
        // Thrusters
		this.m_thrusterManager = this.gameObject.GetComponent<EntityThrusterManager>();
		if( !this.m_thrusterManager )
		{
			this.m_thrusterManager = this.gameObject.AddComponent<EntityThrusterManager>();
		}
		this.m_thrusterManager.Init( this );

        // Weapons
		this.m_weaponManager = this.gameObject.GetComponent<EntityWeaponManager>();
		if( !this.m_weaponManager )
		{
			this.m_weaponManager = this.gameObject.AddComponent<EntityWeaponManager>();
		}
		this.m_weaponManager.Init( this );
        // /// Debug.Log( "Found " + weapons.Length + " weapons!" );
		
		for( int i = 0; i < this.data.visual.debugInfoIndicators.Count; ++i )
		{
			DebugInfoIndicator indicator = this.data.visual.debugInfoIndicators[i];
			indicator.Init( i );
		}
    }

    public override void Teardown()
    {
        base.Teardown(); 

        // Unregister message handlers
        MessageManager.Instance.UnregisterMessageHandler<AssignPlayerControlMessage>( this.OnPlayerControlAssignmentChanged );
		MessageManager.Instance.UnregisterMessageHandler<EntityWrappedScreenMessage>( this.OnEntityWrappedScreen );
        // /// Debug.Log( "Unregistered message handlers for: " + this.name + " (" + this.GetInstanceID() + ")");

		this.m_healthIndicator.Teardown();
		this.m_powerIndicator.Teardown();
    }

	public void ManagedLateUpdate()
	{
		this.m_healthIndicator.ManagedLateUpdate();
		if( this.m_powerIndicator ) this.m_powerIndicator.ManagedLateUpdate();
	}

	public void ManagedFixedUpdate()
	{
		if( (this == null) || this.m_isDead )
		{
			// /// Debug.Break ();
			return;
		}
		
		if( this.navigation && this.navigation.enabled && this.inputHandler )
		{
			this.navigation.ManagedFixedUpdate();
		}
	}

    public void ManagedUpdate()
    {
		if( (this == null) || this.m_isDead )
		{
			// /// Debug.Break ();
			return;
		}
		
        if (this.gameManager && this.gameManager.isGameOver)
        {
            return;
        }
		
		if( this.m_healthIndicator != null ) this.m_healthIndicator.ManagedUpdate();
		if( this.m_powerIndicator != null ) this.m_powerIndicator.ManagedUpdate();
		if( this.m_thrusterManager != null ) this.m_thrusterManager.ManagedUpdate();
		if( this.m_weaponManager != null ) this.m_weaponManager.ManagedUpdate();
		if( this.m_navigation != null ) this.m_navigation.ManagedUpdate();
		if( this.m_audioManager != null ) this.m_audioManager.ManagedUpdate();

		this.m_healthIndicator.numVisibleSegments = this.m_currentHealth;
		
		//! @todo Cleanup (remove) any references to support for more than one weapon per Entity for Zortic...
		foreach( EntityWeapon weapon in this.m_weaponManager.weapons )
		{
			if( weapon.data.game.actionType == EntityWeapon.ActionType.Primary )
			{
				this.m_powerIndicator.numVisibleSegments = weapon.currentPowerLevel;
				break;
			}
		}
    }

    public void OnGUI()
    { 
        if (this.inputHandler)
        {
            this.inputHandler.ManagedUpdate();
        }
		
		foreach( DebugInfoIndicator indicator in this.data.visual.debugInfoIndicators )
		{
			indicator.OnGUI();
		}
    }

    public void Reset()
    {
		this.RestoreInitialTransforms();
		
		// Show player
        if( this.m_data.visual.renderer )
        {
            this.m_data.visual.renderer.enabled = true;
        }
		
		// Restore full health
		this.currentHealth = this.m_data.game.health;
		
		// Reset movement state
		this.m_navigation.Reset();
		
		// Reset input handler
		this.m_inputHandler.Reset();
    }
	
	//! @note Complications arose when adding checks for dead entities, collisions were only processed one way.
	//  So if both entities dealt damage on collision, only one of the two would receive damage if one was destroyed in the exchange.
    public void OnCollisionEnter( Collision other )
    {
        EntityManager otherEntity = other.gameObject.GetComponent<EntityManager>();
		if( this.m_ignoreColliders.Contains(other.gameObject) )
		{
			return;
		}
		
        if( otherEntity )
        {
			this.m_ignoreColliders.Add( other.gameObject );
			
			// /// Debug.Log( "Collision => { other=" + otherEntity.name + " (" + otherEntity.m_isDead + "), this=" + this.name + " (" + this.m_isDead + ") }" );
        	MessageManager.Instance.RaiseEvent( new EntityCollisionMessage( this.gameObject, other.gameObject, other ) );
			if( this.data.visual.collisionParticles )
			{
				// Generate collision particles 
		        GameManager.Instance.UnityInstantiate (this.data.visual.collisionParticles, this.gameObject.transform.position, Quaternion.identity);
			}
			
			// /// Debug.Log ( this.name + " => +" + other.gameObject.name + " @ " + distance );
			float distance = Vector3.Distance( this.transform.position, other.gameObject.transform.position );
			this.StartCoroutine( this.IgnoreCollider( other.gameObject, distance ) );
        }
    }
	
	//! @todo Figure out a way to perform these distance checks uni-directionally between colliders;
	//  currently, this will be executed for both parties in any given collision!
	public IEnumerator IgnoreCollider( GameObject collider, float distance )
	{
		const float BREAK_THRESHOLD = 10.0f;
		float threshold = distance + BREAK_THRESHOLD;
		
		// while( true )
		{
			/*
			if( collider.gameObject == null ) { break; }
			float currentDistance = Vector3.Distance( this.transform.position, collider.transform.position );
			// /// Debug.Log ( this.name + " => " + collider.gameObject.name + ", threshold=" + threshold + ", distance=" + currentDistance );
			if( currentDistance > threshold ) { break; }
			yield return null;	
			*/
			yield return new WaitForSeconds( 1.0f );
		}
		
		// /// Debug.Log ( this.name + " => -" + collider.name );
		this.m_ignoreColliders.Remove( collider );
	}

    protected IEnumerator Expire() 
    {
        // /// Debug.Log( this.name + " expired." );
        yield return new WaitForSeconds( this.m_data.game.timeToLive );
        this.Die();
    }

    public void Die() 
    {
        this.Die( null );
    }

    public void Die( EntityManager source ) 
	{
        // Die player, die!
        GameObject sourceGameObject = ( source != null ) ? source.gameObject : null;
        MessageManager.Instance.RaiseEvent( new EntityDestroyedMessage( sourceGameObject, this.gameObject ) );

        // Hide player
        if( this.m_data.visual.renderer )
        {
            this.m_data.visual.renderer.enabled = false;
        }

        // Generate damage particles 
        if (this.data.visual.destroyedParticles != null) 
        {
            GameManager.Instance.UnityInstantiate (this.data.visual.destroyedParticles, this.gameObject.transform.position, Quaternion.identity);
        }

        // Spawn debris
        if( this.data.visual.debris.Count > 0 )
        {
            foreach( GameObject debri in this.data.visual.debris )
            {
                GameManager.Instance.UnityInstantiate ( debri, debri.transform.position + this.gameObject.transform.position, Quaternion.identity );
            } 
        }
		
		// Reset weapon power levels
		foreach( EntityWeapon weapon in this.m_weaponManager.weapons )
		{
			weapon.data.game.damage = 1;
		}
		
        // Respawn player
		//! @note Infinite Lives: ( this.data.game.lives < 0 )
        if ( (this.data.game.lives < 0) || (--this.m_currentLives >= 0) )
        {
            // /// Debug.Log( "Re-spawning player: " + this.name + ", currentLives=" + this.currentLives );
			if( this.data.game.lives < 0 ) { this.m_currentLives = this.data.game.lives; } //! @note For neatness only...
            this.SpawnPlayer(true);
        }
        // Annihilate player
        else 
        {
            //! @note We use a flag so the actual GameObject may be Destroyed on the next frame. Attempting to
            //  destroy it immediately will cause the MessageManager to throw an exception as it is usually iterating
            //  over its collection of listeners when this occurs. The entity being destroyed will cause it to 
            //  unregister as a listener and typically invalidates the MessageManager's iterator by modifying its
            //  listener collection, forcing it to abort further processing on this frame.
            this.m_isDead = true;
			
            this.StopAllCoroutines();
			GameManager.Instance.UnregisterEntity( this );
            GameManager.Instance.UnityDestroy( this.gameObject );
        }
    }
	
    public void SpawnPlayer(bool reset) 
    {
        if (reset)
        {
            this.Reset();
        }
    }

	public void OnEntityWrappedScreen( Message message )
	{
		EntityWrappedScreenMessage nativeMessage = (EntityWrappedScreenMessage)message;
		if( nativeMessage.entity == this.gameObject )
		{
			this.m_healthIndicator.Snap();
		}
	}
    
    public void OnPlayerControlAssignmentChanged( Message message )
    {
        AssignPlayerControlMessage derivedMessage = message as AssignPlayerControlMessage;
        EntityManager oldEntityManager = derivedMessage.oldPlayer.GetComponent<EntityManager>();
		EntityManager newEntityManager = derivedMessage.newPlayer.GetComponent<EntityManager>();

        // /// Debug.Log( "old=" + derivedMessage.oldPlayer + ", new=" + derivedMessage.newPlayer );
        if( !newEntityManager )
        {
            /// Debug.LogError( "Unable to handle player control assignment because the EntityManager was missing!" );
            return;
        }

        // /// Debug.Log( "PlayerControlAssignmentChanged: Entity=" + entityManager.name + ", player=" + playerManager.name);
		if( this.gameObject == derivedMessage.oldPlayer )
		{
			this.SetPlayerControlled( false );
		}
		if( this.gameObject == derivedMessage.newPlayer )
		{
        	this.SetPlayerControlled( true );
		}
    }

    private void SetPlayerControlled( bool isPlayerControlled )
    {
        PlayerManager playerManager = GameManager.Instance.playerManager;
        EntityInputHandler inputHandler = null;
        if( isPlayerControlled )
        {
            this.m_data.game.proxyForPlayer = playerManager;

            // Destroy AI input handler
            inputHandler = this.GetComponent<EntityAiInputHandler>();
            if( inputHandler )
            {
                GameManager.Instance.UnityDestroy( inputHandler );
            }
			
            // Create live input handler
            this.m_inputHandler = this.gameObject.AddComponent<EntityLiveInputHandler>();
            this.m_inputHandler.inputManager = this.gameManager.inputManager;
			this.m_inputHandler.entity = this;
        }
        else
        {
            this.m_data.game.proxyForPlayer = null;

            // Destroy live input handler
            inputHandler = this.GetComponent<EntityLiveInputHandler>();
            if( inputHandler )
            {
                GameManager.Instance.UnityDestroy( inputHandler );
            }
			
            // Create AI input handler
			this.InitAI();
        }
    }
	
	public void UpdateHealth( float health, EntityManager sourceEntity )
    {
		// Ignore health modification requests if player has no direct control over Entity
		// /// Debug.Log ( "Entity control mode: " + this.m_controlMode + ", " + this.name );
		if( this.m_controlMode == ControlMode.Animation )
		{
			return;
		}
		
        this.currentHealth += (int)health;
		this.ShowIndicators( true );

		// /// Debug.Log( this.name + "::UpdateHealth( " + health + "), current=" + this.healthCur + ", source=" + sourceEntity.name );

        if( health < 0.0f )
        {      
			GameObject goSource = sourceEntity ? sourceEntity.gameObject : null;
            MessageManager.Instance.RaiseEvent( new AssignEntityDamageMessage( this.gameObject, goSource, health ) );
        }

        if( this.m_currentHealth <= 0.0f )
        {
            this.Die( sourceEntity );
        }
    }
	
	public void OnAnimationEvent( string animation )
	{
		MessageManager.Instance.RaiseEvent( new EntityAnimationEventMessage( this.gameObject, animation ) );
	}
	
	public void OnEntityWeaponPowerChanged( EntityWeapon weapon )
	{
		this.ShowIndicators( true );
		this.m_powerIndicator.numVisibleSegments = weapon.currentPowerLevel;
	}
	
	public void OnEntityWeaponPowerChangeWarning( EntityWeapon weapon, float percent )
	{
			
	}
	
	public void ShowIndicators( bool enabled )
	{
		if( this.m_healthIndicator != null ) this.m_healthIndicator.Show ( enabled );
		if( this.m_powerIndicator != null ) this.m_powerIndicator.Show ( enabled );
	}
	
	private void UpdateControlMode( ControlMode controlMode )
	{
		this.m_navigation.UpdateControlMode( controlMode );
		
		EntityLiveInputHandler liveInputHandler = this.m_inputHandler as EntityLiveInputHandler;
		if( liveInputHandler )
		{
			liveInputHandler.UpdateControlMode( controlMode );
		}
		
		if( this.m_data.rigidbody )
		{
			switch( controlMode )
			{
				case EntityManager.ControlMode.Animation:
				{
					this.m_data.rigidbody.constraints = RigidbodyConstraints.None;
					break;
				}
				case EntityManager.ControlMode.PlayerInput:
				default:
				{
					this.m_data.rigidbody.constraints = this.m_rigidbodyConstraints;
					break;
				}
			}
		}
	}
} // class EntityManager
