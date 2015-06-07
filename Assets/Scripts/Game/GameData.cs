using System;
using UnityEngine;

[Serializable]
public class GameData : MonoBehaviour
{
	// --------------------------------------------------------------------------
    // public classES

	[Serializable]
	public class VisualData
	{
		public String gameOverMessage = "GAME OVER";
		public GUIStyle gameOverStyle = new GUIStyle();
		public GUIStyle scoreStyle = new GUIStyle();
	}

	// --------------------------------------------------------------------------
    // DATA MEMBERS

    public VisualData visual = new VisualData();

	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS
}