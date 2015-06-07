using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
	public UnityEngine.Object target = null;
	public float followWeight = 0.1f;
	private GameObject m_target = null;
	
	public void Start()
	{
	}
	
	public void Update()
	{
		if( target == null )
		{
			return;
		}
		
		if( this.m_target == null )
		{
			this.m_target = GameManager.Instance.FindInstanceByTag( this.target );
			if( this.m_target == null )
			{
				return;
			}
		}
		
		this.transform.position = Vector3.Lerp( this.transform.position, this.m_target.transform.position, this.followWeight );
	}
}

