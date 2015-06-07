using UnityEngine;

public class EntityAnimationEventMessage : Message
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

    public GameObject entity = null;
	public string animation = string.Empty;

	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS	

    public EntityAnimationEventMessage( GameObject entity, string animation )
    {
    	this.entity = entity;
		this.animation = animation;
    }
}
