using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CinematicSceneCompletedCondition : GameRuleCondition
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS
	
	public List<string> m_sceneIds = new List<string>();
	
	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS

	private void OnCinematicSceneCompleted( CinematicScene source, EventArgs e )
	{
		this.m_isSatisfied = true;
	}

    public override void OnLoad()
    {
		foreach( string sceneId in this.m_sceneIds )
		{
			CinematicScene scene = GameManager.Instance.cinematicManager.FindCinematicScene( sceneId );
			if( scene != null )
			{
				scene.Completed += OnCinematicSceneCompleted;
			}
		}
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
} // class EventCompletedCondition

