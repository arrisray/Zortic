using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffParams
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS
	
	public static int MIN_LEVEL = 1;
	
	public BuffGui gui = null;
	public EntityManager recipient = null;
	protected List<EntityWeapon> weapons = null;
	protected int buffLevel = BuffParams.MIN_LEVEL;
	protected float activationTime = 0.0f;
	protected ModifyProjectileDamageEvent buffEvent = null;
	protected bool hasEmittedWarning = false;
	
	// --------------------------------------------------------------------------
    // METHODS
	
	public BuffParams( EntityManager recipient, ModifyProjectileDamageEvent buffEvent )
	{
		this.buffEvent = buffEvent;
		this.recipient = recipient;
		this.buffLevel = MIN_LEVEL;
		this.activationTime = Time.time;
		this.hasEmittedWarning = false;
		
		this.weapons = new List<EntityWeapon>();
		foreach( EntityWeapon weapon in recipient.weaponManager.weapons )
		{
			if( weapon.data.game.actionType == this.buffEvent.actionType )
			{
				this.weapons.Add( weapon );
			}
		}
		
		MessageManager.Instance.RegisterMessageHandler<EntityDestroyedMessage>( this.OnEntityDestroyed );
	}
	
	public void OnEntityDestroyed( Message message )
    {
        EntityDestroyedMessage nativeMessage = (EntityDestroyedMessage)message;
        GameObject go = nativeMessage.victim;
        EntityManager entity = go.GetComponent<EntityManager>();
		if( entity == this.recipient )
		{
			this.buffLevel = MIN_LEVEL;
			this.Reset();
		}
	}

	public bool Update()
	{
		if( this.buffLevel == MIN_LEVEL )
		{
			return true;
		}
		
		float elapsedTime = Time.time - this.activationTime;
		float beginWarningPeriod = this.buffEvent.duration - this.buffEvent.warningPeriod;
		if( ( elapsedTime > beginWarningPeriod )
			&& ( elapsedTime < this.buffEvent.duration ) )
		{
			float percentRemaining = Mathf.Clamp01( ( this.buffEvent.duration - elapsedTime ) / this.buffEvent.warningPeriod );
			this.EmitDebuffWarning( percentRemaining );
		}
		else if( elapsedTime > this.buffEvent.duration )
		{
			this.Debuff();
		}

		return ( this.buffLevel != MIN_LEVEL );
	}

	public void Buff()
	{
		++this.buffLevel;
		this.Reset();
		
		foreach( EntityWeapon weapon in this.weapons )
		{
			weapon.PowerUp();
		}
		// /// Debug.Log( "[BUFF] " + this.recipient.name );
	}

	protected void Debuff()
	{
		--this.buffLevel;
		this.Reset();
		
		foreach( EntityWeapon weapon in this.weapons )
		{
			weapon.PowerDown();
		}
		// /// Debug.Log( "[DEBUFF] " + this.recipient.name );
	}

	protected void EmitDebuffWarning( float percent )
	{
		foreach( EntityWeapon weapon in this.weapons )
		{
			weapon.PowerDownWarningPeriod( percent );
			// /// Debug.Log ( "[DEBUFF] Percent warning period remaining: " + percent );
		}
	}

	protected void Reset()
	{
		this.activationTime = Time.time;
		this.hasEmittedWarning = false;

		foreach( EntityWeapon weapon in this.weapons )
		{
			// Update current weapon damage
			weapon.data.game.damage = ( this.buffLevel * (int)this.buffEvent.amount );
			
			// Constrain to valid values, i.e. [MIN_LEVEL, weapon.data.game.maxDamage]
			weapon.data.game.damage = Mathf.Min( weapon.data.game.damage, weapon.data.game.maxDamage );
			weapon.data.game.damage = Mathf.Max( weapon.data.game.damage, MIN_LEVEL );
			
			// /// Debug.Log( "[DE/BUFF] " + weapon.name + " damage: " + weapon.data.game.damage );
		}
		
		/*
		if( this.gui != null )
		{
			this.gui.Show( "x" + this.buffLevel );
		}
		*/
	}
}

