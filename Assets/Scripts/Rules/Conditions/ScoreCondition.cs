using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScoreCondition : GameRuleCondition
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

    [SerializeField] public int score = 0;
    [SerializeField] public bool isRecurring = true; //! @note If this is false, then it implies this condition only occurs once at the designated score.
    [SerializeField] protected int m_accumulatedScore = 0;

	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS

    public override void OnLoad()
    {
        // /// Debug.Log( this.name + ", Listening for player points message." );
    	MessageManager.Instance.RegisterMessageHandler<AssignPlayerPointsMessage>( this.OnPlayerPointsAssigned );
    }

    public override void OnUnload()
    {
		if( this.isRecurring )
        {
            this.m_accumulatedScore -= this.score;
        }
        MessageManager.Instance.UnregisterMessageHandler<AssignPlayerPointsMessage>( this.OnPlayerPointsAssigned );
    }

    public override void CreateInputs()
    {
    }

    public override void CreateOutputs()
    {
    }

    public void OnPlayerPointsAssigned( Message message )
    {
    	AssignPlayerPointsMessage derivedMessage = message as AssignPlayerPointsMessage;
        /// Debug.Log( GameManager.Instance.playerManager + " received " + derivedMessage.points + " points." );
        if( !this.isSatisfied && (this.m_accumulatedScore + derivedMessage.points >= this.score) )
        {
            this.isSatisfied = true;
        }

        this.m_accumulatedScore += derivedMessage.points;
    }
} // class ScoreCondition 