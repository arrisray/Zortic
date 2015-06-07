using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayLevelMusicEvent : GameRuleEvent
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
		if( GameManager.Instance.musicManager != null )
		{
			GameManager.Instance.musicManager.Play( this.level );
		}
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
} // class PlayLevelMusicEvent