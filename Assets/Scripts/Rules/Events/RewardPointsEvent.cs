using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardPointsEvent : GameRuleEvent
{
    // --------------------------------------------------------------------------
    // DATA MEMBERS

	// --------------------------------------------------------------------------
    // DATA MEMBERS

    public int points = 0;

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
        GameManager.Instance.playerManager.UpdateScore( points );
		this.OnComplete();
    }
} // class RewardPointsEvent