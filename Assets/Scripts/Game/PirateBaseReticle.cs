using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateBaseReticle : MonoBehaviour
{
    // --------------------------------------------------------------------------
    // DATA MEMBERS
	
	public GameObject reticlePrefab = null;
	public Vector3 reticleOffset = new Vector3( 0.0f, 500.0f, 0.0f );
	private GameObject m_weapon = null;
	private GameObject m_reticle = null;

    // --------------------------------------------------------------------------
    // PROPERTIES
	
	

    // --------------------------------------------------------------------------
    // METHODS
	
	public void Start()
	{
		this.m_weapon = this.gameObject;
		
		if( this.reticlePrefab )
		{
			this.m_reticle = GameManager.Instance.UnityInstantiate( this.reticlePrefab, Vector3.zero, Quaternion.identity ) as GameObject;
			this.m_reticle.transform.parent = this.m_weapon.transform;
			this.m_reticle.transform.localPosition = this.reticleOffset;
		}
	}
	
	public void Update()
	{
		this.m_reticle.transform.localRotation = Quaternion.Inverse ( this.m_weapon.transform.rotation );
	}

} // end class PirateBaseReticle
