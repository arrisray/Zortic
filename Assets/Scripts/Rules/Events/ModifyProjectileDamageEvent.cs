using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyProjectileDamageEvent : GameRuleEvent
{
    // --------------------------------------------------------------------------
    // DATA MEMBERS

    public float amount = 0.0f;
	public float duration = 3.0f; //! @note In seconds.
	public float warningPeriod = 2.0f; //! @note Seconds left before damage mod expires.
	public BuffGui gui = null;
	public EntityWeapon.ActionType actionType = EntityWeapon.ActionType.Primary;
	public Dictionary<EntityManager, BuffParams> m_buffs = new Dictionary<EntityManager, BuffParams>();

    // --------------------------------------------------------------------------
    // PROPERTIES

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
            return null;
        }
    }

    // --------------------------------------------------------------------------
    // METHODS
	
	public override void Reset()
	{
		this.m_buffs.Clear();
	}
	
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
    }

    public override void CreateOutputs()
    {
    }

    public override void OnRaise()
	{
        // /// Debug.Log( "ModifyProjectileDamageAmount: recipient=" + this.recipient.name );
		
        EntityManager recipient = this.recipient;
        if( !recipient)
        {
            return;
        }
		
		BuffParams buffParams = null;
		if( !this.m_buffs.ContainsKey( recipient ) )
		{
			buffParams = new BuffParams( recipient, this );
			
			GameObject indicatorNode = recipient.indicatorNode;
			if( (this.gui != null) && (indicatorNode != null) )
			{
				buffParams.gui = UnityEngine.Object.Instantiate( this.gui, Vector3.zero, Quaternion.identity ) as BuffGui;
				buffParams.gui.m_entity = recipient.gameObject;
				buffParams.gui.transform.parent = GameManager.Instance.runtimeGuiNode.transform;
				buffParams.gui.transform.localScale = Vector3.one;
			}
			
			this.m_buffs.Add( recipient, buffParams );
		}
		else
		{
			buffParams = this.m_buffs[ recipient ];
		}
		buffParams.Buff();
		
		this.OnComplete();
    }

	public override void OnUpdate() 
    {
		// Update buff'ed entities
		List<EntityManager> removeEntities = new List<EntityManager>();
		List<BuffParams> buffs = new List<BuffParams>( this.m_buffs.Values );
		foreach( BuffParams buff in buffs )
		{
			if( !buff.Update() )
			{
				removeEntities.Add( buff.recipient );
			}
		}
		
		// Garbage collect un-buffed entities
		foreach( EntityManager entity in removeEntities )
		{
			// /// Debug.Log( "Removing " + entity.name + " from BUFF checks..." );
			this.m_buffs.Remove( entity );
		}
    }
} // class ModifyProjectileDamageEvent
