using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DestroyCondition : GameRuleCondition
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

    [SerializeField] public EntityTag subject = null;
    [SerializeField] public EntityTag target = null;
	[SerializeField] public int count = 0;
    [SerializeField] [HideInInspector] private int m_currentCount = 0;

	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS

    public override void OnLoad()
    {		
    	MessageManager.Instance.RegisterMessageHandler<EntityDestroyedMessage>( this.OnEntityDestroyed );
    }

    public override void OnUnload()
    {
		this.m_currentCount = 0;
        MessageManager.Instance.UnregisterMessageHandler<EntityDestroyedMessage>( this.OnEntityDestroyed );
    }

	public override void CreateInputs()
	{
	}

	public override void CreateOutputs()
	{
		Port port = new Port();
		port.id = "attacker";
		port.type = EditorDataType.Vector3;
		this.m_outputs.Add( port );

		port = new Port();
		port.id = "victim";
		port.type = EditorDataType.Vector3;
		this.m_outputs.Add( port );

		port = new Port();
		port.id = "attackerId";
		port.type = EditorDataType.InstanceId;
		this.m_outputs.Add( port );
	}
	
	public override void OnReset()
	{
		this.m_currentCount = 0;
	}

    private void OnEntityDestroyed( Message message )
    {
    	EntityDestroyedMessage derivedMessage = message as EntityDestroyedMessage;

        EntityManager attacker = derivedMessage.attacker ? derivedMessage.attacker.GetComponent<EntityManager>() : null;
        EntityManager victim = derivedMessage.victim.GetComponent<EntityManager>();
		
		// /// Debug.Log( derivedMessage.attacker + " destroyed " + derivedMessage.victim + "." + " Current count=" + this.m_currentCount + ", target count=" + this.count);
        if( attacker && attacker.entityTags.Contains(subject) && victim.entityTags.Contains(target) )
        {
            ++this.m_currentCount;
            // /// Debug.Log( derivedMessage.attacker + " destroyed " + derivedMessage.victim + "." + " Current count=" + this.m_currentCount + ", target count=" + this.count);

            if( this.m_currentCount >= this.count )
            {
                //! @note Tricky trivia: Use the GameObject instance ID rather than the EntityManager's because
                //  Unity's InstanceID system is only gonna recognize the GameObject instance's ID(..ish)...!
                this["attackerId"].data = attacker.gameObject.GetInstanceID().ToString();     
				this["attacker"].data = attacker.transform.position.ToString();
				this["victim"].data = victim.transform.position.ToString();
				
				// /// Debug.Log( "attacker=" + attacker.name + ", id=" + attacker.GetInstanceID() );
				// /// Debug.Log( "victim-pos=" + victim.transform.position.ToString() );

                this.isSatisfied = true;
				return;
            }
        }
    }
} // class DestroyedEntityCountCondition
