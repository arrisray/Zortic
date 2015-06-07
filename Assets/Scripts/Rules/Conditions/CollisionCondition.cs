using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TSDC;

[Serializable]
public class CollisionCondition : GameRuleCondition
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS
 
	[SerializeField] public List<UnityEngine.Object> collider = new List<UnityEngine.Object>();
	[SerializeField] public List<UnityEngine.Object> target = new List<UnityEngine.Object>();
	[SerializeField] public bool ignoreProxyCollisions = true;
	[SerializeField] public bool killCollider = false;
	[SerializeField] [HideInInspector] protected EntityCollisionMessage message = null;
	[SerializeField] [HideInInspector] protected bool m_wasCollisionIgnored = false;

	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS
	
	public override void Reset()
	{
		this.m_wasCollisionIgnored = true;
		this.message = null;
		base.Reset();
	}
	
    public override void OnLoad()
    {
    	MessageManager.Instance.RegisterMessageHandler<EntityCollisionMessage>( this.OnEntityCollided );
    }

    public override void OnUnload()
    {
		this.m_wasCollisionIgnored = false;
        MessageManager.Instance.UnregisterMessageHandler<EntityCollisionMessage>( this.OnEntityCollided );
    }

	public override void CreateInputs()
	{
	}

	public override void CreateOutputs()
	{
		Port port = new Port();
		port.id = "targetId";
		port.type = EditorDataType.InstanceId;
		this.m_outputs.Add( port );

		port = new Port();
		port.id = "colliderId";
		port.type = EditorDataType.InstanceId;
		this.m_outputs.Add( port );

        port = new Port();
        port.id = "colliderDamage";
        port.type = EditorDataType.Float;
        this.m_outputs.Add( port );

        port = new Port();
        port.id = "targetDamage";
        port.type = EditorDataType.Float;
        this.m_outputs.Add( port );
	}
	
	public override void OnPostEventRaised()
    {
        if( this.message == null )
        {
            return;
        }
  
        EntityManager colliderEntity = this.message.collider.GetComponent<EntityManager>();
        EntityManager targetEntity = this.message.target.GetComponent<EntityManager>();
		
		// /// Debug.Log( this.name + ", collider: " + colliderEntity.name + ", target: " + targetEntity.name + ", ignored=" + this.m_wasCollisionIgnored );

        if( colliderEntity && this.killCollider && !this.m_wasCollisionIgnored )
        {
            colliderEntity.Die( targetEntity.identity );
        }

        this.message = null;
    }

	protected void OnEntityCollided( Message message )
	{
        EntityCollisionMessage derivedMessage = message as EntityCollisionMessage;

		EntityManager targetEntity = derivedMessage.target.GetComponent<EntityManager>();
		EntityManager colliderEntity = derivedMessage.collider.GetComponent<EntityManager>();
		
		// /// Debug.Log( this.name + ": target=" + targetEntity.name + ", collider=" + colliderEntity.name );

		if( this.ignoreProxyCollisions && (targetEntity == colliderEntity.proxyForEntity) )
		{
			// /// Debug.LogWarning( this.name + ": Ignoring proxy collision." );
			// /// Debug.Log( this.name + " Details: target=" + targetEntity.name + ", collider=" + colliderEntity.name );
			return;
		}
        
		if( !targetEntity || !colliderEntity )
		{
			// /// Debug.LogWarning( this.name + ": NULL target AND/OR collider." );
			// /// Debug.Log( this.name + " Details: target=" + targetEntity.name + ", collider=" + colliderEntity.name );
			return;
		}

		if( !targetEntity.entityTags.Contains(target) )
		{
			// /// Debug.LogWarning( this.name + ": Failed target tag filter." );
			// /// Debug.Log( this.name + " Details: target=" + targetEntity.name + ", collider=" + colliderEntity.name );
			return;
		}

		if( !colliderEntity.entityTags.Contains(collider) )
		{
			// /// Debug.LogWarning( this.name + ": Failed collider tag filter." );
			// /// Debug.Log( this.name + " Details: target=" + targetEntity.name + ", collider=" + colliderEntity.name );
			return;
		}
		
        // /// Debug.Log( this.name + ": target=" + targetEntity.name + ", collider=" + colliderEntity.name );
		this["targetId"].data = derivedMessage.target.GetInstanceID().ToString();
		this["colliderId"].data = derivedMessage.collider.GetInstanceID().ToString();
        this["colliderDamage"].data = colliderEntity.data.game.collisionDamage.ToString();
        this["targetDamage"].data = targetEntity.data.game.collisionDamage.ToString();

		this.m_wasCollisionIgnored = false;
		this.message = derivedMessage;
		this.isSatisfied = true;
	}
}

