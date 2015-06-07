using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAudioManager : MonoBehaviour
{
	// --------------------------------------------------------------------------
    // STRUCTS
	
	public struct AudioState
	{
		public int timeSamples;
		public bool isPlaying;
		
		public void Clear()
		{
			this.timeSamples = 0;
			this.isPlaying = false;
		}
	}
	
	// --------------------------------------------------------------------------
    // DATA MEMBERS
    
	private EntityManager entity = null;
	private AudioSource m_retroThrusterAudio = null;
	private AudioSource m_mainThrusterAudio = null;
	private AudioSource m_weaponAudio = null;
	private AudioSource m_destroyedAudio = null;
	private AudioSource m_collidedAudio = null;
	private Dictionary<AudioSource, AudioState> m_playStateMap = new Dictionary<AudioSource, AudioState>();
	
	// --------------------------------------------------------------------------
    // PROPERTIES
	
	public EntityData.SoundData data
	{
		get { return this.entity.data.sound; }
	}
	
	// --------------------------------------------------------------------------
    // METHODS
	
	public void Init( EntityManager entity )
	{
		this.entity = entity;
		
		MessageManager.Instance.RegisterMessageHandler<EntityShootMessage>( this.OnEntityShoot );
		MessageManager.Instance.RegisterMessageHandler<EntityThrusterEventMessage>( this.OnEntityThrusterEvent );
		MessageManager.Instance.RegisterMessageHandler<EntityDestroyedMessage>( this.OnEntityDestroyed );
		MessageManager.Instance.RegisterMessageHandler<EntityCollisionMessage>( this.OnEntityCollision );
		
		if( this.data != null )
		{
			if( this.data.weapon != null )
			{
				this.m_weaponAudio = this.gameObject.AddComponent<AudioSource>();
				this.m_weaponAudio.clip = this.data.weapon;
				this.m_weaponAudio.loop = false;
				this.m_weaponAudio.volume = 1.0f;
				this.m_weaponAudio.priority = 255;
				this.m_playStateMap.Add( this.m_weaponAudio, new AudioState() );
			}
			
			if( this.data.mainThruster != null )
			{
				this.m_mainThrusterAudio = this.gameObject.AddComponent<AudioSource>();
				this.m_mainThrusterAudio.clip = this.data.mainThruster;
				this.m_mainThrusterAudio.loop = true;
				this.m_mainThrusterAudio.volume = 1.0f;
				this.m_mainThrusterAudio.priority = 129;
				this.m_playStateMap.Add( this.m_mainThrusterAudio, new AudioState() );
			}
			
			if( this.data.retroThruster != null )
			{
				this.m_retroThrusterAudio = this.gameObject.AddComponent<AudioSource>();
				this.m_retroThrusterAudio.clip = this.data.retroThruster;
				this.m_retroThrusterAudio.loop = false;
				this.m_retroThrusterAudio.volume = 1.0f;
				this.m_retroThrusterAudio.priority = 128;
				this.m_playStateMap.Add( this.m_retroThrusterAudio, new AudioState() );
			}
			
			if( this.data.destroyed != null )
			{
				this.m_destroyedAudio = this.gameObject.AddComponent<AudioSource>();
				this.m_destroyedAudio.clip = this.data.destroyed;
				this.m_destroyedAudio.loop = false;
				this.m_destroyedAudio.volume = 1.0f;
				this.m_destroyedAudio.priority = 255;
				this.m_playStateMap.Add( this.m_destroyedAudio, new AudioState() );
			}
			
			if( this.data.collided != null )
			{
				this.m_collidedAudio = this.gameObject.AddComponent<AudioSource>();
				this.m_collidedAudio.clip = this.data.collided;
				this.m_collidedAudio.loop = false;
				this.m_collidedAudio.volume = 1.0f;
				this.m_collidedAudio.priority = 255;
				this.m_playStateMap.Add( this.m_collidedAudio, new AudioState() );
			}
		}
		
		// Register with AudioManager
		GameManager.Instance.audioManager.AddEntityAudioManager( this );
	}
	
	public void Destroy()
	{
		if( this.m_destroyedAudio != null)
		{
			AudioSource.PlayClipAtPoint( this.data.destroyed, Camera.main.transform.position );
		}
		
		// Unregister from AudioManager
		GameManager.Instance.audioManager.RemoveEntityAudioManager( this );
	}
	
	public void OnEntityShoot( Message message )
	{
		EntityShootMessage derivedMessage = message as EntityShootMessage;
		if( derivedMessage.entity == this.entity 
			&& this.m_weaponAudio != null)
		{
			this.m_weaponAudio.Play(); // OneShot( this.data.weapon.clip );
		}
	}
	
	public void OnEntityDestroyed( Message message )
	{
		// Only process audio events for this manager's associated Entity
		EntityDestroyedMessage derivedMessage = message as EntityDestroyedMessage;
		EntityManager entityManager = derivedMessage.victim.GetComponent<EntityManager>();
		if( entityManager != this.entity )
		{
			return;
		}
		
		this.Destroy ();
	}
	
	public void OnEntityThrusterEvent( Message message )
	{
		// Only process audio events for this manager's associated Entity
		EntityThrusterEventMessage derivedMessage = message as EntityThrusterEventMessage;
		EntityManager entityManager = derivedMessage.entity.GetComponent<EntityManager>();
		if( entityManager != this.entity )
		{
			return;
		}
			
		switch( derivedMessage.eventType )
		{
			case EntityThruster.EventType.StartThrust:
			{
				if( this.m_mainThrusterAudio == null ) { break; }
				if( !this.m_mainThrusterAudio.isPlaying ) 
				{
					this.m_mainThrusterAudio.Play();
				}
				break;
			}
			case EntityThruster.EventType.EndThrust:
			{
				if( this.m_mainThrusterAudio == null ) { break; }
				if( this.m_mainThrusterAudio.isPlaying ) 
				{
					this.m_mainThrusterAudio.Stop();
				}
				break;
			}
			case EntityThruster.EventType.StartStabilize:
			{
				if( this.m_retroThrusterAudio == null
				|| this.data == null
				|| this.data.retroThruster == null ) { break; }
				this.m_retroThrusterAudio.PlayOneShot( this.data.retroThruster );
				break;
			}
			case EntityThruster.EventType.EndStabilize:
			case EntityThruster.EventType.Idle:
			default:
			{
				break;
			}
		}
	}
	
	public void OnEntityCollision( Message message )
	{
		// Only process audio events for this manager's associated Entity
		EntityCollisionMessage derivedMessage = message as EntityCollisionMessage;
		EntityManager targetEntity = derivedMessage.target.GetComponent<EntityManager>();
		EntityManager colliderEntity = derivedMessage.collider.GetComponent<EntityManager>();
		// /// Debug.Log ( this.entity + " (id=" + this.entity.GetInstanceID() + "), " + targetEntity + " (id=" + targetEntity.GetInstanceID() + ")" );
		if( ( this.entity.GetInstanceID() == targetEntity.GetInstanceID() ) && this.m_collidedAudio != null )
		{
			this.m_collidedAudio.Play();
		}
	}
	
	public void ManagedUpdate()
	{
		
	}
	
	public void Pause( bool pause )
	{
		if( pause )
		{
			this.StoreAudioState( this.m_retroThrusterAudio );
			this.StoreAudioState( this.m_mainThrusterAudio );
			this.StoreAudioState( this.m_weaponAudio );
			this.StoreAudioState( this.m_destroyedAudio );
		}
		else
		{
			this.RestoreAudioState( this.m_retroThrusterAudio );
			this.RestoreAudioState( this.m_mainThrusterAudio );
			this.RestoreAudioState( this.m_weaponAudio );
			this.RestoreAudioState( this.m_destroyedAudio );
		}
	}
	
	private bool StoreAudioState( AudioSource audioSource )
	{
		if( (audioSource == null) || !this.m_playStateMap.ContainsKey( audioSource ) )
		{
			return false;
		}
		
		AudioState audioState = this.m_playStateMap[ audioSource ];
		audioState.isPlaying = audioSource.isPlaying;
		audioState.timeSamples = audioSource.timeSamples;
		this.m_playStateMap[ audioSource ] = audioState;
		audioSource.Pause();
		return true;
	}
	
	private bool RestoreAudioState( AudioSource audioSource )
	{
		AudioState audioState;
		if( (audioSource == null) || !this.m_playStateMap.TryGetValue( audioSource, out audioState ) )
		{
			return false;
		}
		
		if( audioState.isPlaying )
		{
			audioSource.timeSamples = audioState.timeSamples;
			audioSource.Play();
			audioState.Clear();
			this.m_playStateMap[ audioSource ] = audioState;
		}
		return true;
	}
}
