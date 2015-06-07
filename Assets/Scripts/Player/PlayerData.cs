using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData : MonoBehaviour
{
    // --------------------------------------------------------------------------
    // public classES

	[Serializable]
	public class GameData
	{
		public GameObject entity = null;
	}

    // --------------------------------------------------------------------------
    // DATA MEMBERS

	public GameData game = new GameData();

    // --------------------------------------------------------------------------
    // METHODS

	public void Awake()
	{
		// /* PlayerManager manager = */this.gameObject.AddComponent<PlayerManager>();
	}
}
