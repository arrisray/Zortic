using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // --------------------------------------------------------------------------
    // DATA MEMBERS
	
	public static string PlayerTagName = "Player";
    private PlayerData m_data = null;
    private int score = 0;
    private PlayerIndicators m_indicators = null;
    private EntityManager m_currentEntity = null;

    // --------------------------------------------------------------------------
    // PROPERTIES

    public PlayerData data
    { 
        get { return this.m_data; }
    }
    public PlayerIndicators indicators
    { 
        get { return this.m_indicators; }
    }
    public int currentScore
    { 
        get { return this.score; }
    }
	public EntityManager currentEntity
	{
		get { return this.m_currentEntity; }
		set { this.m_currentEntity = value; }
	}

    // --------------------------------------------------------------------------
    // METHODS

    public void Setup( PlayerData playerData, PlayerIndicators playerIndicators )
    {
        // this.StartCoroutine( this.Init() );
        this.m_data = playerData;
        this.m_indicators = playerIndicators;

        // Create initial entity
        if( this.m_data.game.entity )
        {
            UnityEngine.Object instance = GameManager.Instance.UnityInstantiate( this.m_data.game.entity, Vector3.zero, Quaternion.identity );  
            // yield return new WaitForEndOfFrame();
            EntityManager entity = (instance as GameObject).GetComponent<EntityManager>();
			if( entity )
			{
				this.currentEntity = entity;
			}
        }
    }

    public void Teardown()
    {

    }

    public void UpdateScore( int points )
    {
        // /// Debug.Log( "Player received points: " + points );

        score += points;
        if( score < 0 )
        {
            // levelTransition.GetComponent<Navigation>().GameOver();
        }
    } 
}
