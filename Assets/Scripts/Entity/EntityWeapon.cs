using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityWeapon : EntityAttachment
{
	// --------------------------------------------------------------------------
    // DELEGATES
	
	public delegate void PowerChangedEventHandler( EntityWeapon weapon );
	public delegate void PowerChangeWarningEventHandler( EntityWeapon weapon, float percent );
	
    // --------------------------------------------------------------------------
    // ENUMS

    public enum ActionType
    {
        Unknown,
        Primary,
        Secondary
    }

    // --------------------------------------------------------------------------
    // DATA MEMBERS
	
	public event PowerChangedEventHandler PowerChanged;
	public event PowerChangeWarningEventHandler PowerChangeWarning;
    private float nextFire = 0.0f;     //fireRate check variable
    private EntityWeaponData m_data = null;
    private EntityManager m_target = null;
	private EntityManager m_entity = null;
	private List<EntityManager> m_projectiles = new List<EntityManager>();

    // --------------------------------------------------------------------------
    // PROPERTIES

    public EntityWeaponData data { get { return this.m_data; } }
    public EntityManager currentTarget { get { return this.m_target; } }
    public float energyUse { get { return this.m_data.projectile.energyUse; } }
	public int currentPowerLevel
	{
		get
		{
			return this.m_data.game.damage;
		}
		set { this.m_data.game.damage = value; }
	}

    // --------------------------------------------------------------------------
    // METHODS

    public void Init( EntityManager entity, EntityWeaponData data )
    {
		this.m_entity = entity;
        this.m_data = data;
        this.m_data.game.proxyForEntity = entity;
    }

    public void OnEnable()
    {
        MessageManager.Instance.RegisterMessageHandler<EntityDestroyedMessage>( this.OnEntityDestroyed );
    }

    public void OnDisable()
    {
        MessageManager.Instance.UnregisterMessageHandler<EntityDestroyedMessage>( this.OnEntityDestroyed );
    }

    public void Reset()
    {
        this.nextFire = 0.0f; 
        this.m_target = null;
        this.m_projectiles.Clear();
		
		this.data.projectile.currentVisualInfo = new EntityWeaponData.ProjectileData.VisualInfo( this.data.projectile.initVisualInfo );
    }

    public void OnGUI()
    {
        // Draw targeting reticle(s)
        if ( (this.m_data != null)
			&& (this.m_data.visual != null) 
			&& (this.m_data.visual.showReticle)
			&& (this.m_data.visual.targetLock != null)
			&& (this.m_target != null) )
        {
            // /// Debug.Log( this.name + ", draw reticle: " + target );
            var targetPos = Camera.main.WorldToScreenPoint(this.m_target.transform.position);
            GUI.DrawTexture(new Rect(
                targetPos.x - (Screen.height/40), 
                Screen.height - targetPos.y - (Screen.height/40), 
                Screen.height/20, Screen.height/20), 
                this.m_data.visual.targetLock);
        }
    }

    public void OnEntityDestroyed( Message message )
    {
        EntityDestroyedMessage nativeMessage = (EntityDestroyedMessage)message;
        GameObject go = nativeMessage.victim;
        EntityManager entity = go.GetComponent<EntityManager>();
        if( !entity )
        {
            /// Debug.LogWarning( "Ignoring entity destroy message for GameObject (=" + go.name + ") without EntityManager component.");
            return;
        }

        // Is projectile?
        if( this.m_projectiles.Contains(entity) )
        {
            this.m_projectiles.Remove( entity ); 
            // /// Debug.Log( "Removed projectile: " + entity.name );
        }

        // Is target?
        if( entity == this.m_target )
        {
            this.m_target = null;        
        }
		
		if( entity == this.m_entity )
		{
			this.Reset ();
		}
    } 

    public void FixedUpdate()
    {
        this.UpdateProjectiles();
    }

    public void UpdateTargeting()
    {
        RaycastHit[] hits = Physics.RaycastAll(this.transform.position, this.transform.forward);
        foreach( RaycastHit hit in hits )
        {
            GameObject hitObject = hit.transform.gameObject;
            EntityManager entity = hitObject.GetComponent<EntityManager>();
            if( entity && entity.data.game.isTargetable )
            {
                // /// Debug.Log( "Can target " + this.name + "...?" );

                // Check targeting filter
                bool isFiltered = false;
                foreach( EntityTag entityTag in this.data.targeting.targetFilter )
                {
                    // /// Debug.Log( this.name + " target filter: " + entityTag );
                    // /// Debug.Log( entity.name + " MonoTag count: " + entity.entityTags.Count);
                    if( entity.entityTags.Contains(entityTag) )
                    {
                        // /// Debug.Log( entity.name + " contains MonoTag: TRUE");
                        isFiltered = true;
                        break;
                    }
                }
                if( isFiltered )
                {
                    continue;
                }

                // Save target reference
                this.m_target = entity;
            }
        } // end foreach
    }

    public bool UpdateFire()  
    {
        if ((Time.time > this.nextFire))
        { 
            // Not enough ergs for the pew-pew...?
			/*
            if (currentEnergy < this.m_data.projectile.energyUse)
            {
                if (this.m_data.game.showInsufficientEnergyWarning)
                {
                    //! @todo PUT THIS BACK IN...
                    // Perform each scene in response to insufficient energy for weapon fire
                    // this.m_screenplay.Perform("recharge", false);
                }
                return 0.0f;
            }
            //*/

            // Create the projectile for the pew-pew!
			this.CreateProjectile();

            // currentEnergy -= this.m_data.projectile.energyUse;
            this.nextFire = Time.time + this.m_data.projectile.fireRate;
        }

        return true;
    }
	
	public void PowerUp()
	{
		this.data.projectile.currentVisualInfo.trailLength += this.data.projectile.trailLengthDelta;
		this.data.projectile.currentVisualInfo.trailWidth.x += this.data.projectile.trailWidthDelta.x;
		this.data.projectile.currentVisualInfo.trailWidth.y += this.data.projectile.trailWidthDelta.y;
		this.data.projectile.currentVisualInfo.headSize += this.data.projectile.headSizeDelta;
		
		if( this.PowerChanged != null )
		{
			this.PowerChanged( this );
		}
	}
	
	public void PowerDownWarningPeriod( float percent )
	{
		if( this.PowerChangeWarning != null )
		{
			this.PowerChangeWarning( this, percent );
		}
	}
	
	public void PowerDown()
	{
		this.data.projectile.currentVisualInfo.trailLength -= this.data.projectile.trailLengthDelta;
		this.data.projectile.currentVisualInfo.trailWidth.x -= this.data.projectile.trailWidthDelta.x;
		this.data.projectile.currentVisualInfo.trailWidth.y -= this.data.projectile.trailWidthDelta.y;
		this.data.projectile.currentVisualInfo.headSize -= this.data.projectile.headSizeDelta;
		
		if( this.PowerChanged != null )
		{
			this.PowerChanged( this );
		}
	}

    protected void UpdateProjectiles()
    {
        // Homing
        foreach (EntityManager projectile in this.m_projectiles)
        {
            if( projectile == null || projectile.rigidbody == null )
            {
				/// Debug.LogWarning( "Projectile \"(" + this.name + "\") is not a valid entity or is missing a rigidbody." );
				continue;
			}
			
			// Update power level visuals
			TrailRenderer trailRenderer = projectile.GetComponent<TrailRenderer>();
			if( trailRenderer != null )
			{
				trailRenderer.time = this.data.projectile.currentVisualInfo.trailLength;
				trailRenderer.startWidth = this.data.projectile.currentVisualInfo.trailWidth.x;
				trailRenderer.endWidth = this.data.projectile.currentVisualInfo.trailWidth.y;
			}
			
			// Only deal with homing logic after this...
			if( ( (this.m_data != null) && (this.m_data.projectile != null) && !this.m_data.projectile.isHoming ) || !this.m_target)
	        {
	            continue;
	        }
			
            this.UpdateHomingProjectile( projectile );
        }
    }
	
	private void UpdateHomingProjectile( EntityManager projectile )
	{
		// Position
        Vector3 direction = this.m_target.transform.position - projectile.transform.position;
        projectile.rigidbody.AddForce(direction.normalized * 0.1f, ForceMode.Impulse); 

        // Rotation
        Quaternion targetRotation = Quaternion.LookRotation(this.m_target.transform.position - projectile.rigidbody.position, Vector3.up);
        targetRotation = Quaternion.Slerp( projectile.rigidbody.rotation, targetRotation, 0.1f );
        projectile.rigidbody.MoveRotation( targetRotation );

		// Limit projectile speed
		// Apply speed limit
		//! @note ...only if max speed has been set!
		if( (projectile.data.movement.maxSpeed != 0.0f)
			&& (projectile.data.movement.rigidbody.velocity.magnitude > projectile.data.movement.maxSpeed) )
		{
			projectile.data.movement.rigidbody.velocity = projectile.data.movement.rigidbody.velocity.normalized * projectile.data.movement.maxSpeed;
		}
	}

	private void CreateProjectile()
    {
        GameObject goProjectile = GameManager.Instance.UnityInstantiate(this.m_data.visual.projectile, this.transform.position, this.transform.rotation) as GameObject;
        EntityManager projectileEntity = goProjectile.GetComponent<EntityManager>();
        if( projectileEntity )
        {
            projectileEntity.data.game.collisionDamage = this.m_data.game.damage; //! @todo Maybe this could be a force multiplier...?!
            projectileEntity.proxyForEntity = this.m_data.game.proxyForEntity;
            projectileEntity.proxyForPlayer = this.m_data.game.proxyForEntity.proxyForPlayer;

			// Ignore collisions between projectile and its creator
			foreach( Collider collider in projectileEntity.proxyForEntity.colliders )
			{
				foreach( Collider projectileCollider in projectileEntity.colliders )
				{
					// /// Debug.Log( "Ignoring collision between: " + collider.name + " & " + projectileCollider.name );
					Physics.IgnoreCollision( collider, projectileCollider );
				}
			}
			
			MessageManager.Instance.RaiseEvent( new EntityCreatedMessage( projectileEntity.gameObject, this.m_entity.gameObject ) ); 
			MessageManager.Instance.RaiseEvent( new EntityShootMessage( this.m_entity ) ); 

            this.m_projectiles.Add( projectileEntity );
        }
    }

	private bool IsTargetInRange( EntityManager entity )
	{
		return true;
	}
} // end class EntityWeapon
