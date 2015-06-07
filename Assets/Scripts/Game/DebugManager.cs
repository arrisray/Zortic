using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviourSingleton<DebugManager>
{
    // --------------------------------------------------------------------------
    // DATA MEMBERS

    public bool m_drawBoundingBoxes = false;
    public bool m_drawViewport = false;
    public bool m_drawVelocities = false;
	public bool m_drawCenterOfMass = false;

    // --------------------------------------------------------------------------
    // PROPERTIES

    // --------------------------------------------------------------------------
    // METHODS

	public void Update() 
	{
		if( this.m_drawViewport )
		{
			this.DrawViewport();	
		}
	}

	public void OnDrawGizmos()
	{
    	if( !GameManager.Instance )
    	{
			return;
		}

	    List<UnityEngine.Object> instances = GameManager.Instance.instances;
	    foreach( UnityEngine.Object instance in instances )
	  	{
			GameObject goInstance = instance as GameObject;
			if( !goInstance )
			{
				continue;
			}

        	EntityManager entity = goInstance.GetComponent<EntityManager>();
        	if( !entity )
        	{
        		continue;
        	}

			if( this.m_drawVelocities )
			{
				this.DrawVelocity( entity );
			}

			if( this.m_drawBoundingBoxes )
			{
				this.DrawBoundingBox( entity );
			}

			if( this.m_drawCenterOfMass )
			{
				this.DrawCenterOfMass( entity );
			}
	    }
	}

	protected void DrawVelocity( EntityManager entity )
	{
		if( entity.rigidbody )
		{
			Gizmos.color = Color.red;
            // Gizmos.DrawRay( entity.transform.position, entity.navigation.currentVelocity * 10.0f );  
            Gizmos.DrawRay( entity.transform.position, entity.rigidbody.velocity * 1.0f );   
		}
	}

	protected void DrawCenterOfMass( EntityManager entity ) 
    {
    	if( entity.data.movement.rigidbody )
    	{
    		Gizmos.color = Color.yellow;
    		Gizmos.DrawWireCube( entity.data.movement.rigidbody.worldCenterOfMass, Vector3.one * 10.0f ); 
    	}
    }

    protected void DrawBoundingBox( EntityManager entity ) 
    {
    	if( entity.data.visual.renderer )
    	{
    		Gizmos.color = Color.white;
    		Bounds bounds = entity.data.visual.renderer.bounds;
    		Gizmos.DrawWireCube( bounds.center, bounds.size ); 
    	}
    }

    protected void DrawViewport()
    {
		Camera camera = GameManager.Instance.sceneCamera;
        float z = 1000.0f; //! @hack Magic number!!!
        Vector3 tl = new Vector3( 0.0f, 1.0f, z ); 
        Vector3 tr = new Vector3( 1.0f, 1.0f, z ); 
        Vector3 bl = new Vector3( 0.0f, 0.0f, z ); 
        Vector3 br = new Vector3( 1.0f, 0.0f, z ); 

        tl = camera.ViewportToWorldPoint( tl );
        tr = camera.ViewportToWorldPoint( tr );
        bl = camera.ViewportToWorldPoint( bl );
        br = camera.ViewportToWorldPoint( br );

        /// Debug.DrawLine( tl, tr, Color.red, 0.0f, false );
        /// Debug.DrawLine( tr, br, Color.red, 0.0f, false );
        /// Debug.DrawLine( br, bl, Color.red, 0.0f, false );
        /// Debug.DrawLine( bl, tl, Color.red, 0.0f, false );
    }
}
