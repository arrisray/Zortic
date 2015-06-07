using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent( typeof(GameData) )]
[RequireComponent( typeof(MusicManager) )]
[RequireComponent( typeof(PlayerManager) )]
public class GameManager : MonoBehaviourSingleton<GameManager>
{
	// --------------------------------------------------------------------------
    // ENUMS
	
	public enum Levels
	{
		Menu = 0,
		GameAsteroids,
		GameMinerPods,
		GamePirateFighter,
		GamePirateBase,
		Credits,
		Count
	}
	
    // --------------------------------------------------------------------------
    // DATA MEMBERS
	
	public Camera sceneCamera = null;
	public Camera guiCamera = null;
	public GameObject runtimeNode = null;
	public GameObject runtimeIndicatorsNode = null;
	public GameObject runtimeGuiNode = null;
    public PlayerManager playerManager = null;
	public MessageManager messageManager = MessageManager.Instance;
	public InputManager inputManager = InputManager.Instance;
    public EnvironmentManager environmentManager = null;
	public CinematicManager cinematicManager = null;
	public DebugInfoIndicator.Data debugInfoIndicatorData = new DebugInfoIndicator.Data();
    private bool m_isGameOver = false;
    private GameData m_data = null;
    private Dictionary< EntityTag, List<GameObject> > m_entityTagMap = new Dictionary< EntityTag, List<GameObject> >();
    [SerializeField] private List<GameRule> m_gameRules = new List<GameRule>();
	private Dictionary<GameRule, bool> m_defaultGameRuleStates = new Dictionary<GameRule, bool>();
    private Dictionary<int, UnityEngine.Object> m_instances = new Dictionary<int, UnityEngine.Object>();
	private List< EntityManager > m_entities = new List<EntityManager>();
	private Dictionary< GameObject, List<GameObject> > m_instanceMap = new Dictionary< GameObject, List<GameObject> >();
	private Dictionary< GameObject, GameObject > m_reverseInstanceMap = new Dictionary< GameObject, GameObject >();
	private bool m_pause = false;
	private AudioManager m_audioManager = null;
	private MusicManager m_musicManager = null;
	private Levels m_currentLevel = Levels.Menu;

    // --------------------------------------------------------------------------
    // PROPERTIES

    public GameData data 
    { 
        get { return this.m_data; }
    }

    public bool isGameOver
    { 
        get { return this.m_isGameOver; }
        set { this.m_isGameOver = value; }
    }
	
	public Levels currentLevel
	{
		get { return this.m_currentLevel; }
		set 
		{ 
			this.m_currentLevel = value; 
			this.LoadLevel( this.m_currentLevel );
		}
	}
	
	public MusicManager musicManager
	{
		get { return this.m_musicManager; }
	}

	public List<UnityEngine.Object> instances
	{
		get { return this.m_instances.Select(kvp => kvp.Value).ToList(); }
	}
	
	public List<GameRule> gameRules
	{
		get { return this.m_gameRules; }
	}
	
	public AudioManager audioManager
	{
		get { return this.m_audioManager; }
	}
	
	public bool pause
	{
		get { return this.m_pause; }
		set 
		{ 
			Time.timeScale = ( value == true ) ? 0.0f : 1.0f;
			this.ShowRuntimeGuis( !value );
			this.ShowRuntimeIndicators( !value );
			this.m_pause = value; 
			this.m_audioManager.Pause ();
		}
	}
	
	public EntityManager currentPlayer
    { 
        get { return this.playerManager.currentEntity; }
        set
        {
            if( !value )
            {
                /// Debug.LogError( "Attempted to assign a null entity!" );
                return;
            }
			
			// Re-assign Player EntityTag
			EntityTag playerTag = GameManager.Instance.FindEntityTagByName( PlayerManager.PlayerTagName );
			EntityManager oldPlayer = this.playerManager.currentEntity;
			if( oldPlayer )
			{
				oldPlayer.entityTags.Remove( playerTag );
			}
			if( value )
			{
				value.entityTags.Add( playerTag );
			}

            // /// Debug.Log( "AssignPlayerControl: entity=" + this.currentEntity.name + ", player=" + this.name );
            this.playerManager.currentEntity = value;
            MessageManager.Instance.RaiseEvent( new AssignPlayerControlMessage( this.playerManager.currentEntity.gameObject, oldPlayer.gameObject ) );
			
			this.m_entityTagMap[ playerTag ].Clear();
			this.m_entityTagMap[ playerTag ].Add( this.playerManager.currentEntity.data.prefab );
        }
    }

    // --------------------------------------------------------------------------
    // METHODS

    public override void OnEnable()
    {
		this.runtimeNode = new GameObject( "_runtime" );
		this.runtimeNode.transform.position = Vector3.zero;
		this.runtimeNode.transform.rotation = Quaternion.identity;
		this.runtimeNode.transform.localScale = Vector3.one;
		this.runtimeNode.transform.parent = this.transform;
		
		this.runtimeIndicatorsNode = new GameObject( "indicators" );
		this.runtimeIndicatorsNode.transform.position = Vector3.zero;
		this.runtimeIndicatorsNode.transform.rotation = Quaternion.identity;
		this.runtimeIndicatorsNode.transform.localScale = Vector3.one;
		this.runtimeIndicatorsNode.transform.parent = this.runtimeNode.transform;
		
        this.m_data = this.GetComponent<GameData>();
		this.LoadResources();
        this.SetupManagers();
    }

    public override void OnDisable()
    {
        this.TeardownManagers();
        this.UnloadResources();
        base.OnDisable();
    }
	
	public void Start()
	{
		foreach( GameRule rule in this.m_gameRules )
		{
			rule.Start();
		}
	}

	public override void Reset()
	{
        base.Reset();
	}
	
	public void ShowRuntimeGuis( bool isVisible )
	{
		this.runtimeGuiNode.SetActiveRecursively( isVisible );
	}
	
	public void ShowRuntimeIndicators( bool isVisible )
	{
		this.runtimeIndicatorsNode.SetActiveRecursively( isVisible );
	}
	
	public void FixedUpdate()
	{
		if( !this.m_pause )
		{
			// Update entities
			foreach( EntityManager entity in this.m_entities )
			{
				entity.ManagedFixedUpdate();
			}
		}
	}

    public void Update()
    {
        List<GameRule> untriggeredGameRules = new List<GameRule>( this.m_gameRules );
        List<GameRule> triggeredGameRules = new List<GameRule>();
  
        bool areEligibleGameRulesTriggered = false;
        while( !areEligibleGameRulesTriggered )
        {      
            areEligibleGameRulesTriggered = true;

            foreach( GameRule gameRule in untriggeredGameRules )
            {
                if( gameRule.Update() )
                {
                    triggeredGameRules.Add( gameRule );
                    areEligibleGameRulesTriggered = false;
                }
            }

            foreach( GameRule gameRule in triggeredGameRules )
            {
                untriggeredGameRules.Remove( gameRule );
            }
        }

		// Update entities
		//! @note Reverse iterating because entities may destroy themselves during an update,
		//  thus modifying the collection which does not agree with IEnumerators during a foreach( ... ).
		if( !this.m_pause )
		{
			for( int i = (this.m_entities.Count - 1); i >= 0; --i )
			{
				EntityManager entity = this.m_entities[i];
				entity.ManagedUpdate();
			}
		}
		
		// Update audio manager
		this.m_audioManager.ManagedUpdate();
    }

	public void LateUpdate()
	{
		if( !this.m_pause )
		{
			// Update entities
			for( int i = (this.m_entities.Count - 1); i >= 0; --i )
			{
				EntityManager entity = this.m_entities[i];
				entity.ManagedLateUpdate();
			}
		}
	}

	public void OnDrawGizmos()
	{
		// Update entities
		/*
		for( int i = (this.m_entities.Count - 1); i >= 0; --i )
		{
			EntityManager entity = this.m_entities[i];
			// entity.ManagedOnDrawGizmos();
		}
        //*/
	}

    public void OnGUI()
    {
        // Game over
        if (this.m_isGameOver)
        {
            if(Time.timeScale != 0.0)
            {
                if (GUI.Button(new Rect(0, 0, Screen.width, Screen.height),"", this.data.visual.gameOverStyle))
                    Application.LoadLevel("MainMenu");

                if (Time.time % 2 < 1)
                    GUI.Label(new Rect(0, 0, Screen.width, Screen.height),this.data.visual.gameOverMessage, this.data.visual.gameOverStyle);
            }
        }
		/*
		// Update entities
		for( int i = (this.m_entities.Count - 1); i >= 0; --i )
		{
			EntityManager entity = this.m_entities[i];
			// entity.ManagedOnGUI();
		}
        //*/
    }

	public EntityTag FindEntityTagByName( string name )
	{
		List<EntityTag> tags = this.m_entityTagMap.Keys.ToList();
		foreach( EntityTag tag in tags )
		{
			if( tag.name == name )
			{
				return tag;
			}
		}
		return null;
	}
	
	public UnityEngine.Object FindObjectByInstanceId( int id )
	{
		if( this.m_instances.ContainsKey(id) )
		{
			return this.m_instances[id];
		}
		return null;
	}

	public GameObject FindPrefabByTag( UnityEngine.Object tag )
	{
		if( tag == null )
		{
			/// Debug.LogError( "Ignoring attempt to search for a prefab with an empty tag...!" );
			return null;
		}
		
		GameObject prefab = null;
		if( tag as EntityTag )
		{
			if( !this.m_entityTagMap.ContainsKey( tag as EntityTag ) )
			{
				/// Debug.LogWarning( "No prefabs registered for EntityTag '" + tag.name + "'!" );
				return null;
			}
			
			try
			{
				List<GameObject> prefabs = this.m_entityTagMap[ tag as EntityTag ];
				int index = (int)( UnityEngine.Random.value * (float)prefabs.Count );
				prefab = prefabs[index];
			}
			catch( System.Exception e )
			{
				/// Debug.LogWarning( e.Message );
				/// Debug.LogWarning( "Tag: " + tag.name );
			}
		}
		else if( tag as GameObject )
		{
			// /// Debug.Log ( "FindPrefabByTag GameObject: " + tag + ", " + (tag as GameObject) );
			prefab = ( tag as GameObject );
		}
		else
		{
			/// Debug.LogError( "Tag (type=" + tag.GetType() + ") must be of type EntityTag or GameObject!" );
			prefab = null;
		}

		return prefab;
	}
	
	public GameObject[] FindInstancesByTag( UnityEngine.Object tag )
	{
		if( tag == null )
		{
			/// Debug.LogError( "Ignoring attempt to search for a prefab with an empty tag...!" );
			return null;
		}
		
		GameObject prefab = this.FindPrefabByTag( tag );
		if( prefab == null )
		{
			/// Debug.LogWarning ( "Unable to find object instance by tag: " + tag.name );
			return null;
		}
		
		List<GameObject> instances = null;
		if( this.m_instanceMap.TryGetValue( prefab, out instances ) )
		{
			if( instances.Count == 0 )
			{
				// /// Debug.LogWarning( "No GameObject instances exist for tag: " + tag.name );
				return null;
			}
			return instances.ToArray();
		}
		
		// /// Debug.LogWarning( "No GameObject instances exist for tag: " + tag.name );
		return null;
	}
	
	public GameObject FindInstanceByTag( UnityEngine.Object tag )
	{
		GameObject[] instances = this.FindInstancesByTag( tag );
		if( instances == null || instances.Length == 0 )
		{
			return null;
		}
			
		int index = (int)( Mathf.Round( UnityEngine.Random.value * (float)(instances.Length - 1) ) );
		return instances[ index ];
	}

	public bool RegisterEntity( EntityManager entity )
	{
		if( !this.m_entities.Contains( entity ) )
		{
			this.m_entities.Add( entity );
			return true;
		}
		return false;
	}

	public bool UnregisterEntity( EntityManager entity )
	{
		if( !this.m_instances.Remove( entity.GetInstanceID() ) )
        {
            return false;
        }
		
		entity.Teardown();
        
		// Remove prefab <-> instance mapping info
		//! @hack OMG this is so haxxxed...
		GameObject goInstance = entity.gameObject;
		GameObject original = null;
		if( (goInstance != null) && this.m_reverseInstanceMap.TryGetValue( goInstance, out original ) )
		{
			List<GameObject> instances = null;
			if( this.m_instanceMap.TryGetValue( original, out instances ) )
			{
				instances.Remove ( goInstance );
				this.m_reverseInstanceMap.Remove ( goInstance );
				if( instances.Count == 0 )
				{
					this.m_instanceMap.Remove( original );
				}
			}
		}
		
		return this.m_entities.Remove( entity );
	}

    public UnityEngine.Object UnityInstantiate( UnityEngine.Object original, Vector3 position, Quaternion rotation )
    {
        UnityEngine.Object instance = Instantiate( original, position, rotation );
		// /// Debug.Log( "Adding instance ID: " + instance.GetInstanceID() + ", name=" + instance.name );
        this.m_instances.Add( instance.GetInstanceID(), instance );
		
		//! @hack HACK HACK HACK!!!
		GameObject go = instance as GameObject;
		if( go )
		{
			EntityData entity = go.GetComponent<EntityData>();
			if( entity )
			{
				entity.prefab = original as GameObject;
			}
		}
		
		// Save prefab <-> instance mapping info
		//! @hack OMG this is so haxxxed...
		GameObject originalGameObject = original as GameObject;
		List<GameObject> instances = null;
		if( (originalGameObject != null) && !this.m_instanceMap.TryGetValue( originalGameObject, out instances ) )
		{
			instances = new List<GameObject>();
			this.m_instanceMap.Add ( originalGameObject, instances );
		}
		instances.Add ( instance as GameObject );
		this.m_reverseInstanceMap.Add ( instance as GameObject, originalGameObject );

        return instance;
    }

    public bool UnityDestroy( UnityEngine.Object instance )
    {
		if( instance as GameObject )
		{
			GameObject prefab;
			if( this.m_reverseInstanceMap.TryGetValue( instance as GameObject, out prefab ) )
			{
				List<GameObject> instances;
				if( this.m_instanceMap.TryGetValue( prefab, out instances ) )
				{
					instances.Remove( instance as GameObject );
					this.m_instanceMap[prefab] = instances;
				}
			}
		}
		
		Destroy( instance );
		GC.Collect();
        return true;
    }

    public bool RegisterGameRule( GameRule gameRule )
    {
        if( this.m_gameRules.Contains(gameRule) )
        {
            return false;
        }
		
		// /// Debug.Log( "Reg'd " + gameRule.name + ", count=" + this.m_gameRules.Count );
        this.m_gameRules.Add( gameRule );
        return true;
    }

    public bool UnregisterGameRule( GameRule gameRule )
    {
        if( !this.m_gameRules.Contains(gameRule) )
        {
            return false;
        }
		
		// /// Debug.Log( "Unreg'd " + gameRule.name );
        this.m_gameRules.Remove( gameRule );
        return true;
    }
	
	public void ResetGameRules()
	{
		foreach( GameRule gameRule in this.m_gameRules )
		{
			gameRule.Reset ();
		}
	}

    public bool RegisterPrefab( EntityTag tag, GameObject prefab )
    {
		if( !tag )
		{
			/// Debug.LogError( "Attempted to register a NULL EntityTag." + "  " + prefab.name );
			return false;
		}

		// Recursively register prefab with parent tags, if any
		foreach( EntityTag parent in tag.parents )
		{
			this.RegisterPrefab( parent, prefab );
		}

        if( !this.m_entityTagMap.ContainsKey(tag) )
        {
            this.m_entityTagMap.Add( tag, new List<GameObject>() );
			// /// Debug.Log ( "Creating prefab list container for tag: " + tag.name );
        }

        List<GameObject> prefabs = this.m_entityTagMap[tag];
        if( prefabs.Contains(prefab) )
        {
            return false;
        }
		
        // /// Debug.Log( "Registered prefab '" + prefab.name + "' with tag '" + tag + "' (instance=" + tag.GetInstanceID() + ") ." );
        prefabs.Add( prefab );
        return true;
    }

    public bool UnregisterPrefab( EntityTag key, GameObject prefab )
    {
		// Recursively unregister prefab with parent tags, if any
		if( key is EntityTag )
		{
			EntityTag tag = key as EntityTag;
			foreach( EntityTag parent in tag.parents )
			{
				this.UnregisterPrefab( parent, prefab );
			}
		}

        if( !this.m_entityTagMap.ContainsKey(key) )
        {
            return false;
        }

        List<GameObject> prefabs = this.m_entityTagMap[key];
        if( !prefabs.Contains( prefab ) )
        {
            return false;
        }

        // /// Debug.Log( "Unregistered '" + prefab.name + "' from MonoTag '" + entityTag.tag + "'." );
        prefabs.Remove( prefab );
        return true;
    }

    private void SetupManagers()
    {
		if( this.m_audioManager == null )
		{
			this.m_audioManager = this.gameObject.AddComponent<AudioManager>();
		}
		this.m_audioManager.Setup();
		
		this.m_musicManager = this.gameObject.GetComponent<MusicManager>();
		if( this.m_musicManager != null )
		{
			this.m_musicManager.Setup ();
		}
		
		this.cinematicManager = CinematicManager.Instance;
		
        this.inputManager.Setup();
		
		// Player Manager
		if( this.playerManager == null )
		{
			this.playerManager = this.gameObject.GetComponent<PlayerManager>();
		}
        PlayerData playerData = this.gameObject.GetComponent<PlayerData>();
        PlayerIndicators playerIndicators = this.gameObject.AddComponent<PlayerIndicators>();
        this.playerManager.Setup( playerData, playerIndicators );
		this.currentPlayer = this.playerManager.currentEntity;
    }

    private void TeardownManagers()
    {
        // this.inputManager.Teardown();
        // this.playerManager.Teardown();
    }

    private void LoadResources()
    {
		//! @hack ???
		this.m_gameRules.Clear ();
		
        // Load game rules
		//! @note We only load the rules associated with the currently loaded level.
        //! @note Each GameRule will register itself with the GameManager when loaded.
		string levelRulesPath = "Rules/" + Application.loadedLevelName;
        UnityEngine.Object[] resources = Resources.LoadAll( levelRulesPath );
		// /// Debug.Log( "Num Game Rules: " + resources.Length );
		foreach( UnityEngine.Object resource in resources )
        {
			GameRule rule = (GameRule)resource;
			rule.Load();
			this.m_defaultGameRuleStates.Add ( rule, rule.enabled );
		}

		// Setup tag map
		resources = Resources.LoadAll( "Tags" );
		foreach( UnityEngine.Object resource in resources )
		{
			EntityTag tag = resource as EntityTag;
            if( !tag )
            {
                continue;
            }
			
			List<GameObject> instances = null;
			if( this.m_entityTagMap.TryGetValue( tag, out instances ) )
			{
				/// Debug.LogError ( "Duplicate tag detected: " + tag.name );
			}
			else
			{
				this.m_entityTagMap.Add( tag, new List<GameObject>() );
			}
		}

        // Initialize tag mappings
        resources = Resources.LoadAll( "Prefabs" );
        // /// Debug.Log( "Num Prefabs: " + resources.Length );
        foreach( UnityEngine.Object resource in resources )
        {
            GameObject prefab = resource as GameObject;
            if( !prefab )
            {
                continue;
            }

			// /// Debug.Log( "Load: " + prefab.name );

            EntityTags entityTags = prefab.GetComponent<EntityTags>();
            if( !entityTags )
            {
                continue;
            }

            foreach( EntityTag entityTag in entityTags.entityTags )
            {
				// /// Debug.Log( "EntityTag: " + entityTag.name );
                this.RegisterPrefab( entityTag, prefab );
            }

			// this.RegisterPrefab( prefab, prefab );
        }
    }

    private void UnloadResources()
    {
		while( this.m_gameRules.Count > 0 ) 
		{
            GameRule rule = this.m_gameRules[0];
            this.m_gameRules.RemoveAt(0);
			rule.Unload();
			rule.enabled = this.m_defaultGameRuleStates[rule];
		}
		this.m_gameRules.Clear();
		this.m_defaultGameRuleStates.Clear();

        foreach( UnityEngine.Object instance in this.m_instances.Values )
        {
            // /// Debug.Log( "Unload; " + instance.name );
            GameObject gameObject = instance as GameObject;
            if( !gameObject )
            {
                continue;
            }

            EntityManager entity = gameObject.GetComponent<EntityManager>();
            if( !entity )
            {
                continue;
            }

            entity.Teardown();
        }
		this.m_instances.Clear();
    }
	
	private void LoadLevel( Levels level )
	{
		GameManager.Instance.ResetGameRules();
		
		switch( level )
		{
			case Levels.Menu:
			{
				Application.LoadLevel( "Menu" );
				break;
			}
			case Levels.GameAsteroids:
			{
				Application.LoadLevel( "Game" );
				break;
			}
			case Levels.Credits:
			{
				// Application.LoadLevel( "Credits" );
				break;
			}
			default:
			{
				break;
			}
		} // end switch
		
		// Update level music
		//! @note This doesn't work because this GameManager will be destroyed right
		// after the call to load a new level. Nothing after the switch will execute(?)!
		// this.m_musicManager.Play( this.m_currentLevel );
	}
}
