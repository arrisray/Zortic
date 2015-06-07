using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispenseGrabbimiteEvent : GameRuleEvent
{

    // --------------------------------------------------------------------------
    // DATA MEMBERS
	
	public UnityEngine.Object grabbimiteTag = null;

    // --------------------------------------------------------------------------
    // PROPERTIES
	
	
    // --------------------------------------------------------------------------
    // METHODS

    public override void OnRaise()
    {
		
    }

    public override void OnLoad()
    {
		MessageManager.Instance.RegisterMessageHandler<EntityDestroyedMessage>( this.OnEntityDestroyed );
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
	
	public void OnEntityDestroyed( Message message )
    {
        EntityDestroyedMessage nativeMessage = message as EntityDestroyedMessage;
        GameObject go = nativeMessage.victim;
        EntityManager entity = go.GetComponent<EntityManager>();
		if( entity == null )
		{
			return;
		}
		
		if( entity != GameManager.Instance.currentPlayer )
		{
			return;
		}
		
		foreach( EntityWeapon weapon in entity.weaponManager.weapons )
		{
			if( weapon.data.game.actionType == EntityWeapon.ActionType.Primary )
			{
				this.DispenseGrabbimite( entity, weapon );
			}
		}
	}
	
	private void DispenseGrabbimite( EntityManager player, EntityWeapon weapon )
	{
		// /// Debug.Log( "CURRENT POWER LEVEL: " + weapon.currentPowerLevel );
		GameObject grabbimitePrefab = GameManager.Instance.FindPrefabByTag( this.grabbimiteTag );
		for( int i = BuffParams.MIN_LEVEL; i < weapon.currentPowerLevel; ++i ) //! @hack Need a better place to store & access MIN_LEVEL!!!
		{
			this.SpawnGrabbimite( player, grabbimitePrefab );
		}
	}
	
	protected void SpawnGrabbimite( EntityManager player, GameObject prefab )
    {
        // Position
		Vector3 instancePosition = player.transform.position;

        // Rotation
		Quaternion instanceRotation = player.transform.rotation;
		
		// Spawn instance
        GameObject instance = GameManager.Instance.UnityInstantiate(prefab, instancePosition, instanceRotation) as GameObject;
		// /// Debug.Log ("Spawned entity: " + instance.name );
		
		// Init instance movement params
        EntityManager entity = instance.GetComponent<EntityManager>();
        if( entity && entity.data )
        {
            entity.data.movement.initialPosition = instancePosition;
            entity.data.movement.initialSpeedRange = UnityEngine.Random.insideUnitCircle * 50.0f;
            entity.data.movement.initialSpinForceRange = UnityEngine.Random.insideUnitCircle * 50.0f;
            entity.data.movement.initialSpinAxis = Vector3.up;
            entity.InitMovement();
        }
    }
} // class DispenseGrabbimiteEvent