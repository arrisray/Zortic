using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAudioTransition : Transition
{
	// --------------------------------------------------------------------------
    // MEMBERS
	
	public delegate void ShowFinished( EntityAudioTransition transition );
	public delegate void HideFinished( EntityAudioTransition transition );
		
	public AudioSource a, b = null;
	public event ShowFinished OnShowFinished;
	public event HideFinished OnHideFinished;
	
	// --------------------------------------------------------------------------
    // METHODS
	
	public void Start()
	{
		this.a.volume = 1.0f;
		this.b.volume = 1.0f;
	}
	
	public override void Show()
	{
		this.a.time = 0.0f;
		this.b.time = 0.0f;
		
		this.a.volume = 1.0f;
		this.b.volume = 0.0f;
		
		this.a.Play();
		this.b.Play();
		/// Debug.Log ( "PLAYING SOUNDS" );
		base.Show();
	}
	
	public override void Hide()
	{
		base.Hide();
	}
	//*
	public override void OnShowCompleted() 
	{
		base.OnShowCompleted();
		if( this.OnShowFinished != null )
		{
			this.OnShowFinished( this );
		}
	}
	
	public override void OnHideCompleted() 
	{
		base.OnHideCompleted();
		if( this.OnHideFinished != null )
		{
			this.OnHideFinished( this );
		}
	}
	//*/
	protected override void OnShowUpdated( float value )
	{
		this.a.volume = Mathf.Lerp( 1.0f, 0.0f, value );
		this.b.volume = Mathf.Lerp( 0.0f, 1.0f, value );
		/// Debug.Log ( this.a.volume + ", " + this.b.volume );
	}
	
	protected override void OnHideUpdated( float value )
	{
		this.a.volume = Mathf.Lerp( 0.0f, 1.0f, value );
		this.b.volume = Mathf.Lerp( 1.0f, 0.0f, value );
	}
}

