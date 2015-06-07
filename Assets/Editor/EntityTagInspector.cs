using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor( typeof(EntityTag) )]
public class EntityTagInspector : Editor
{
	// private EntityTag m_target = null;
    // private SerializedObject m_entityTag = null;
	// private int m_selectedIndex = 0;
	// private static string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
	/*
	void OnEnable() 
	{
		this.m_target = (EntityTag)target;	
		this.m_entityTag = new SerializedObject( target ); 
        // this.m_tag = this.m_entityTag.FindProperty( "tag" );

        for( int i = 0; i < tags.Length; ++i )
        {
        	string s = tags[i];
        	if (s == this.m_target.tag)
        	{
		        this.m_selectedIndex = i; 
		        break;
        	}
        }
	}
	//*/
	public override void OnInspectorGUI() 
	{
		// Set our controls like normal
       	EditorGUIUtility.LookLikeControls();
       	this.DrawDefaultInspector ();
		/*
		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.LabelField("Tag");

			//! @ref http://answers.unity3d.com/questions/57952/getting-the-tag-array.html?sort=oldest
			this.m_selectedIndex = EditorGUILayout.Popup(
				this.m_selectedIndex,
				tags);
            this.m_target.tag = tags[this.m_selectedIndex];
            // /// Debug.Log( "Set MonoTag.tag=" + this.m_target.tag);
           	EditorUtility.SetDirty( target ); 
		}
		EditorGUILayout.EndHorizontal();

        if( this.m_entityTag.ApplyModifiedProperties()
            || ( (Event.current.type == EventType.ValidateCommand) && (Event.current.commandName == "UndoRedoPerformed") ) )
        {
            if( PrefabUtility.GetPrefabType(target) != PrefabType.Prefab )
            {
                // this.m_target.tag = tags[this.m_selectedIndex];
                // /// Debug.Log( "Set MonoTag.tag=" + this.m_target.tag);
            }
        }  

		// If we've changed the gui, set it to dirty
		if (GUI.changed)
		{
			EditorUtility.SetDirty (target);
		}
        //*/
	}
}
