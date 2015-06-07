using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Dialogue
{
	// --------------------------------------------------------------------------
    // ENUMS
	
	public enum PlaylistMode
	{
		All,
		Sequential,
		Random
	}
	
	public enum PlayMode
	{
		Play,
		Stop
	}
	
	// --------------------------------------------------------------------------
	// DELEGATES
	
	public delegate void CompletedEventHandler( System.Object sender, EventArgs eventArgs );
	
	// --------------------------------------------------------------------------
	// CLASSES
	
	[Serializable]
	public class WordBalloon
	{
		public UnityEngine.Object m_speaker = null;
		public float duration = 1.0f;
		public GameObject visual = null;
		public AudioClip audio = null;
		[HideInInspector] public Vector3 offset = Vector3.zero; //! @note In screen space, i.e. pixels.
		private GameObject m_speakerInstance = null;
		
		public bool isValid
		{
			get 
			{ 
				return ( !this.m_speaker || (this.m_speaker && this.m_speakerInstance) );
			}
		}
		
		public GameObject speaker
		{
			get { return this.m_speakerInstance; }
		}
		
		public GameObject UpdateSpeaker()
		{
			return this.UpdateSpeaker( false );
		}
		
		public GameObject UpdateSpeaker( bool force )
		{
			if( this.m_speaker && (!this.m_speakerInstance || force) )
			{
				this.m_speakerInstance = GameManager.Instance.FindInstanceByTag( this.m_speaker );
			}
			return this.m_speakerInstance;
		}
		
		public void Show()
		{
			this.UpdateSpeaker( true );
			this.visual.SetActive( true );
		}
		
		public void Hide()
		{
			this.visual.SetActive( false );
		}
	}
	
	// --------------------------------------------------------------------------
	// DATA MEMBERS
	
	public UnityEngine.Object m_defaultSpeaker = null;
	public string id = string.Empty;
	public PlaylistMode m_playlistMode = PlaylistMode.All;
	public List<WordBalloon> m_lines = new List<WordBalloon>();
	public event CompletedEventHandler Completed;
	private float m_elapsedTime = 0.0f;
	private int m_currentLine = -1;
	private float m_lastRealTime = 0.0f;
	private PlayMode m_currentPlayMode = PlayMode.Stop;
	
	// --------------------------------------------------------------------------
	// PROPERTIES
	
	public bool isPlaying
	{
		get { return this.m_currentPlayMode == PlayMode.Play; }
	}
	
	public WordBalloon currentLine
	{
		get 
		{ 
			return ( (this.m_currentLine < 0) || (this.m_currentLine >= this.m_lines.Count) )
				? null 
				: this.m_lines[ this.m_currentLine ]; 
		}
	}
	
	public PlaylistMode playlistMode
	{
		get { return this.m_playlistMode; }
		set { this.m_playlistMode = value; }
	}
	
	// --------------------------------------------------------------------------
	// METHODS
	
	public void Start()
	{
		// Initialize placement of word balloons relative to their speaker
		foreach( WordBalloon line in this.m_lines )
		{
			Vector3 zeroPos = GameManager.Instance.sceneCamera.WorldToScreenPoint( Vector3.zero );
			Vector3 pos = GameManager.Instance.guiCamera.WorldToScreenPoint( line.visual.transform.position );
			line.offset = pos - zeroPos;
			if( line.m_speaker == null )
			{
				line.m_speaker = this.m_defaultSpeaker;
			}
		}
		
		this.Reset();
	}
	
	public void Reset()
	{
		this.m_currentLine = -1;
		this.m_currentPlayMode = PlayMode.Stop;
		this.ClearCurrentLine ();
	}
	
	public void ClearCurrentLine()
	{
		this.m_elapsedTime = 0.0f;
		
		foreach( WordBalloon line in this.m_lines )
		{
			line.Hide();
		}
	}
	
	public void Play()
	{
		//! @hack WTF?!
		this.m_elapsedTime = 0.0f;
		
		// If another line is already playing, invoke completion cleanup
		// by manually advancing the dialogue; this is good practice because
		// it will raise the OnComplete event, as appropriate.
		if( this.isPlaying )
		{
			this.AdvanceDialogue();
		}
		
		switch( this.playlistMode )
		{
			case PlaylistMode.Sequential: 
			{
				// Only need to assign a value if a playlist hasn't started yet, otherwise
				// m_currentLine will already have been incremented after the last line
				// was finished being spoken...
				this.m_currentLine = (this.m_currentLine < 0) ? 0 : this.m_currentLine; 
				if( this.currentLine != null )
				{
					this.m_currentPlayMode = PlayMode.Play;
					this.currentLine.Show();
				}
				break;
			}
			case PlaylistMode.Random: 
			{
				// Generate an random index in the range: [0..Count)
				this.m_currentLine = (int)( Mathf.Round( UnityEngine.Random.value * (float)(this.m_lines.Count - 1) ) );
				this.m_currentPlayMode = PlayMode.Play;
				this.currentLine.Show();
				break;
			}
			case PlaylistMode.All: 
			default:
			{
				this.m_currentLine = 0;
				this.m_currentPlayMode = PlayMode.Play;
				this.currentLine.Show();
				break;
			}
		}
		
		// /// Debug.Log ( this.id + " current line: " + this.m_currentLine );
	}
	
	public void Stop()
	{
		this.ClearCurrentLine();
		
		this.m_currentPlayMode = PlayMode.Stop;
		this.m_currentLine = this.m_lines.Count;
		
		this.OnCompleted( EventArgs.Empty );
	}
	
	public void SkipLine()
	{
		if( (this.currentLine != null) && (this.m_currentPlayMode == PlayMode.Play) )
		{
			// this.m_elapsedTime = this.currentLine.duration;
			this.AdvanceDialogue();
		}
	}
	
	public bool Update()
	{
		float elapsed = Time.realtimeSinceStartup - this.m_lastRealTime;
		this.m_lastRealTime = Time.realtimeSinceStartup;
		
		if( (this.m_currentPlayMode != PlayMode.Play) || (this.currentLine == null) )
		{
			return false;
		}
		
		// If the speaker no longer exists, stop all the talking!
		if( !this.currentLine.isValid )
		{
			this.Stop ();
			return false;
		}
		
		// Update placement of word balloon relative to its speaker, if there is one
		if( this.currentLine.speaker )
		{
			GameObject speaker = this.currentLine.speaker;
			Vector3 p = GameManager.Instance.sceneCamera.WorldToScreenPoint( speaker.transform.position );
			p = GameManager.Instance.guiCamera.ScreenToWorldPoint( p + this.currentLine.offset );
			this.currentLine.visual.transform.position = p;
		}
		
		// Check if we need to progress the dialogue
		this.m_elapsedTime += elapsed;
		if( (this.currentLine.duration != 0.0f) && (this.m_elapsedTime >= this.currentLine.duration) )
		{
			this.AdvanceDialogue();
		}
		return true;
	}
	
	private void AdvanceDialogue()
	{
		++this.m_currentLine;	
		if( (this.playlistMode != PlaylistMode.All) || (this.m_currentLine >= this.m_lines.Count) )
		{
			this.Stop ();
		}
		else // if ( (this.playlistMode == PlaylistMode.All) && (this.m_currentLine < this.m_lines.Count) )
		{
			this.ClearCurrentLine ();
			this.currentLine.UpdateSpeaker( true );
			this.currentLine.Show();
		}
	}
	
	protected virtual void OnCompleted( EventArgs e ) 
	{
		if( this.Completed != null )
		{
			this.Completed( this, e );
		}
	}
}
