using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CinematicManager : MonoBehaviourSingleton<CinematicManager> 
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS
	
	public Camera m_sceneCamera = null;
	public UIRoot m_guiRoot = null;
	public List<GameCinematicScene> m_gameScenes = new List<GameCinematicScene>();
	public List<ComicCinematicScene> m_cinematicScenes = new List<ComicCinematicScene>();
	private List<GameCinematicScene> m_currentGameCinematicScenes = new List<GameCinematicScene>();
	private ComicCinematicScene m_currentComicCinematicScene = null;
	
	// --------------------------------------------------------------------------
    // PROPERTIES
	
	public List<CinematicScene> scenes 
	{
		get
		{
			List<CinematicScene> scenes = new List<CinematicScene>();
			scenes.AddRange( this.m_gameScenes.Cast<CinematicScene>() );
			scenes.AddRange( this.m_cinematicScenes.Cast<CinematicScene>() );
			return scenes;
		}
	}
	
	// --------------------------------------------------------------------------
    // METHODS
	
	public void Load()
	{
		
	}
	
	public void Unload()
	{
		List<CinematicScene> scenes = this.scenes;
		foreach( CinematicScene scene in scenes )
		{
			scene.Reset();
		}
	}
	
	public void OnGUI()
	{
		List<CinematicScene> scenes = this.scenes;
		foreach( CinematicScene scene in scenes )
		{
			scene.OnGUI();
		}
	}
	
	public void Update () 
	{
	}
	
	public CinematicScene FindCinematicScene( string id )
	{
		List<CinematicScene> scenes = this.scenes;
		foreach( CinematicScene scene in scenes )
		{
			if( scene.id == id )
			{
				return scene;
			}
		}
		return null;
	}
	
	public bool PlayGameCinematicScene( string id, Dialogue.PlaylistMode playlistMode )
	{
		GameCinematicScene scene = this.FindCinematicScene( id ) as GameCinematicScene;
		if( scene == null )
		{
			/// Debug.Log( "Couldn't find game cinematic scene with ID: " + id );
			return false;
		}
		
		if( this.m_currentGameCinematicScenes.Contains( scene ) )
		{
			scene.Stop();
		}
		else
		{
			this.m_currentGameCinematicScenes.Add( scene );
		}
		
		return scene.Play( playlistMode );
	}
	
	public bool PlayComicCinematicScene( string id, Dialogue.PlaylistMode playlistMode )
	{
		if( this.m_currentComicCinematicScene != null )
		{
			this.m_currentComicCinematicScene.Stop();
		}
		
		CinematicScene scene = this.FindCinematicScene( id );
		if( scene != null )
		{
			this.m_currentComicCinematicScene = scene as ComicCinematicScene;
			return scene.Play( playlistMode );
		}
		return false;
	}
}
