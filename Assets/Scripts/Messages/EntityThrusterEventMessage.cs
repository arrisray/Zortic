using UnityEngine;

public class EntityThrusterEventMessage : Message
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

    public GameObject entity = null;
	public EntityThruster.EventType eventType = EntityThruster.EventType.Idle;

	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS	

    public EntityThrusterEventMessage( GameObject entity, EntityThruster.EventType eventType )
    {
    	this.entity = entity;
		this.eventType = eventType;
    }
}
