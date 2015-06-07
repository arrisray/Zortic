using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyHealthEvent : GameRuleEvent
{
    // --------------------------------------------------------------------------
    // DATA MEMBERS

    public float health = 0.0f;
    public bool isDamage = false;
	public UnityEngine.Object recipientEntity = null;

    // --------------------------------------------------------------------------
    // PROPERTIES

    protected float amount
    {
        get
        {
            bool wasModifiedFromInputPort = false;
            float health = 0.0f, amount = 0.0f;      
            if( float.TryParse(this[ "damage" ].data, out amount) )
            {
                wasModifiedFromInputPort = true;
                health += amount * -1.0f;
            }
        
            if( float.TryParse(this[ "health" ].data, out amount) )
            {
                wasModifiedFromInputPort = true;
                health += amount;
            }
    
            if( !wasModifiedFromInputPort )
            {
                health = this.health;
                health *= this.isDamage ? -1.0f : 1.0f;
            }
            return health;
        }
    }

    protected EntityManager recipient
    {
        get
        {
            if( (this[ "recipientId" ] != null) && !string.IsNullOrEmpty( this[ "recipientId" ].data ) )
            {
                int instanceId = 0;
                if( Int32.TryParse( this[ "recipientId" ].data, out instanceId ) )
                {
                    GameObject instance = GameManager.Instance.FindObjectByInstanceId( instanceId ) as GameObject;
                    if( instance )
                    {
                        return instance.GetComponent<EntityManager>();
                    }
                }
            }
			else if( this.recipientEntity )
			{
				GameObject instance = GameManager.Instance.FindInstanceByTag( this.recipientEntity );
				return ( instance != null ) ? instance.GetComponent<EntityManager>() : null;
			}
            return null;
        }
    }

    protected EntityManager source
    {
        get
        {
            if( (this[ "sourceId" ] != null) && !string.IsNullOrEmpty( this[ "sourceId" ].data ) )
            {
                int instanceId = 0;
                if( Int32.TryParse( this[ "sourceId" ].data, out instanceId ) )
                {
                    GameObject instance = GameManager.Instance.FindObjectByInstanceId( instanceId ) as GameObject;
                    if( instance )
                    {
                        return instance.GetComponent<EntityManager>();
                    }
                }
            }
            return null;
        }
    }

    // --------------------------------------------------------------------------
    // METHODS

    public override void OnLoad()
    {
    }

    public override void OnUnload()
    {
    }

    public override void CreateInputs()
    {
        Port port = new Port();
        port.id = "recipientId";
        port.type = EditorDataType.InstanceId;
        this.m_inputs.Add( port );

        port = new Port();
        port.id = "sourceId";
        port.type = EditorDataType.InstanceId;
        this.m_inputs.Add( port );
  
        port = new Port();
        port.id = "damage";
        port.type = EditorDataType.Float;
        this.m_inputs.Add( port );

        port = new Port();
        port.id = "health";
        port.type = EditorDataType.Float;
        this.m_inputs.Add( port );
    }

    public override void CreateOutputs()
    {
    }
	
	public override void OnRaise()
    {
        float health = this.amount;

        // /// Debug.Log( "ModifyHealth: recipient=" + this.recipient.name + ", source=" + this.source.name );

        EntityManager recipient = this.recipient;
        if( recipient )
        {
            EntityManager sourceEntity = this.source != null ? this.source.identity : null;
            recipient.UpdateHealth( health, sourceEntity );
        }
		
		this.OnComplete();
    }
} // class RewardPointsEvent