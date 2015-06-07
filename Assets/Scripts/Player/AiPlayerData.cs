using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AiPlayerData : MonoBehaviour
{
	// --------------------------------------------------------------------------
    // CLASSES

	[Serializable]
    public class Game
    {
		// public List<Steering> steerings = new List<Steering>();
		// public float awareness = 0.025f; //! @note Sense radius as percentage of viewport space.
		public UnityEngine.Object target = null;
		public float steeringPersistence = 5.0f; //! @note Max number of seconds a given steering mode will be in effect.
		public float attackPersistence = 5.0f; //! @note Max number of seconds an attack volley will last.
		public float threatDistance = 1.0f;
    }

	// --------------------------------------------------------------------------
    // MEMBERS

    public Game game = new Game();
} // public class AiPlayerData