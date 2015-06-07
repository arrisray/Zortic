using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityThrusterManager : MonoBehaviour
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS
	
	private bool m_isStabilized = true;
	private EntityManager m_entity = null;
	private List<EntityThruster> m_thrusters = null;
	private bool m_isWatchingForFullStop = false;

	// --------------------------------------------------------------------------
    // METHODS

	public void Init( EntityManager entity )
    {
    	this.m_entity = entity;

		this.DiscoverThrusters();
    }

	public void Start ()
	{
	
	}
	
	public void ManagedUpdate ()
	{
		if( this.m_entity.navigation.isMoving && !this.m_isWatchingForFullStop )
		{
			this.m_isWatchingForFullStop = true;
			StartCoroutine( this.WatchForFullStop() );
		}

		if( !this.m_isStabilized )
		{
			foreach( EntityThruster thruster in this.m_thrusters )
			{
				thruster.Stabilize();
			}
			this.m_isStabilized = true;
		}

		foreach( EntityThruster thruster in this.m_thrusters )
		{
			thruster.ManagedUpdate();
		}
	}

	protected IEnumerator WatchForFullStop()
	{
		while( this.m_entity.navigation.isMoving )
		{
			yield return null;
		}

		this.m_isStabilized = false;
		this.m_isWatchingForFullStop = false;
	}

	protected void DiscoverThrusters()
	{
		this.m_thrusters = new List<EntityThruster>();
        EntityThrusterData[] thrusters = this.m_entity.GetComponentsInChildren<EntityThrusterData>();
        foreach( EntityThrusterData data in thrusters )
        {
            EntityThruster thruster = data.gameObject.AddComponent<EntityThruster>();
            thruster.Init( this.m_entity, data );
            this.m_thrusters.Add( thruster );
        }
        // /// Debug.Log( "Found " + thrusters.Length + " thrusters!" );
	}
}

