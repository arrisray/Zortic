using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
		
public static class GameRuleNodeInspector
{
	public static void OnInspectorGUI<T>( ref GameRuleNode node ) where T : GameRuleNode
	{
		T derivedNode = node as T;
		List<string> nodeTypes = GameRuleEditor.GetNodeTypes( derivedNode );
		int selectedIndex = nodeTypes.IndexOf( derivedNode.GetType().ToString() );
		
		// Draw event type dropdown
		int index = EditorGUILayout.Popup( "Type", selectedIndex, nodeTypes.ToArray() );
		if( index != selectedIndex )
		{
			// Get requested type info
			string newType = nodeTypes[ index ];
			Assembly assembly = typeof( GameManager ).Assembly;
			System.Type t = Type.GetType( newType + ", " + assembly.FullName ); //! @hack Assumes knowledge of .NET's assembly-qualified name signature...
		
			// Create instance of requested type
			GameRuleNode instance = Utils.InvokeGenericMethod( typeof( GameRuleEditor ), "CreateGameRuleNodeAsset", t, null ) as GameRuleNode;
			
			// Destroy existing node asset
			GameRuleEditor.DestroyGameRuleAsset( derivedNode );
			
			// Store new node
			node = instance as GameRuleNode;
			return;
		}
		
		// Draw base node properties
		SerializedObject nodeObject = new SerializedObject( derivedNode );
		nodeObject.Update();
		bool visitChildren = true;
		SerializedProperty property = nodeObject.GetIterator();
		while( property.NextVisible( visitChildren ) )
		{
			if( property.name == "m_Script" ) //! @hack ...
			{
				continue;
			}
				
			EditorGUILayout.PropertyField( property, property.isExpanded );
			visitChildren = ( property.type != "vector" );
		}
		
		// Draw horizontal line break
		EditorGUILayout.Separator();
			
		// Draw input ports
		if( node is GameRuleEvent )
		{
			GameRuleNodeInspector.DrawPorts( node.m_inputs, "Inputs", "Inputs" );
		}
			
		// Draw output ports
		if( node is GameRuleCondition )
		{
			GameRuleNodeInspector.DrawPorts( node.m_outputs, "Outputs", "Outputs" );
		}
		
		if( nodeObject.ApplyModifiedProperties() )
		{
			AssetDatabase.RenameAsset( AssetDatabase.GetAssetPath( node ), node.id );
		}
		
		if( GUI.changed )
		{
	        EditorUtility.SetDirty( node );
       	}
			
		EditorGUIUtility.LookLikeControls();
	}
	
	private static void DrawPorts( List<Port> ports, string portsPropertyName, string label )
	{		
		EditorGUILayout.LabelField( label );		

		foreach( Port port in ports )	
		{
            GameRuleNodeInspector.DrawPort( port );
        }
	}

	private static void DrawPort( Port port )
	{
		const int INDENT_SIZE = 1; //! @hack Magic number!
		EditorGUI.indentLevel += INDENT_SIZE;

		EditorGUILayout.BeginHorizontal();
		{
			if( port.id == string.Empty )
			{
				port.id = "New Port";
			} 
			
			string label = port.id + " (" + port.type.ToString() + ")";
			port.linkFromId = EditorGUILayout.TextField( label, port.linkFromId );
		}
		EditorGUILayout.EndHorizontal();

		EditorGUI.indentLevel -= INDENT_SIZE;
	}
}
