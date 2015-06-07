using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SegmentedCircleData
{
	
	public bool isAlwaysVisible = false;
	public bool allowPitchRoll = false;
	public Vector2 radius;
	public float thickness;
	public int dashSize;
	public int gapSize;
	public Transform center;
	public Transform attach;
	public Color color;
	public Material material;
	public Vector3 offset;
	public float followWeight;
	public Vectrosity.LineType lineType;
	public Vectrosity.Joins joinType;
	public float inTransitionPeriod;
	public float outTransitionPeriod;
	public float persistancePeriod;
	public float decrementDuration = 1.0f; //! @note In seconds.
	public Color decrementColor = Color.red;
	[HideInInspector] public int numSegments;
}
