using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[RequireComponent( typeof(MeshFilter) )]
[RequireComponent( typeof(MeshRenderer) )]
// --------------------------------------------------------------------------
//! @brief 
public class EntityThrusterData : EntityAttachmentData
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS
	
	public bool isAudioMainThruster = false;
	public bool isAudioRetroThruster = false;
	public Vector2 activationSpeedRange = Vector2.zero; //! @note As a percentage of max speed.
    public List<Utils.Direction> thrustDirection = new List<Utils.Direction>(); //! @note The direction to which this thruster will respond.
    public Utils.Direction strafeDirection = Utils.Direction.Unknown; //! @note The direction to which this thruster will respond.
	public bool isStabilizer = true;
	public float stabilizeDuration = 0.5f; //! @note In seconds.
	public ParticleSystem particles = null;
	
	public int thrustDirectionMask
	{
		get 
		{
			int mask = 0;
			for( int i = 0; i < this.thrustDirection.Count; ++i )
			{
				mask |= (int)this.thrustDirection[i];
			}
			return mask;
		}
	}

    // --------------------------------------------------------------------------
    // PROPERTIES

    // --------------------------------------------------------------------------
    // METHODS

    public void OnEnable()
    {
    	base.attachmentType = EntityAttachmentData.Type.Thruster;
    }
}
