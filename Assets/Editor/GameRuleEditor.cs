using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using TSDC;

//! @url CONSIDER THIS!!! http://answers.unity3d.com/questions/51615/do-custom-inspectors-support-inheritance.html

//! @url http://answers.unity3d.com/questions/39313/how-do-i-get-a-callback-every-frame-in-edit-mode.html
[InitializeOnLoad]
public class GameRuleEditor
{
	public static string GameRulePath = "Assets/Resources/Rules";
	public static string GameRuleConditionPath = "Assets/Resources/Conditions";
	public static string GameRuleEventPath = "Assets/Resources/Events";
	public static List<string> conditionTypes = new List<string>();
	public static List<string> eventTypes = new List<string>();
	
	static GameRuleEditor()
	{
		EditorApplication.update += Update;
		
		IEnumerable<Type> conditions = Utils.GetDerivedTypesFor<GameRuleCondition>();
		foreach( Type t in conditions ) { conditionTypes.Add( t.FullName ); }
		
		IEnumerable<Type> events = Utils.GetDerivedTypesFor<GameRuleEvent>();
		foreach( Type t in events ) { eventTypes.Add( t.FullName ); }
	}
	
	public GameRuleEditor()
	{	
	}
	
	public static GameRule CreateRule()
	{
		GameRule asset = CustomAssetUtility.CreateAsset<GameRule>( GameRuleEditor.GameRulePath );
		asset.Load ();
		return asset;
	}
	
	public static T CreateCondition<T>() where T : GameRuleCondition
	{
		T condition = GameRuleEditor.CreateGameRuleNodeAsset<T>();
		condition.Load ();
		return condition;
	}
	
	public static T CreateEvent<T>() where T : GameRuleEvent
	{
		T e = GameRuleEditor.CreateGameRuleNodeAsset<T>();
		e.Load();
		return e;
	}
	
	public static T CreateGameRuleNodeAsset<T>() where T : GameRuleNode
	{
		string assetPath = string.Empty;
		if( typeof(GameRuleCondition).IsAssignableFrom( typeof(T) ) )
		{
			assetPath = GameRuleEditor.GameRuleConditionPath;
		}
		else if( typeof(GameRuleEvent).IsAssignableFrom( typeof(T) ) )
		{
			assetPath = GameRuleEditor.GameRuleEventPath;
		}
		
		T asset = CustomAssetUtility.CreateAsset<T>( assetPath );
		asset.id = asset.name;
		asset.Load ();
		return asset;
	}
	
	public static void DestroyGameRule( GameRule rule )
	{
		string nodePath = AssetDatabase.GetAssetPath( rule.condition );
		AssetDatabase.DeleteAsset( nodePath );
		
		foreach( GameRuleEvent ruleEvent in rule.events )
		{
			nodePath = AssetDatabase.GetAssetPath( ruleEvent );
			AssetDatabase.DeleteAsset( nodePath );
		}
		
		nodePath = AssetDatabase.GetAssetPath( rule );
		AssetDatabase.DeleteAsset( nodePath );
	}
	
	public static void DestroyGameRuleAsset( ScriptableObject asset )
	{
		string assetPath = AssetDatabase.GetAssetPath( asset );
		AssetDatabase.DeleteAsset( assetPath );
	}
	
	public static List<string> GetNodeTypes( GameRuleNode node )
	{
		if( node is GameRuleCondition )
		{
			return GameRuleEditor.conditionTypes;
		}
		else if( node is GameRuleEvent )
		{
			return GameRuleEditor.eventTypes;
		}
		return null;
	}
	
	static List<string> GetDerivedTypes< TYPE >()
	{
		List<string> results = new List<string>();
		System.Type type = typeof( TYPE );
		if( type != null )
		{
			List<Type> types = type.GetAllDerivedClasses();
			foreach( Type t in types )
			{
				results.Add( t.ToString() );
			}
		}
		return results;
	}
	
	static void Update()
	{
		if( EditorApplication.isCompiling )
		{
			// /// Debug.Log ( Time.frameCount + ": COMPILING" );
		}
	}
}


