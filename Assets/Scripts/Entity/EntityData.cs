using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EntityData : MonoBehaviour 
{
    // --------------------------------------------------------------------------
    // CLASSES

	[Serializable]
	public class VisualData 
	{
		public MeshRenderer renderer = null;
		public Texture2D energyCurImage = null;
		public Material outlineMaterial = null;
		public GameObject collisionParticles; // explosion effect
		public GameObject destroyedParticles;
		public List<GameObject> debris = new List<GameObject>();
		public SegmentedCircleData healthIndicator = new SegmentedCircleData();
		public SegmentedCircleData powerIndicator = new SegmentedCircleData();
		public List<DebugInfoIndicator> debugInfoIndicators = new List<DebugInfoIndicator>();
		[HideInInspector] public Material currentOutlineMaterial = null;
	}

	[Serializable]
	public class MovementData
	{
		public Rigidbody rigidbody = null;
		public bool isControlled = true;
		public bool randomInitialPosition = false;
		public Vector3 initialPosition = Vector3.zero;
		public bool randomInitialRotation = false;
		public Vector3 initialRotation = Vector3.zero;
		public Vector2 initialSpeedRange = Vector2.zero;
		public Vector2 initialSpinForceRange = Vector2.zero;
		public bool randomInitialSpinAxis = false;
		public Vector3 initialSpinAxis = Vector3.zero;
		public float maxSpeed = 3.0f;        //top speed
		public float maxAcceleration = 3.0f;     //increase speed
		public float deceleration = 1.0f;   //braking power
		public float fullStopThreshold = 25.0f; //! @note The minimum speed at which the entity will be forced to a full stop.
		public bool isBankingEnabled = true;
		public float bankRate = 180.0f;        //! @note The speed at which the Entity rolls (degrees/sec).
		public float bankLimit = 75.0f;        //! @note The maximum desired roll left or right (degrees).
		public float rotationSpeedIdle = 180.0f;   //turning speed while stopped (degrees/sec)
		public float rotationSpeedPowered = 90.0f;  //turning speed while moving (degrees/sec)
		public bool allowScreenWrap = true;
        public Transform rootNode = null;
        public bool createRotationNode = false;
        [HideInInspector] public GameObject rotationNode = null;
	}

	[Serializable]
	public class GameData
	{
		public float energyCap = 10.0f;
		public float rechargeRate = 0.5f;
		public bool hasScreenplay = false; 
		public int lives = 0; //! @note Any negative value indicates infinite lives.
		public int health = 10;
        public float timeToLive = 0.0f; //! @note in seconds.
        public float collisionDamage = 0.0f;
        public int pointValue = 0;
        public bool isTargetable = true;
		public EntityManager proxyForEntity = null;
		public PlayerManager proxyForPlayer = null;
	}
	
	[Serializable]
	public class SoundData
	{
		public AudioClip mainThruster = null;
		public AudioClip retroThruster = null;
		public AudioClip weapon = null;
		public AudioClip destroyed = null;
		public AudioClip collided = null;
	}
	
    // --------------------------------------------------------------------------
    // DATA MEMBERS
	
	[HideInInspector] public GameObject prefab = null; //! @hack HACK HACK HACK!!!
	public VisualData visual = new VisualData();
	public MovementData movement = new MovementData();
	public GameData game = new GameData();
	public SoundData sound = new SoundData();

    // --------------------------------------------------------------------------
    // METHODS

	public void Awake()
	{
		/* EntityManager manager = */this.gameObject.AddComponent<EntityManager>();
		
		/*foreach( DebugInfoIndicator indicator in this.visual.debugInfoIndicators )
		{
			indicator.Init( this.gameObject );
		}*/
	}
}
