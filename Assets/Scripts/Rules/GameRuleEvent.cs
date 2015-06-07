using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameRuleEvent : GameRuleNode
{
	// --------------------------------------------------------------------------
    // TYPES
	
	public delegate void EventCompletedDelegate( GameRuleEvent source, EventArgs e );
	
	// --------------------------------------------------------------------------
    // CLASSES
	
	public class NameComparer : IComparer<GameRuleEvent>
    {
        public virtual int Compare( GameRuleEvent a, GameRuleEvent b )
        {
            return a.name.CompareTo( b.name );
        }
    }
	
	// --------------------------------------------------------------------------
    // DATA MEMBERS
	
	public event EventCompletedDelegate Completed = null;

	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS
	
	public abstract void OnRaise();

    public void Raise( List<Port> inputs )
    {
        // Copy input port data from condition outputs
        foreach( Port eventPort in this.m_inputs )
        {
            foreach( Port conditionPort in inputs )
            {
                // /// Debug.Log( "Comparing: " + conditionPort.id + ", " + eventPort.linkFromId);
                if (conditionPort.id == eventPort.linkFromId)
                {
                    eventPort.data = conditionPort.data;
                    continue;
                }
            }
        }
    
        // Execute custom event logic
        this.OnRaise();
    }
	
	protected virtual void OnComplete()
	{
		if( this.Completed != null )
		{
			this.Completed( this, new EventArgs() );
		}
	}
} // class GameRuleEvent