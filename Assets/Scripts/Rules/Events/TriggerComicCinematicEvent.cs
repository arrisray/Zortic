using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerComicCinematicEvent : GameRuleEvent
{
    // --------------------------------------------------------------------------
    // DATA MEMBERS

	// --------------------------------------------------------------------------
    // DATA MEMBERS

    public string cinematicId = string.Empty;
	public Dialogue.PlaylistMode playlistMode = Dialogue.PlaylistMode.All;

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
        GameManager.Instance.cinematicManager.PlayComicCinematicScene( this.cinematicId, this.playlistMode );
    }
} // class TriggerComicCinematic