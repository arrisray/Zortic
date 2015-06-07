using System.Collections.Generic;
using UnityEngine;

public class EntityShootMessage : Message
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

    public EntityManager entity = null;

	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS	

    public EntityShootMessage( EntityManager entity )
    {
    	this.entity = entity;
    }
}
