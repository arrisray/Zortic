using UnityEngine;

public class EntityReceiveDamageMessage : Message
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

    public GameObject entity = null;
    public float damage = 0.0f;

	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS	

    public EntityReceiveDamageMessage( GameObject entity, float damage )
    {
        this.entity = entity;
        this.damage = damage;

        // base.Send();
    }
}
