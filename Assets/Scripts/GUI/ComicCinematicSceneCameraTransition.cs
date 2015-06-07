using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ComicCinematicSceneCameraTransition : Transition
{
	// --------------------------------------------------------------------------
    // STRUCTS

	[Serializable]
	struct CameraParams
	{
		public Vector3 position;
		public Quaternion rotation;
		public float fov;
		public float near;
		public float far;
		
		public CameraParams( GameObject gameObject, Camera camera )
		{
			this.position = gameObject.transform.position;
			this.rotation = gameObject.transform.rotation;
			this.fov = camera.fov;
			this.near = camera.near;
			this.far = camera.far;
		}
		
		public CameraParams( Camera camera )
		{
			this.position = camera.transform.position;
			this.rotation = camera.transform.rotation;
			this.fov = camera.fov;
			this.near = camera.near;
			this.far = camera.far;
		}
	}
	
	// --------------------------------------------------------------------------
    // MEMBERS
	
	public Camera sceneCamera = null;
	public UnityEngine.Object target = null;
	public UIPanel backdropPanel = null;
	private CameraParams 
		m_startCameraParams,
		m_endCameraParams;
	
	// --------------------------------------------------------------------------
    // METHODS
	
	public void Start()
	{
		this.backdropPanel.alpha = 0.0f;
	}
	
	public override void Show()
	{
		GameObject goTarget = GameManager.Instance.FindInstanceByTag( this.target );
		if( goTarget == null )
		{
			/// Debug.LogError ( "Unable to find scene camera transition target: " + this.target );
			return;
		}
		
		this.m_startCameraParams = new CameraParams( this.sceneCamera );
		this.m_endCameraParams = new CameraParams( goTarget, this.sceneCamera );
		this.m_endCameraParams.fov = this.m_startCameraParams.fov * 0.5f;
		
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
		this.sceneCamera.transform.position = Vector3.Lerp( this.m_startCameraParams.position, this.m_endCameraParams.position, value );
		this.sceneCamera.fov = Mathf.Lerp( this.m_startCameraParams.fov, this.m_endCameraParams.fov, value );
		this.backdropPanel.alpha = Mathf.Lerp( 0.0f, 1.0f, value );
	}
	
	protected override void OnHideUpdated( float value )
	{
		this.sceneCamera.transform.position = Vector3.Lerp( this.m_endCameraParams.position, this.m_startCameraParams.position, value );
		this.sceneCamera.fov = Mathf.Lerp( this.m_endCameraParams.fov, this.m_startCameraParams.fov, value );
		this.backdropPanel.alpha = Mathf.Lerp( 1.0f, 0.0f, value );
	}
}


