using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ComicCinematicScene : CinematicScene
{		
	// --------------------------------------------------------------------------
	// DATA MEMBERS
	
	public UIRoot m_guiRoot = null;
	public UISprite m_comicStripSprite = null;
	public UIAnchor m_comicStripAnchor = null;
	public Dialogue m_dialogue = new Dialogue();
	public List<Transition> transitions = new List<Transition>();
	private bool m_isPlaying = false;
	private Rect m_comicStripRect;
	
	// --------------------------------------------------------------------------
    // PROPERTIES
	
	public override string id 
	{ 
		get { return this.gameObject.name; }
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
		// Register event handlers
		this.m_dialogue.Completed += new Dialogue.CompletedEventHandler( this.OnDialogueCompleted );
		this.m_dialogue.Start ();
		this.Reset ();
	}
	
	public void Update()
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
		GameManager.Instance.pause = true;
		this.m_dialogue.playlistMode = playlistMode;
		this.StartCoroutine( this.Show() );
		return true;
	}
	
	public override bool Stop()
	{
		if( !this.m_isPlaying )
		{
			return false;
		}
		
		this.m_isPlaying = false;
		this.StartCoroutine( this.Hide() );
		return true;
	}
	
	public override void SkipLine()
	{
		this.m_dialogue.SkipLine();
	}
	
	public void OnDialogueCompleted( System.Object sender, EventArgs eventArgs )
	{
		this.Stop ();
		this.OnCompleted();
	}
	
	protected IEnumerator Show()
	{
		base.isInputEnabled = false;
		foreach( Transition transition in this.transitions )
		{
			transition.Show();
			while( !transition.isShown )
			{
				yield return null;
			}
		}
		base.isInputEnabled = true;
		
		this.m_dialogue.Play();
	}
	
	protected IEnumerator Hide()
	{
		this.m_dialogue.Stop();
		
		base.isInputEnabled = false;
		for( int i = this.transitions.Count-1; i >= 0; --i )
		{
			Transition transition = this.transitions[i];
			transition.Hide();
			while( !transition.isHidden )
			{
				yield return null;
			}
		}
		base.isInputEnabled = true;
		
		GameManager.Instance.pause = false;
	}
}
