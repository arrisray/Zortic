using UnityEngine;

public class EntityDestroyedMessage : Message
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

    public GameObject attacker = null;
    public GameObject victim = null;

	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS	

    public EntityDestroyedMessage( GameObject attacker, GameObject victim )
    {
    	this.attacker = attacker;
    	this.victim = victim;
    }
}
