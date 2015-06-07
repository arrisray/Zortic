using UnityEngine;

public class AssignPlayerPointsMessage : Message
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

    public int points = 0;

	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS	

    public AssignPlayerPointsMessage( int points )
    {
        this.points = points;

        // base.Send();
    }
}
