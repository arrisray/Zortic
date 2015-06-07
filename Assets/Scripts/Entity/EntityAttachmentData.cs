using System;
using UnityEngine;

// --------------------------------------------------------------------------
//! @brief 
public class EntityAttachmentData : MonoBehaviour
{
	// --------------------------------------------------------------------------
    // ENUMS

    public enum Type
    {
    	Unknown,
    	Thruster,
    	WeaponSystem,
    };

	// --------------------------------------------------------------------------
    // DATA MEMBERS

    [HideInInspector] public EntityAttachmentData.Type attachmentType = Type.Unknown;
    // String name = String.Empty;
    // Vector3 position = Vector3.zero;
    // Quaternion rotation = Quaternion.identity;

    // --------------------------------------------------------------------------
    // PROPERTIES

    // --------------------------------------------------------------------------
    // METHODS
}
