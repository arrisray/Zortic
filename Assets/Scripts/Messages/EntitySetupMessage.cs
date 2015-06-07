using UnityEngine;

public class EntityCreatedMessage : Message
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

    public GameObject entity = null;
    public GameObject creator = null;

	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS	

    public EntityCreatedMessage( GameObject entity, GameObject creator )
    {
    	this.entity = entity;
    	this.creator = creator;
    }
}
