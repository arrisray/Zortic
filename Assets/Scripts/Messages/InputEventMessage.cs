using System.Collections.Generic;
using UnityEngine;

public class InputEventMessage : Message
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

    public EntityInputHandler inputHandler = null;

	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS	

    public InputEventMessage( EntityInputHandler inputHandler )
    {
    	this.inputHandler = inputHandler;
    }
}
