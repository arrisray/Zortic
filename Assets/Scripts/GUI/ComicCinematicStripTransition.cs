using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComicCinematicStripTransition : Transition
{
	// --------------------------------------------------------------------------
    // MEMBERS
	
	public GameObject comicStrip = null;
	private Vector2 offsets = new Vector2( 0.0f, 2.0f );
	
	// --------------------------------------------------------------------------
    // METHODS
	
	public void Start()
	{
		// Initialize comic strip position (off-screen)
		Vector3 p = this.comicStrip.transform.localPosition;
		p.x = offsets.y * 2.0f;
		this.comicStrip.transform.localPosition = p;
	}
	
	public override void Show()
	{
		base.Show();
	}
	
	public override void Hide()
	{
		base.Hide();
	}
	
	public override void Skip()
	{
		base.Skip();
	}
	
	protected override void OnShowUpdated( float value )
	{
		Vector3 p = this.comicStrip.transform.localPosition;
		p.x = Mathf.Lerp( offsets.y, offsets.x, value );
		this.comicStrip.transform.localPosition = p;
	}
	
	protected override void OnHideUpdated( float value )
	{
		Vector3 p = this.comicStrip.transform.localPosition;
		p.x = Mathf.Lerp( offsets.x, offsets.y, value );
		this.comicStrip.transform.localPosition = p;
	}
	
	public override void OnShowCompleted() 
	{
		base.OnShowCompleted();
	}
	
	public override void OnHideCompleted() 
	{
		base.OnHideCompleted();
	}
}

