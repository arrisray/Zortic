using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//! @ref http://answers.unity3d.com/questions/21664/is-it-possible-to-have-multiple-tags-or-the-like-o.html
[Serializable]
public class EntityTag : ScriptableObject
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

	public List<EntityTag> parents = new List<EntityTag>();

	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS

	public static bool IsEqual( EntityTag lhs, EntityTag rhs )
	{
		// string debug = "[EntityTag] lhs:" + lhs.name + " == rhs:" + rhs.name + " ? ";		
		if( lhs == rhs )
		{
			// debug += "TRUE";
			// /// Debug.Log( debug );
			return true;
		}

		foreach( EntityTag lhsParentTag in lhs.parents )
		{
			if( EntityTag.IsEqual( lhsParentTag, rhs ) )
			{
				// debug += "TRUE";
				// /// Debug.Log( debug );
				return true;
			}
		}
		
		// debug += "FALSE";
		// /// Debug.Log( debug );
		return false;
	}
} // class MonoTag