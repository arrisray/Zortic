using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EntityManager))]
public class EntityManagerInspector : Editor
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

    // private static float SnapThresholdScalar = 0.1f;
    // private static Vector3 SnapThreshold = Vector3.one * SnapThresholdScalar;
    // private static float HandleSize = 1.0f;
    // private GUIStyle m_labelStyle = null;
    // private String m_lastSelectedHandle = String.Empty;
    // private int m_currentAttachmentIndex = -1;
    // private Dictionary<string, int> m_controlIdMap = null; //! @todo Change this to be object hash -> control ID to support duplicate names...?
    // private bool isMouseDown = false;

    /*
    private EntityManager m_entityManager = null;
    private SerializedObject
        m_entity = null,
        m_params = null;
    private SerializedProperty m_visualParams = null;
    */

    // --------------------------------------------------------------------------
    // PROPERTIES

    // --------------------------------------------------------------------------
    // METHODS

    public void OnEnable()
    {
        // this.m_controlIdMap = new Dictionary<string, int>();

        /*
        this.m_entityManager = (EntityManager)target;

        this.m_entity = new SerializedObject( target ); 
        this.m_params = new SerializedObject( this.m_entityManager.m_params ); 

        this.m_visualParams = this.m_params.FindProperty( "visual" );
        */
    }

	public override void OnInspectorGUI() 
	{
        /*
        this.m_params.Update();	

        EditorGUILayout.PropertyField( this.m_visualParams, true );

        if( this.m_params.ApplyModifiedProperties()
            || ( (Event.current.type == EventType.ValidateCommand) && (Event.current.commandName == "UndoRedoPerformed") ) )
        {
            if( PrefabUtility.GetPrefabType(target) != PrefabType.Prefab )
            {
                this.m_entityManager.Teardown();
                this.m_entityManager.Setup();
            }
        }  
        */

        /*
        if (GUI.changed)
        {
            EditorUtility.SetDirty (this.m_entityManager);
        }
        */
	}

    public void OnSceneGUI()
    {
        // this.DrawAttachmentGUI();
    }

    public void DrawAttachmentGUI()
    {
        /*
        // Take a snapshot of our target before making changes 
        Undo.SetSnapshotTarget(this.m_entityManager, "Transform Entity Attachment");

        // For each 
        List<EntityAttachmentParams> attachments = this.m_entityManager.attachmentParams;
        for( int i = 0; i < attachments.Count; ++i )
        {
            this.m_currentAttachmentIndex = i;

            EntityAttachmentParams attachment = attachments[i];
            this.m_controlIdMap[attachment.name] = -1;

            Vector3 position = this.m_entityManager.transform.position + (this.m_entityManager.transform.rotation * attachment.position);
            Quaternion rotation = attachment.rotation;

            // /// Debug.Log(attachment.name);
            Handles.color = Color.red;
            Handles.FreeMoveHandle( 
                position, 
                rotation, 
                HandleSize, 
                SnapThreshold, 
                this.DrawHandle );

            // /// Debug.Log ("current=" + attachment.name + ", last=" + this.m_lastSelectedHandle );
            // /// Debug.Log( Tools.current );
            if( attachment.name == this.m_lastSelectedHandle )
            {
                switch( Tools.current )
                {
                    case Tool.Move:
                    {
                        Vector3 newPosition = Handles.PositionHandle(position, rotation);
                        newPosition = Quaternion.Inverse(this.m_entityManager.transform.rotation) * (newPosition - this.m_entityManager.transform.position);
                        if( newPosition != position )
                        {
                            attachment.position = newPosition;
                            position = this.m_entityManager.transform.position + (this.m_entityManager.transform.rotation * attachment.position);
                            rotation = attachment.rotation;
                        }
                        break;
                    } 
                    case Tool.Rotate:
                    {
                        Quaternion newRotation = Handles.RotationHandle(rotation, position);
                        // newRotation = Quaternion.Inverse(this.m_entityManager.transform.rotation) * (newPosition - this.m_entityManager.transform.position);
                        if( newRotation != rotation )
                        {
                            attachment.rotation = newRotation;
                            // position = this.m_entityManager.transform.position + (this.m_entityManager.transform.rotation * attachment.position);
                            rotation = attachment.rotation;
                        }
                        break;
                    } 
                    case Tool.Scale:
                    default:
                    {
                        break;
                    }
                }
            }

            Vector3 labelPosition = position; 
            Handles.Label( labelPosition, attachment.name, this.m_labelStyle );
        }

        this.m_currentAttachmentIndex = -1;
        */
    }

    //! @brief The callback function for drawing a handle as requested in OnSceneGUI().
    //! @hack Really just using this because I don't know another way to reliably get ahold of a handle's control ID...
    private void DrawHandle(int controlId, Vector3 position, Quaternion rotation, float size)
    {
        /*
        List<EntityAttachmentParams> attachments = this.m_entityManager.attachmentParams;
        EntityAttachmentParams attachment = attachments[this.m_currentAttachmentIndex];
        this.m_controlIdMap[ attachment.name ] = controlId;

        // /// Debug.Log ("control-id=" + controlId);
        Handles.DotCap(controlId,
            position,
            rotation,
            size);

        // 
        // /// Debug.Log( "current-control=" + this.m_controlIdMap[attachment.name] + ", hot-control=" + GUIUtility.hotControl );
        if( this.m_controlIdMap[attachment.name] == GUIUtility.hotControl )
        {
            this.m_lastSelectedHandle = attachment.name;
        }
        */
    }
}
