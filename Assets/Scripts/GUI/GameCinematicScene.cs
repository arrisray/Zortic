using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameCinematicScene : CinematicScene
{		
	// --------------------------------------------------------------------------
	// DATA MEMBERS
	
	public UnityEngine.Object speakerId = null;
	public Dialogue m_dialogue = new Dialogue();
	private bool m_isPlaying = false;
	private Rect m_comicStripRect;
	
	// --------------------------------------------------------------------------
    // PROPERTIES
	
	public override string id 
	{ 
		get { return this.m_dialogue.id; }
	}
	
	public override Dialogue dialogue 
	{ 
		get { return this.m_dialogue; }
	}
	
	public override bool isPlaying 
	{ 
		get { return this.m_isPlaying; } 
	}
	
	// --------------------------------------------------------------------------
	// METHODS
	
	public void Start()
	{
		//! @note We don't allow skipping of game cinematics, since they should be unobtrusive anyways!
		base.isInputEnabled = false; 
		this.m_dialogue.Completed += OnDialogueCompleted;
		
		this.Reset ();
		this.m_dialogue.Start ();
	}
	
	public void LateUpdate()
	{
		this.m_dialogue.Update();
	}
	
	public override void Reset()
	{
		this.m_dialogue.Reset ();
	}
	
	public override bool Play( Dialogue.PlaylistMode playlistMode )
	{
		if( this.m_isPlaying )
		{
			return false;
		}
		
		this.m_isPlaying = true;
		this.m_dialogue.playlistMode = playlistMode;
		this.m_dialogue.Play();
		return true;
	}
	
	public override bool Stop()
	{
		if( !this.m_isPlaying )
		{
			return false;
		}
		
		this.m_isPlaying = false;
		this.m_dialogue.Stop ();
		return true;
	}
	
	public override void SkipLine()
	{
		this.m_dialogue.SkipLine();
	}
	
	private void OnDialogueCompleted (object sender, EventArgs eventArgs)
	{
		this.OnCompleted();
	}
}
