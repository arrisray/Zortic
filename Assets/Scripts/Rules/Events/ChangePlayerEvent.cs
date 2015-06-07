using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePlayerEvent : GameRuleEvent
{

    // --------------------------------------------------------------------------
    // DATA MEMBERS
	
	public bool disableCurrentPlayer = false;
    public UnityEngine.Object newPlayer = null;

    // --------------------------------------------------------------------------
    // PROPERTIES
	
	
    // --------------------------------------------------------------------------
    // METHODS
	
	public override void OnStart()
	{
	}

    public override void OnRaise()
    {
		GameObject prefab = GameManager.Instance.FindPrefabByTag( this.newPlayer );
		if( prefab == null ) 
		{
			/// Debug.LogWarning( "Unable to find new player prefab by tag: " + this.newPlayer );
			return;
		}
		
		GameObject currentPlayer = GameManager.Instance.playerManager.currentEntity.gameObject;
		if( this.disableCurrentPlayer )
		{
			currentPlayer.SetActiveRecursively( false );
		}
		
		GameObject instance = GameManager.Instance.UnityInstantiate( prefab, currentPlayer.transform.position, currentPlayer.transform.rotation ) as GameObject;
		if( instance == null )
		{
			/// Debug.LogWarning( "Unable to instantiate new player from prefab: " + prefab );
			return;
		}
		
		EntityManager entity = instance.GetComponent<EntityManager>();
		if( entity == null )
		{
			/// Debug.LogWarning( "New player (" + instance + ") is not an Entity!" );
			return;
		}
		GameManager.Instance.currentPlayer = entity;
    }

    public override void OnLoad()
    {
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
} // class LoadLevelEvent