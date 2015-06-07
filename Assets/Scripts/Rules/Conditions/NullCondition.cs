using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NullCondition : GameRuleCondition
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS
	
	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS

    public override void OnLoad()
    {
		this.isSatisfied = true;
    } 
   
    public override void OnUnload()
    {
    }

	public override void CreateInputs()
	{
	}

	public override void CreateOutputs()
	{
	}
} // class GameRuleCondition