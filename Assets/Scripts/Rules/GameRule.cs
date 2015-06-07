using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameRule : ScriptableObject
{		
	// --------------------------------------------------------------------------
    // DATA MEMBERS
 
	[SerializeField] public string id = string.Empty;
	[SerializeField] public bool enabled = true;
	[SerializeField] public bool isOneShot = false;
	[SerializeField] public GameRuleCondition condition = null;
	[SerializeField] public List<GameRuleEvent> events = new List<GameRuleEvent>();
	
	// --------------------------------------------------------------------------
    // METHODS
	
	public virtual void Start()
	{
		if( this.condition )
		{
			this.condition.OnStart();
		}
	}
	
	public void Reset()
	{
		if( this.condition )
		{
			this.condition.Reset();
		}
		
		foreach( GameRuleEvent gameEvent in this.events )
		{
			gameEvent.Reset();
		}
	}

	public void Load()
	{
		this.id = this.name;
		
		if( GameManager.Instance ) 
		{ 
			GameManager.Instance.RegisterGameRule( this ); 
		}
		
		if( this.condition != null )
		{
			this.condition.Load();
		}
		
		this.events.Sort( new GameRuleEvent.NameComparer() );
		foreach( GameRuleEvent gameEvent in this.events )
		{
			gameEvent.Load();
		}
	}

    public void Unload()
    {
        // this.Reset();

		if( this.condition != null )
		{
			this.condition.Unload();
		}
		
		foreach( GameRuleEvent gameEvent in this.events )
		{
			gameEvent.Unload();
		}

      	if( GameManager.Instance != null ) //! @hack !!!!!!!!!!
		{ 
			GameManager.Instance.UnregisterGameRule( this ); 
		}
    }
	
    public virtual bool Update()
    {
        if( !this.enabled )
        {
			// /// Debug.LogWarning( "Game rule disabled: " + this.name );
            return false;
        }

        if( (this.events.Count == 0) || (this.condition == null) )
        {
			/// Debug.LogWarning( "Please assign at least one condition and one event to your game rule ('" + this.name + "')." );
            return false;
        }
		
		foreach( GameRuleEvent gameEvent in this.events )
		{
			gameEvent.Update();
		}
		
		// /// Debug.Log ( "cnd=" + this.condition.name + ", cnd-sat?" + this.condition.isSatisfied );
        if( !this.condition.isSatisfied )
        {
			return false;
		}
		
		// Raise event
        // /// Debug.Log( this.condition.name + " condition is satisfied!" );
		foreach( GameRuleEvent gameEvent in this.events )
		{
			// /// Debug.Log ( "Processing game event: " + gameEvent.name );
			gameEvent.Raise ( this.condition.outputs );
		}
		
		// Handle post-events logic
        this.condition.OnPostEventRaised();
		foreach( GameRuleEvent gameEvent in this.events )
		{
        	gameEvent.OnPostEventRaised();
		}
		
		// Disable one-shot rules
		if( this.isOneShot )
		{
			this.enabled = false;
		}
		
		// Reset condition state
		this.condition.Reset();
        return true;
    }
} // class GameRule