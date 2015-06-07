using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableInputEvent : GameRuleEvent
{

    // --------------------------------------------------------------------------
    // DATA MEMBERS
	
	public UnityEngine.Object entity = null;
    public bool enableInput = true;

    // --------------------------------------------------------------------------
    // PROPERTIES
	
    // --------------------------------------------------------------------------
    // METHODS

    public override void OnRaise()
    {
		GameObject[] instances = GameManager.Instance.FindInstancesByTag( this.entity );
		foreach( GameObject instance in instances )
		{
			EntityManager entity = instance.GetComponent<EntityManager>();
			if( !entity ) { continue; }
			entity.enableInput = this.enableInput;
			
			// If disabling input events, clear out any current inputs 
			if( !this.enableInput )
			{
				entity.inputHandler.Reset ();
			}
		}
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
} // class EnableInputEvent