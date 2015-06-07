using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityWeaponManager : MonoBehaviour
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS
	
	private EntityManager m_entity = null;
	private List<EntityWeapon> m_weapons = new List<EntityWeapon>();
	private List<EntityManager> m_projectiles = new List<EntityManager>();
	
	// --------------------------------------------------------------------------
    // PROPERTIES

	public List<EntityWeapon> weapons
	{
		get { return this.m_weapons; }
	}

	// --------------------------------------------------------------------------
    // METHODS

	public void Init( EntityManager entity )
	{
		this.m_entity = entity;

		this.m_weapons = new List<EntityWeapon>();
        EntityWeaponData[] weapons = this.GetComponentsInChildren<EntityWeaponData>();
        foreach( EntityWeaponData data in weapons )
        {
            EntityWeapon weapon = data.gameObject.AddComponent<EntityWeapon>();
            weapon.Init( this.m_entity, data );
            this.m_weapons.Add( weapon );
        }

		MessageManager.Instance.RegisterMessageHandler<EntityCreatedMessage>( this.OnEntityCreated );
		MessageManager.Instance.RegisterMessageHandler<EntityDestroyedMessage>( this.OnEntityDestroyed );
	}

	public void Teardown()
	{
		MessageManager.Instance.UnregisterMessageHandler<EntityCreatedMessage>( this.OnEntityCreated );
		MessageManager.Instance.UnregisterMessageHandler<EntityDestroyedMessage>( this.OnEntityDestroyed );
	}

	public void ManagedUpdate()
    {
        foreach( EntityWeapon weapon in this.m_weapons )
        {
            weapon.UpdateTargeting();
			
            switch (weapon.data.game.actionType)
            {
                case EntityWeapon.ActionType.Primary:
                {
                    if (this.m_entity.inputHandler.isFiringPrimaryWeapon) 
                    {
                        weapon.UpdateFire();
                    }    
                    break;
                }
                case EntityWeapon.ActionType.Secondary:
                {
                    if (this.m_entity.inputHandler.isFiringSecondaryWeapon) 
                    {
                        weapon.UpdateFire();
                    }    
                    break;
                }
                default:
                {
                    break;
                }
            } // end switch
        } // end foreach
    }

	protected void OnEntityCreated( Message message )
	{
		EntityCreatedMessage derivedMessage = message as EntityCreatedMessage;
		EntityManager entityCreator = null;
		EntityWeapon[] entityWeapons = derivedMessage.creator.GetComponentsInChildren<EntityWeapon>();
		foreach( EntityWeapon weapon in entityWeapons )
		{
			if( this.m_weapons.Contains( weapon ) )
			{
				entityCreator = derivedMessage.creator.GetComponent<EntityManager>();
				break;
			}
		}

		if( !entityCreator )
		{
			return;
		}

		// Ignore collisions against all other projectiles...
		EntityManager newProjectile = derivedMessage.entity.GetComponent<EntityManager>();
		foreach( EntityManager siblingProjectile in this.m_projectiles )
		{
			foreach( Collider siblingCollider in siblingProjectile.colliders )
			{
				foreach( Collider newCollider in newProjectile.colliders )
				{
					// /// Debug.Log( "Ignoring collision between: " + collider.name + " & " + projectileCollider.name );
					Physics.IgnoreCollision( newCollider, siblingCollider );
				}
			}
		}

		this.m_projectiles.Add( newProjectile );
	}

	protected void OnEntityDestroyed( Message message )
	{
		EntityDestroyedMessage derivedMessage = message as EntityDestroyedMessage;
		EntityManager destroyedEntity = derivedMessage.victim.GetComponent<EntityManager>();
		if( !this.m_projectiles.Contains( destroyedEntity ) )
		{
			return;
		}
		
		this.m_projectiles.Remove( destroyedEntity );
	}
}

