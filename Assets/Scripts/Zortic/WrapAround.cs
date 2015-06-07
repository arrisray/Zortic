using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrapAround : MonoBehaviour
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

	private Camera m_sceneCamera = null;
	private Vector3 m_minBoundsViewPosition = Vector3.zero;
	private Vector3 m_maxBoundsViewPosition = Vector3.zero;
	private Vector3 m_basisLookAt = Vector3.zero;

	// --------------------------------------------------------------------------
    // PROPERTIES

	public EntityManager entity
	{
		get
		{
			return this.gameObject.GetComponent<EntityManager>();
		}
	}

	public Bounds bounds 
	{
		get
		{
			return this.entity && this.entity.data.visual.renderer
				? this.entity.data.visual.renderer.bounds
			 	: new Bounds( this.transform.position, Vector3.one );
		}
	}
 
	public bool isFacingViewport
	{
		get
		{
			this.m_basisLookAt = Vector3.zero - this.entity.rigidbody.position;
            // /// Debug.Log( this.name + " rigidbody velocity: " + this.entity.rigidbody.velocity );
			return ( Vector3.Dot(this.m_basisLookAt, this.entity.rigidbody.velocity) > 0.0f );
		}		
	}

	public bool isOutsideViewport
	{
		get
		{
			this.m_minBoundsViewPosition = this.m_sceneCamera.WorldToViewportPoint(this.bounds.min);
			this.m_maxBoundsViewPosition = this.m_sceneCamera.WorldToViewportPoint(this.bounds.max);
			return ( this.m_minBoundsViewPosition.x > 1.0f )
				|| ( this.m_maxBoundsViewPosition.x < 0.0f )
				|| ( this.m_minBoundsViewPosition.y > 1.0f )
				|| ( this.m_maxBoundsViewPosition.y < 0.0f );
		}
	}

	// --------------------------------------------------------------------------
    // METHODS

	public void Start()
	{
		this.m_sceneCamera = Camera.main; 
	}

	public void ManagedUpdate ()
	{
		if( this.isOutsideViewport && !this.isFacingViewport)
		{
			Vector3 currentPosition = this.m_sceneCamera.WorldToViewportPoint( this.transform.position );
			Vector3 boundsOffset = Vector3.zero;
			// /// Debug.Log( this.name + ", min=" + this.m_minBoundsViewPosition + ", max=" + this.m_maxBoundsViewPosition + ", current=" + currentPosition );

			if( this.m_maxBoundsViewPosition.x < 0.0f ) 
			{
				currentPosition.x = 1.0f;
				boundsOffset.x += this.bounds.extents.x;
			}
			else if( this.m_maxBoundsViewPosition.x > 1.0f)
			{
			  	currentPosition.x = 0.0f;
				boundsOffset.x -= this.bounds.extents.x;
			}

			if( this.m_maxBoundsViewPosition.y < 0.0f ) 
			{
				currentPosition.y = 1.0f;
				boundsOffset.z += this.bounds.extents.z;
			}
			else if( this.m_maxBoundsViewPosition.y > 1.0f)
			{
			  	currentPosition.y = 0.0f;
				boundsOffset.z -= this.bounds.extents.z;
			}

			// /// Debug.Log( this.name + ", new=" + currentPosition );
			// /// Debug.Log( this.name + ", bounds-offset=" + boundsOffset );

			this.transform.position = this.m_sceneCamera.ViewportToWorldPoint( currentPosition ); // + boundsOffset;
			
			// Raise event
			MessageManager.Instance.RaiseEvent( new EntityWrappedScreenMessage( this.gameObject ) );
		}
	}
}
