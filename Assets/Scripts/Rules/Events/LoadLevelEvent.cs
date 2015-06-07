using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLevelEvent : GameRuleEvent
{

    // --------------------------------------------------------------------------
    // DATA MEMBERS

    public GameManager.Levels level;

    // --------------------------------------------------------------------------
    // PROPERTIES
	
	
    // --------------------------------------------------------------------------
    // METHODS

    public override void OnRaise()
    {
		GameManager.Instance.currentLevel = this.level;
		this.OnComplete();
    }

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
} // class LoadLevelEvent