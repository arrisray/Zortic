using UnityEngine;

public class PlayerScoreChangedMessage : Message
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

    public GameObject entity = null;
    public float score = 0;

	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS	

    public PlayerScoreChangedMessage( GameObject entity, float score )
    {
    	this.entity = entity;
    	this.score = score;

    	// base.Send();
    }
}
