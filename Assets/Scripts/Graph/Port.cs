using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TSDC;

[Serializable]
public class Port
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS
	
    [SerializeField] public string id = string.Empty;
    [SerializeField] public EditorDataType type = EditorDataType.Int;
    [SerializeField] public string data = string.Empty;
	[SerializeField] public string linkFromId = string.Empty;
} // end class Port

