using UnityEngine;

public class EntitySetupMessage : Message
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

    public GameObject entity = null;

	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS	

    public EntitySetupMessage( GameObject entity )
    {
    	this.entity = entity;
    }
}
