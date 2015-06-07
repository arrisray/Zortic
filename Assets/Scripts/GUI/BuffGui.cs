using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffGui : MonoBehaviour
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS
	
	public UILabel m_label = null;
	public GameObject m_entity = null;
	public float m_timeout = 3; //! @note In seconds.
	private bool m_isVisible = false;
	private float m_currentElapsed = 0.0f;
	
	// --------------------------------------------------------------------------
    // PROPERTIES
	
	public float timeout
	{
		get { return this.m_timeout; }
	}
	
	public string text
	{
		get { return this.m_label.text; }
		set { this.m_label.text = value; }
	}
	
	// --------------------------------------------------------------------------
    // METHODS
	
	public void Start()
	{
		
	}
	
	public void Show( string text )
	{
		this.text = text;
		this.m_currentElapsed = 0.0f;
		if( !this.m_isVisible )
		{
			this.m_isVisible = this.m_label.enabled = true;
			GameManager.Instance.StartCoroutine( this.Display() );
		}
	}
	
	public void Update()
	{
		if( this.m_label.enabled )
		{
			Vector3 p = GameManager.Instance.sceneCamera.WorldToScreenPoint( this.m_entity.transform.position );
			this.transform.position = GameManager.Instance.guiCamera.ScreenToWorldPoint( p );
		}
	}
	
	protected IEnumerator Display()
	{
		while( this.m_currentElapsed < this.m_timeout )
		{
			yield return null;
			this.m_currentElapsed += Time.deltaTime;
		}
		
		this.m_isVisible = this.m_label.enabled = false;
	}
}

