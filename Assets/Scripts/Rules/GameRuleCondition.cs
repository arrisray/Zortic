using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class GameRuleCondition : GameRuleNode
{
	// --------------------------------------------------------------------------
    // CLASSES

	// --------------------------------------------------------------------------
    // DATA MEMBERS

    [SerializeField] [HideInInspector] protected bool m_isSatisfied = false;

	// --------------------------------------------------------------------------
    // PROPERTIES

    public virtual bool isSatisfied
    {
    	get { return this.m_isSatisfied; }
    	protected set { this.m_isSatisfied = value; }
    }

	// --------------------------------------------------------------------------
    // METHODS
	
	public override void Reset()
	{
		this.m_isSatisfied = false;
		base.Reset();
	}
} // class GameRuleCondition