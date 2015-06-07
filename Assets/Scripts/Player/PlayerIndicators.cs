using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent( typeof(PlayerManager) )]
public class PlayerIndicators : MonoBehaviour
{
    // --------------------------------------------------------------------------
    // DATA MEMBERS

	private PlayerManager m_player = null; // GameManager.Instance.playerManager;
    private GameManager m_gameManager = null;

    // --------------------------------------------------------------------------
    // METHODS

	public void Start()
	{
        this.m_gameManager = GameManager.Instance;
		this.m_player = GameManager.Instance.playerManager;
        /*
        /// Debug.Log( this.m_gameManager );
        /// Debug.Log( this.m_gameManager.data );
        /// Debug.Log( this.m_gameManager.data.visual );
        //*/
		// this.m_player = this.gameObject.GetComponent<PlayerManager>();
	}

	public void Update()
	{
	}

	public void Reset()
	{
	}


	public void OnGUI()
	{
		// float livesGUISize = 140.0f;
		// Vector2 playerScreenPos = PlayerScreenPos();

		// Display score
		if( this.m_player != null && this.m_gameManager != null && this.m_gameManager.data != null )
		{
			GUI.Label(new Rect(20,20,200,200), this.m_player.currentScore + "", this.m_gameManager.data.visual.scoreStyle);
		}
	}

	/*
	public Vector2 PlayerScreenPos() 
	{
		if (Camera.current)
		{
			Vector2 playerScreenPos = Camera.current.WorldToScreenPoint(this.m_player.transform.position);
			playerScreenPos.y = Screen.height - playerScreenPos.y;
			return playerScreenPos;
		}
		return Vector2.zero;
	}
	*/
}
