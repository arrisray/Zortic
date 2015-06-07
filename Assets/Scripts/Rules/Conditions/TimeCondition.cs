using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCondition : GameRuleCondition
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

    [SerializeField] public float elapsedSeconds = 0.0f;
    [SerializeField] public bool isRecurring = false;
	[SerializeField] public bool isImmediate = true;

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
	
	public override void OnStart()
	{
		if( this.isImmediate )
			GameManager.Instance.StartCoroutine( this.Run() );
		else
			GameManager.Instance.StartCoroutine( this.WaitAndRun() );
	}

	public override void CreateInputs()
	{
	}

	public override void CreateOutputs()
	{
		Port port = new Port();
		port.id = "elapsed";
		port.type = EditorDataType.Float;
		this.m_outputs.Add( port );
	}
	
	private IEnumerator WaitAndRun()
    {
		yield return new WaitForSeconds( this.elapsedSeconds );
		GameManager.Instance.StartCoroutine( this.Run() );
	}

    private IEnumerator Run()
    {
    	this.isSatisfied = true;
    	while( this.isSatisfied )
    	{
    		yield return null;
    	}

    	if( this.isRecurring )
    	{
			yield return new WaitForSeconds( this.elapsedSeconds );
	        GameManager.Instance.StartCoroutine( this.Run() );
    	}
    }
} // class GameRuleCondition