using System;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
	public float m_backgroundRotationRate = 0.1f; //! @note Degrees per second.
	public ParticleSystem[] m_stars = null;

	public void Start () 
	{
	}

	public void Update () 
	{
		foreach (ParticleSystem stars in this.m_stars)
		{
			float deltaRotation = this.m_backgroundRotationRate * Time.deltaTime;
			stars.transform.Rotate(new Vector3(0.0f, 0.0f, deltaRotation));	
		}
	}
}
