using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour 
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS
	
	public AudioClip menu = null;
	public AudioClip gameAsteroid = null;
	public AudioClip gameMinerPod = null;
	public AudioClip gamePirateFighter = null;
	public AudioClip gamePirateBase = null;
	public AudioClip credits = null;
	private List<AudioSource> m_themes = null;
	private AudioSource m_currentSong = null;
	
	// --------------------------------------------------------------------------
    // PROPERTIES
	
	// --------------------------------------------------------------------------
    // METHODS
	
	public void Setup()
	{
		this.m_themes = new List<AudioSource>();
		
		for( int i = 0; i < (int)GameManager.Levels.Count; ++i )
		{
			AudioSource audioSource = this.gameObject.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
			audioSource.loop = true;
			this.m_themes.Add( audioSource );
		}
		
		this.m_themes[ (int)GameManager.Levels.Menu ].clip = this.menu;
		this.m_themes[ (int)GameManager.Levels.GameAsteroids ].clip = this.gameAsteroid;
		this.m_themes[ (int)GameManager.Levels.GameMinerPods ].clip = this.gameMinerPod;
		this.m_themes[ (int)GameManager.Levels.GamePirateFighter ].clip = this.gamePirateFighter;
		this.m_themes[ (int)GameManager.Levels.GamePirateBase ].clip = this.gamePirateBase;
		this.m_themes[ (int)GameManager.Levels.Credits ].clip = this.credits;
	}
	
	public void Play( GameManager.Levels level )
	{
		AudioSource oldTheme = this.m_currentSong;
		AudioSource newTheme = this.m_themes[ (int)level ];
		this.StartCoroutine( this.Crossfade( oldTheme, newTheme ) );
	}
	
	public IEnumerator Crossfade( AudioSource oldTheme, AudioSource newTheme )
	{
		// No crossfade, just play the new clip!
		if( oldTheme == null )
		{
			newTheme.loop = true;
			newTheme.volume = 1.0f;
			newTheme.Play();
		}
		// Else...
		else
		{
			oldTheme.loop = false;
			
			newTheme.timeSamples = oldTheme.timeSamples;
			newTheme.loop = true;
			newTheme.volume = 0.0f;
			newTheme.Play();
			
			float remainingPercent = ( oldTheme.clip.length - oldTheme.time ) / oldTheme.clip.length;
			while( ( remainingPercent > 0.0f ) && oldTheme.isPlaying )
			{
				newTheme.volume = 1.0f - remainingPercent;
				oldTheme.volume = remainingPercent;
				yield return null;
				remainingPercent = ( oldTheme.clip.length - oldTheme.time ) / oldTheme.clip.length;
			}
			
			oldTheme.Stop();
			oldTheme.volume = 0.0f;
			
			newTheme.volume = 1.0f;
		}
		
		this.m_currentSong = newTheme;
	}
}
