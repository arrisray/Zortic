using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.Linq;
using TSDC;

//! @url http://answers.unity3d.com/questions/200123/editor-how-to-do-propertyfield-for-list-elements.html
[CustomEditor(typeof(GameRule))]
public class GameRuleInspector : Editor
{
	// --------------------------------------------------------------------------
    // DELEGATES
	
	public delegate bool DrawDelegate( SerializedProperty property );
	
	// --------------------------------------------------------------------------
    // DATA MEMBERS
	
	private GameRule m_gameRule = null;
	private int INDENT_LEVEL = 2;
	private GUIContent m_addItemLabel = new GUIContent( "+", "Add" );
	private GUIContent m_removeItemLabel = new GUIContent( "-", "Remove" );
	private GUILayoutOption m_miniButtonWidth = GUILayout.MaxWidth( 20.0f );
	//! @hack This flag is used to force an editor update on the game rule's first run because
	//  we are manually creating a default condition asset (NullCondition) to assign to its
	//  condition data member when we create the game rule itself. This goes unnoticed by
	//  Unity's serializer and so the default condition will not properly persist unless we
	//  manually force it to on the game rule's first run through, by explicitly calling
	//  EditorUtility.SetDirty().
	private bool m_isFirstRun = true; 
	
	// --------------------------------------------------------------------------
    // METHODS
	
	public void OnEnable()
	{
		this.m_gameRule = (GameRule)target;
	}	
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		this.DrawGameRule();
		
		if( this.serializedObject.ApplyModifiedProperties() )
		{
			AssetDatabase.RenameAsset( AssetDatabase.GetAssetPath( this.m_gameRule ), this.m_gameRule.id );
		}
		
		if( GUI.changed || this.m_isFirstRun )
		{
			this.m_isFirstRun = false;
	        EditorUtility.SetDirty( target );
       	}
		
		EditorGUILayout.Separator();
		if( GUILayout.Button( "Delete Game Rule" ) )
		{
			if( EditorUtility.DisplayDialog( "Delete Game Rule", "You sho 'bout dat...?", "Yep", "Nope" ) )
			{
				GameRuleEditor.DestroyGameRule( this.m_gameRule );
			}
		}
	}
	
	public void DrawGameRule()
	{
		SerializedProperty 
			id = this.serializedObject.FindProperty( "id" ),
			enabled = this.serializedObject.FindProperty( "enabled" ),
			isOneShot = this.serializedObject.FindProperty( "isOneShot" ),
			gameEvents = this.serializedObject.FindProperty( "events" );
		
		EditorGUIUtility.LookLikeControls();

		EditorGUILayout.PropertyField( id );
		EditorGUILayout.PropertyField( enabled );
		EditorGUILayout.PropertyField( isOneShot );
		
		EditorGUILayout.Separator();
		
		this.DrawCondition( ref this.m_gameRule.condition );
		
		EditorGUILayout.Separator();
		
		this.DrawEventList( gameEvents );
	}
	
	public bool DrawEventList( SerializedProperty listProperty )
	{
		// Local vars
		string[] typeNames = GameRuleEditor.eventTypes.ToArray();
		List<GameRuleEvent> nodes = this.m_gameRule.events;
		
		// Draw section header
		EditorGUILayout.BeginHorizontal();
		{
			// Draw node foldout
			listProperty.isExpanded = EditorGUILayout.Foldout( listProperty.isExpanded, Utils.UppercaseFirst(listProperty.name) );
			
			// Draw add node button
			if( GUILayout.Button( this.m_addItemLabel, EditorStyles.miniButton, this.m_miniButtonWidth ) )
			{
				// Get requested type info
				string newType = typeNames[ 0 ];
				Assembly assembly = typeof( GameManager ).Assembly;
				System.Type t = Type.GetType( newType + ", " + assembly.FullName ); //! @hack Assumes knowledge of assembly-qualified name signature...
			
				// Create instance of requested type
				GameRuleEvent instance = Utils.InvokeGenericMethod( typeof( GameRuleEditor ), "CreateGameRuleNodeAsset", t, null ) as GameRuleEvent;
				nodes.Insert( 0, instance );
			}
		}
		EditorGUILayout.EndHorizontal();
		
		// Draw nodes
		if( listProperty.isExpanded && (nodes.Count > 0) )
        {
			EditorGUI.indentLevel += INDENT_LEVEL;
			
			// Draw nodes
			for( int i = nodes.Count-1; i >= 0; --i )
			{
				GameRuleEvent node = nodes[i];
				if( !this.DrawEvent( ref node ) )
				{
					// Destroy existing node asset
					nodes.RemoveAt ( i );
					GameRuleEditor.DestroyGameRuleAsset( node );
				}
				else
				{
					nodes[i] = node;
				}
			}
			
			EditorGUI.indentLevel -= INDENT_LEVEL;
		}
		return true;
	}
	
	public bool DrawEvent( ref GameRuleEvent gameEvent )
	{
		bool isDestroyed = false;
		
		SerializedObject eventObject = new SerializedObject( gameEvent );
		SerializedProperty eventProperty = eventObject.GetIterator();
		
		// Draw event foldout
		EditorGUILayout.BeginHorizontal();
		{
			eventProperty.isExpanded = EditorGUILayout.Foldout( eventProperty.isExpanded, gameEvent.id );
			isDestroyed = GUILayout.Button( this.m_removeItemLabel, EditorStyles.miniButton, this.m_miniButtonWidth );
		}
		EditorGUILayout.EndHorizontal();
		
		if( isDestroyed )
		{
			return false;
		}
		
		if( eventProperty.isExpanded )
		{			
			EditorGUI.indentLevel += INDENT_LEVEL;
			{
				// Draw derived node type
				System.Object[] args = new System.Object[ 1 ] { gameEvent };
				
				MethodInfo onInspectorGUIMethod = typeof( GameRuleNodeInspector ).GetMethod ( "OnInspectorGUI", BindingFlags.Public | BindingFlags.Static );
				MethodInfo genericOnInspectorGUIMethod = onInspectorGUIMethod.MakeGenericMethod( gameEvent.GetType () );
				genericOnInspectorGUIMethod.Invoke( null, args );
				
				gameEvent = args[0] as GameRuleEvent;
			}
			EditorGUI.indentLevel -= INDENT_LEVEL;
		}
		
		return true;
	}
	
	public bool DrawCondition( ref GameRuleCondition condition )
	{
		SerializedObject conditionObject = new SerializedObject( condition );
		SerializedProperty conditionProperty = conditionObject.GetIterator();
		
		// Draw foldout
		conditionProperty.isExpanded = EditorGUILayout.Foldout( conditionProperty.isExpanded, "Condition" );
		
		// Draw derived node type
		if( conditionProperty.isExpanded )
		{
			EditorGUI.indentLevel += INDENT_LEVEL;
			{
				System.Object[] args = new System.Object[ 1 ] { condition };
				
				MethodInfo onInspectorGUIMethod = typeof( GameRuleNodeInspector ).GetMethod ( "OnInspectorGUI", BindingFlags.Public | BindingFlags.Static );
				MethodInfo genericOnInspectorGUIMethod = onInspectorGUIMethod.MakeGenericMethod( condition.GetType () );
				genericOnInspectorGUIMethod.Invoke( null, args );
				
				condition = args[0] as GameRuleCondition;
			}
			EditorGUI.indentLevel -= INDENT_LEVEL;
		}
		return true;
	}
	
	public void PrintSerializedObjectHierarchy( SerializedObject serializedObject )
	{
		SerializedProperty property = serializedObject.GetIterator();
		
		string msg = "===== " + serializedObject.ToString() + "\n";
		while( property.Next( true ) )
		{
			msg += "-";
			for( int i = 0; i < property.depth; ++ i )
			{
				msg += "-";
			}
			msg += "> " + property.name + "\n";
		}
		
		/// Debug.Log ( msg );
	}
	
	public void PrintSerializedPropertyHierarchy( SerializedProperty property )
	{
		SerializedProperty iterator = this.serializedObject.FindProperty( property.propertyPath );
		
		string msg = "===== " + property.name + "\n";
		while( iterator.Next( true ) )
		{
			msg += "-";
			for( int i = 0; i < iterator.depth; ++ i )
			{
				msg += "-";
			}
			msg += "> " + iterator.name;
			if( iterator.propertyType == SerializedPropertyType.ObjectReference )
			{
				msg += " " + iterator.objectReferenceValue;
			}
			msg += "\n";
		}
		
		/// Debug.Log ( msg );
	}
}
	