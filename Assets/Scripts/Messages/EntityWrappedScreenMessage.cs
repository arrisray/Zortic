using UnityEngine;

public class EntityWrappedScreenMessage : Message
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

    public GameObject entity = null;

	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS	

    public EntityWrappedScreenMessage( GameObject entity )
    {
    	this.entity = entity;
    }
}
