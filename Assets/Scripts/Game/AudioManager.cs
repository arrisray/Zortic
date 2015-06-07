using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour 
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS
	
	private bool m_isPaused = false;
	private List<EntityAudioManager> m_entityAudioManagers = new List<EntityAudioManager>();
	
	// --------------------------------------------------------------------------
    // PROPERTIES
	
	// --------------------------------------------------------------------------
    // METHODS
	
	public void Setup() 
	{
		// EntityAudioManager[] entityAudioManagers = GameObject.FindObjectsOfType( typeof( EntityAudioManager ) ) as EntityAudioManager[];
		// this.m_entityAudioManagers.AddRange( entityAudioManagers );
	}
	
	public void ManagedUpdate() 
	{
	
	}
	
	public bool AddEntityAudioManager( EntityAudioManager entityAudioManager )
	{
		if( this.m_entityAudioManagers.Contains( entityAudioManager ) )
		{
			return false;
		}
		this.m_entityAudioManagers.Add ( entityAudioManager );
		return true;
	}
	
	public bool RemoveEntityAudioManager( EntityAudioManager entityAudioManager )
	{
		return this.m_entityAudioManagers.Remove( entityAudioManager );
	}

	public void Pause()
	{
		this.m_isPaused = !this.m_isPaused;
		foreach( EntityAudioManager audioManager in this.m_entityAudioManagers )
		{
			audioManager.Pause( this.m_isPaused );
		}
	}
}
