using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Transition : MonoBehaviour
{
	// --------------------------------------------------------------------------
    // MEMBERS
	
	public float duration = 1.0f;
	protected bool m_isShown = false;
	protected Hashtable 
		m_showParams = null,
		m_hideParams = null;
	
	// --------------------------------------------------------------------------
    // PROPERTIES
	
	public bool isShown 
	{ 
		get { return this.m_isShown; } 
	}
	
	public bool isHidden 
	{ 
		get { return !this.m_isShown; } 
	}
	
	// --------------------------------------------------------------------------
    // METHODS
	
	public virtual void Show()
	{
		this.CreateTweenParams();
		iTween.ValueTo( this.gameObject, this.m_showParams );
	}
	
	public virtual void Hide()
	{
		iTween.ValueTo( this.gameObject, this.m_hideParams );
	}
	
	public virtual void Skip() {}
	
	protected virtual void OnShowUpdated( float value )
	{
	}
	
	protected virtual void OnHideUpdated( float value )
	{
	}
	
	public virtual void OnShowCompleted() 
	{
		this.m_isShown = true;
	}
	
	public virtual void OnHideCompleted() 
	{
		this.m_isShown = false;
	}
	
	protected void CreateTweenParams()
	{
		// Init transition params
		this.m_showParams = iTween.Hash(
			"ignoretimescale", true,
			iT.ValueTo.from, 0.0f,
			iT.ValueTo.to, 1.0f,
			iT.ValueTo.time, this.duration,
			iT.ValueTo.easetype, iTween.EaseType.easeInSine, // easeInCirc,
			iT.ValueTo.onupdate, "OnShowUpdated",
			iT.ValueTo.onupdatetarget, this.gameObject,
			iT.ValueTo.oncomplete, "OnShowCompleted",
			iT.ValueTo.oncompletetarget, this.gameObject
			);
		
		this.m_hideParams = iTween.Hash(
			"ignoretimescale", true,
			iT.ValueTo.from, 0.0f,
			iT.ValueTo.to, 1.0f,
			iT.ValueTo.time, this.duration,
			iT.ValueTo.easetype, iTween.EaseType.easeOutSine, // easeOutCirc,
			iT.ValueTo.onupdate, "OnHideUpdated",
			iT.ValueTo.onupdatetarget, this.gameObject,
			iT.ValueTo.oncomplete, "OnHideCompleted",
			iT.ValueTo.oncompletetarget, this.gameObject
			);
	}
}

