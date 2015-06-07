using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableGameRuleEvents : GameRuleEvent
{

    // --------------------------------------------------------------------------
    // DATA MEMBERS

    public List<GameRule> gameRules = new List<GameRule>();

    // --------------------------------------------------------------------------
    // PROPERTIES
	
	
    // --------------------------------------------------------------------------
    // METHODS

    public override void OnLoad()
    {
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
	
	public override void OnRaise()
    {
		foreach( GameRule rule in this.gameRules )
		{
			// /// Debug.Log ( this.name );
			rule.enabled = true;
		}
		
		this.OnComplete();
    }
} // class RewardPointsEvent