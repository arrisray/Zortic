using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DebugInfoIndicator
{
	public enum InfoType
	{
		None,
		Health,
		Weapons
	}
	
	[Serializable]
	public class Data
	{
		public Color color = Color.white;
		public UnityEngine.Font font = null;
		public int fontSize = 1;
		public TextAnchor anchor = TextAnchor.MiddleLeft;
		public TextAlignment alignment = TextAlignment.Left;
	}
	
	public GameObject parent = null;
	public InfoType info = InfoType.None;
	public Vector3 offset = Vector3.zero;
	private GameObject gameObject = null;
	private int fieldIndex = 0;
	private EntityManager entity = null;
	private GUIText guiText = null;
	
	public void Init( int index )
	{
		if( this.parent != null )
		{
			this.fieldIndex = index;
			
			this.entity = this.parent.GetComponent<EntityManager>();
			
			this.gameObject = new GameObject( "debug-info-" + Guid.NewGuid().ToString() );
			this.gameObject.transform.position = Vector3.zero;
			
			this.guiText = this.gameObject.AddComponent<GUIText>();
			
			this.gameObject.transform.parent = GameManager.Instance.runtimeIndicatorsNode.transform;
		}
	}
	
	public void OnGUI()
	{
		if( this.entity == null )
		{
			/// Debug.LogWarning( "No entity associated with DebugInfoIndicator!" );	
			return;
		}
		
		this.guiText.transform.position = Vector3.zero;
		switch( this.info )
		{
			case InfoType.Health:
			{
				this.guiText.text = "Health: " + this.entity.currentHealth.ToString();
				break;
			}
			case InfoType.Weapons:
			{
				foreach( EntityWeapon weapon in this.entity.weaponManager.weapons )
				{
					this.guiText.text = "Weapon: { name: " + weapon.name + ", damage: " + this.entity.weaponManager.weapons[0].data.game.damage.ToString() + " }";
				}
				break;
			}
			case InfoType.None:
			default:
			{
				break;
			}
		}
		
		Vector3 offset = this.offset;
		offset.z -= ( this.fieldIndex * GameManager.Instance.debugInfoIndicatorData.fontSize );
		
		Camera camera = GameManager.Instance.sceneCamera;
		Vector3 pos = camera.WorldToViewportPoint( this.parent.transform.position + offset );
		
		this.guiText.font = GameManager.Instance.debugInfoIndicatorData.font;
		this.guiText.fontSize = GameManager.Instance.debugInfoIndicatorData.fontSize;
		this.guiText.material.color = GameManager.Instance.debugInfoIndicatorData.color;
		this.guiText.alignment = GameManager.Instance.debugInfoIndicatorData.alignment;
		this.guiText.anchor = GameManager.Instance.debugInfoIndicatorData.anchor;
		this.guiText.transform.position = pos;
	}
}

