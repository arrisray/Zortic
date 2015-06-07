using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CinematicScene : MonoBehaviour
{
	// --------------------------------------------------------------------------
    // TYPES
	
	public delegate void CompletedDelegate( CinematicScene source, EventArgs e );
	public event CompletedDelegate Completed = null;
	
	// --------------------------------------------------------------------------
	// PROPERTIES
	
	public abstract string id { get; }
	public abstract Dialogue dialogue { get; }
	public abstract bool isPlaying { get; }
	public bool isInputEnabled { get; set; }
	
	// --------------------------------------------------------------------------
	// METHODS
	
	public abstract bool Play( Dialogue.PlaylistMode playlistMode );
	public abstract bool Stop();
	public abstract void SkipLine();
	public abstract void Reset();
	
	public void OnGUI()
	{
		if( !this.isInputEnabled ) 
		{
			return; 
		}
		
		// if( Input.GetKeyDown( KeyCode.Space ) )
		// {
		// 		this.SkipLine();
		// }
		if( Input.GetKeyDown( KeyCode.Space ) )
		{
			this.Stop();
		}
	}
	
	public void OnCompleted()
	{
		if( this.Completed != null )
		{
			this.Completed( this, new EventArgs() );
		}
	}
}
