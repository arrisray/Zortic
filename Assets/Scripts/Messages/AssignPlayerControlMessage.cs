using UnityEngine;

public class AssignPlayerControlMessage : Message
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

    public GameObject oldPlayer = null;
	public GameObject newPlayer = null;

	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS	

    public AssignPlayerControlMessage( GameObject newPlayer, GameObject oldPlayer )
    {
		this.oldPlayer = oldPlayer;
        this.newPlayer = newPlayer;
    }
}
