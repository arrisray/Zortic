using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;
using TSDC;

public class SegmentedCircle : MonoBehaviour
{
	// --------------------------------------------------------------------------
    // STRUCTS
	
	public class TransitionParams
	{
		// --------------------------------------------------------------------------
		// DATA MEMBERS
		
		#region Data Members
		
		public float lerpFactor = 0.0f;
		public float duration = 0.1f; //! @note In seconds.
		
		public float alphaOffset = 0.0f;
		public float radiusOffset = 0.0f;
		public float rotationOffset = 0.0f;
		public float thicknessOffset = 0.0f;
		
		public float currentThickness = 0.0f;
		public float currentRadius = 0.0f;
		public float currentAlpha = 0.0f;
		public float currentRotation = 0.0f;
		
		public float endRotation = 0.0f;
		public float endRadius = 0.0f;
		public float endAlpha = 0.0f;
		public float endThickness = 0.0f;

		protected SegmentedCircleData data = null;

		#endregion

		// --------------------------------------------------------------------------
		// METHODS
		
		#region Methods

		public TransitionParams( SegmentedCircleData data )
		{
			this.data = data;

			this.alphaOffset = 0.0f; // this.data.color.a;
			this.rotationOffset = 120.0f; //! @hack HARDCODED VALUE!!!
			this.radiusOffset = this.data.radius.x;
			this.thicknessOffset = this.data.thickness;
			
			this.endRadius = this.data.radius.y; // this.currentRadius = this.radiusOffset;
			this.endAlpha = 1.0f; // this.currentAlpha = this.alphaOffset;
			this.endThickness = this.data.thickness; // this.currentThickness = this.thicknessOffset;
			this.endRotation = this.currentRotation = this.rotationOffset;
		}
		
		#endregion
	}

	// --------------------------------------------------------------------------
    // DATA MEMBERS
	
	private int m_numVisibleSegments = 0;
	private string m_id = string.Empty;
	private SegmentedCircleData m_data = null;
	private List<VectorLine> m_segments = new List<VectorLine>();
	private Vector3 m_currentPosition = Vector3.zero;
	private TransitionParams m_transitionEndpoints = null;
	protected Hashtable 
		m_showParams = null,
		m_hideParams = null,
		m_decrementParams = null;
	
	// --------------------------------------------------------------------------
    // PROPERTIES

	#region Properties

	public bool isValid
	{
		get
		{
			return ( this.m_data.attach != null )
				&& ( this.m_data.numSegments > 0 )
				&& ( this.m_data.dashSize > 0 )
				&& ( this.m_data.radius.sqrMagnitude > 0.0f )
				&& ( this.m_data.thickness > 0 );
		}
	}
	
	public int numSegments 
	{ 
		get { return this.m_data.numSegments; }
		set { this.m_data.numSegments = value; }
	}
	
	public int numVisibleSegments 
	{ 
		get
		{
			return this.m_numVisibleSegments;
		}
		set
		{
			this.m_numVisibleSegments = value;
			this.m_numVisibleSegments = Mathf.Min( this.numSegments, this.m_numVisibleSegments );
			this.m_numVisibleSegments = Mathf.Max( 0, this.m_numVisibleSegments );
		}
	}
	
	#endregion

	// --------------------------------------------------------------------------
    // METHODS

	public void Init( SegmentedCircleData data )
	{
		this.m_data = data;
		this.m_transitionEndpoints = new TransitionParams( this.m_data );
		this.numVisibleSegments = this.m_data.numSegments;

		this.m_id = Utils.GetUniqueID();

		if( this.isValid )
		{
			if( this.m_data.center )
				this.m_currentPosition = this.m_data.center.position;
			this.CreateTweenParams();
			this.CreateCircle();
		}
	}

	public void Refresh( float lerpFactor )
	{
		// Update current transition params
		this.m_transitionEndpoints.lerpFactor = lerpFactor;
		this.m_transitionEndpoints.currentAlpha = Mathf.Lerp( this.m_transitionEndpoints.alphaOffset, this.m_transitionEndpoints.endAlpha, lerpFactor );
		this.m_transitionEndpoints.currentRadius = Mathf.Lerp( this.m_transitionEndpoints.radiusOffset, this.m_transitionEndpoints.endRadius, lerpFactor );
		this.m_transitionEndpoints.currentRotation = Mathf.Lerp( this.m_transitionEndpoints.rotationOffset, this.m_transitionEndpoints.endRotation, lerpFactor );
		this.m_transitionEndpoints.currentThickness = Mathf.Lerp( this.m_transitionEndpoints.thicknessOffset, this.m_transitionEndpoints.endThickness, lerpFactor );
		
		/*
		/// Debug.Log( this.name + "={\n"
			+ "alpha: " + this.m_transitionEndpoints.currentAlpha + ",\n"
			+ "radius: " + this.m_transitionEndpoints.currentRadius + ",\n"
			+ "rotation: " + this.m_transitionEndpoints.currentRotation + ",\n"
			+ "thickness: " + this.m_transitionEndpoints.currentThickness + "\n"
			+ "}" );
			//*/
		
		// Update target color alpha
		Color c = this.m_data.color;
		c.a = this.m_transitionEndpoints.currentAlpha;
		this.m_data.color = c;
		
		for( int i = 0; i < this.m_segments.Count; ++i )
		{
			VectorLine segment = this.m_segments[ i ];
			Vector3[] points = this.GetSegmentPoints( i );
			segment.ZeroPoints();
			segment.Resize( points );
			segment.SetColor( this.m_data.color );

			float[] widths = new float[ segment.points3.Length - 1 ];
			widths.Populate( this.m_data.thickness );
			segment.SetWidths( widths );
		}
	}

	public void Teardown()
	{
		for( int i = 0; i < this.m_segments.Count; ++i )
		{
			VectorLine segment = this.m_segments[ i ];
			VectorLine.Destroy( ref segment );
		}
		this.m_segments.Clear();
	}

	public void Snap()
	{
		this.m_currentPosition = this.m_data.center.position;
	}

	public void ManagedUpdate()
	{
	}
	
	public void ManagedLateUpdate()
	{
		this.UpdatePosition();
		/*
		if( !this.m_data.isAlwaysVisible && !this.isValid )
		{
			return;
		}
		*/
		this.DrawSegments();
	}

	public void Show( bool isVisible )
	{
		if( this.m_showParams == null || this.m_hideParams == null )
		{
			// /// Debug.Log( this.gameObject.name + ": INVALID" );
			return;
		}
		
		// Show
		if( isVisible )
		{
			this.StopCoroutine( "Countdown" );
			this.StartCoroutine( "Countdown" );
			
			if( this.m_transitionEndpoints.lerpFactor == 0.0f )
				iTween.ValueTo( this.gameObject, this.m_showParams );
		}
		// Hide
		else
		{
			if( this.m_transitionEndpoints.lerpFactor == 1.0f )
				iTween.ValueTo( this.gameObject, this.m_hideParams );
		}
	}
	
	public void Decrement( bool isAnimated )
	{
		if( !isAnimated )
		{
			--this.numVisibleSegments;
			return;
		}
	}
	
	protected void CreateTweenParams()
	{
		// Init transition params
		this.m_showParams = iTween.Hash(
			"ignoretimescale", true,
			iT.ValueTo.from, 0.0f,
			iT.ValueTo.to, 1.0f,
			iT.ValueTo.time, this.m_data.inTransitionPeriod,
			iT.ValueTo.easetype, iTween.EaseType.easeInSine, // easeInCirc,
			iT.ValueTo.onupdate, "OnShowUpdated",
			iT.ValueTo.onupdatetarget, this.gameObject
			);
		
		this.m_hideParams = iTween.Hash(
			"ignoretimescale", true,
			iT.ValueTo.from, 1.0f,
			iT.ValueTo.to, 0.0f,
			iT.ValueTo.time, this.m_data.outTransitionPeriod,
			iT.ValueTo.easetype, iTween.EaseType.easeOutSine, // easeOutCirc,
			iT.ValueTo.onupdate, "OnHideUpdated",
			iT.ValueTo.onupdatetarget, this.gameObject
			);
		
		this.m_decrementParams = iTween.Hash(
			"ignoretimescale", true,
			iT.ValueTo.from, 1.0f,
			iT.ValueTo.to, 0.0f,
			iT.ValueTo.time, this.m_data.decrementDuration,
			iT.ValueTo.easetype, iTween.EaseType.easeOutSine, // easeOutCirc,
			iT.ValueTo.onupdate, "OnDecrementUpdated",
			iT.ValueTo.onupdatetarget, this.gameObject
			);
	}
	
	protected virtual void OnShowUpdated( float value )
	{
		this.Refresh( value );
	}
	
	protected virtual void OnHideUpdated( float value )
	{
		this.Refresh( value );
	}
	
	protected virtual void OnDecrementUpdated( float value )
	{
		
	}
	
	protected IEnumerator Countdown()
	{
		yield return new WaitForSeconds( this.m_data.persistancePeriod );
		this.Show ( false );
	}

	private bool UpdateTransition()
	{
		return true;
	}

	protected void UpdatePosition()
	{
		if( !this.m_data.center || !this.m_data.attach )
		{
			return;
		}
		
		if( this.m_data.followWeight == 0.0f )
		{
			if( !this.m_data.allowPitchRoll )
			{
				this.m_data.attach.rotation = Quaternion.identity;
			}
		}
		else
		{
			this.m_currentPosition = Vector3.Lerp( this.m_currentPosition, this.m_data.center.position, this.m_data.followWeight );
			if( this.m_data.attach )
			{
				this.m_data.attach.position = this.m_currentPosition;
			}
			
			if( !this.m_data.allowPitchRoll && this.m_data.attach && this.m_data.center )
			{
				this.m_data.attach.rotation = Quaternion.identity;
			}
		}
	}

	protected void DrawSegments()
	{
		// Prep VectorLine for drawing
		bool isOrtho = !this.m_data.allowPitchRoll;
		VectorLine.SetCamera( Camera.mainCamera, isOrtho );
		if( this.m_segments.Count == 0 || !this.enabled )
		{
			return;
		}
		
		// Draw and show visible segments
		for( int i = 0; i < this.numVisibleSegments; ++i )
		{
			VectorLine segment = this.m_segments[ i ];
			segment.active = true;

			segment.SetColor( this.m_data.color );
			segment.Draw( this.m_data.attach );
		}
		
		// Draw and hide invisible segments
		for( int i = this.numVisibleSegments; i < this.numSegments; ++i )
		{
			VectorLine segment = this.m_segments[ i ];
			segment.active = false;
		}
	}

	protected bool CreateCircle()
	{
		if( this.m_data.numSegments == 0 || this.m_data.dashSize == 0 )
		{
			// /// Debug.LogWarning( "Requested an invalid SegmentedCircle (=" + this.m_id + ") because either segment or dash size was equal to 0!" );
			return false;
		}

		for( int i = 0; i < this.numSegments; ++i )
		{
			VectorLine segment = this.CreateSegment( i );
			this.m_segments.Add( segment );
		}
		return true;
	}

	protected VectorLine CreateSegment( int index )
	{
		Vector3[] points = this.GetSegmentPoints( index );
		
		string segmentId = this.m_id + "-segment-" + index;
		VectorLine segment = new VectorLine( segmentId, points, this.m_data.color, this.m_data.material, this.m_data.thickness, this.m_data.lineType, Joins.Weld );
		segment.vectorObject.transform.parent = GameManager.Instance.runtimeIndicatorsNode.transform;
		return segment;
	}

	protected Vector3[] GetSegmentPoints( int segmentIndex )
	{
		Vector3 center = Vector3.zero;
		List<Vector3> points = new List<Vector3>();
		
		int numSubSegments = this.m_data.dashSize + this.m_data.gapSize;
		float segmentSize = ( 360.0f / ( this.numSegments * (float)numSubSegments ) ) * Mathf.Deg2Rad;
		float segmentStart = segmentIndex * segmentSize * (float)numSubSegments;
		
		for( int i = 0; i < this.m_data.dashSize + 1; ++i )
		{
			int arcIndex = i;
			float arcRadians = segmentStart + ( segmentSize * arcIndex );
			
			Vector3 point = center + this.m_data.offset;
			point.x += this.m_transitionEndpoints.currentRadius * Mathf.Sin( arcRadians );
			point.z += this.m_transitionEndpoints.currentRadius * Mathf.Cos( arcRadians );

			points.Add( point );
		}

		return points.ToArray();
	}
}
