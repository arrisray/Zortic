using UnityEngine;

public class AssignEntityDamageMessage : Message
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

    public GameObject attacker = null;
    public GameObject target = null;
    public float damage = 0.0f;

	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS	

    public AssignEntityDamageMessage( GameObject attacker, GameObject target, float damage )
    {
        // /// Debug.Log( attacker.name + " (" + attacker.GetInstanceID() + "), " + target.name + " (" + target.GetInstanceID() + "), damage=" + damage);
    	this.attacker = attacker;
    	this.target = target;
    	this.damage = damage;

    	// base.Send();
    }
}
