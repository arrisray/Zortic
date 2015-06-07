using UnityEngine;

public class EntityCollisionMessage : Message
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

    public GameObject target = null;
    public GameObject collider = null;
	public Collision info = null;

	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS	

    public EntityCollisionMessage( GameObject target, GameObject collider, Collision info )
    {
        // /// Debug.Log( target.name + " (" + target.GetInstanceID() + "), " + collider.name + " (" + collider.GetInstanceID() + ")");
    	this.target = target;
    	this.collider = collider;
    	this.info = info;

    	// base.Send();
    }
}
