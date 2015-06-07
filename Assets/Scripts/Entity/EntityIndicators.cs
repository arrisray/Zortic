using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(EntityManager) )]
public class EntityIndicators : MonoBehaviour
{
#if FALSE
    // --------------------------------------------------------------------------
    // DATA MEMBERS

	private EntityManager m_entity = null;
	private List<EntityIndicatorInfo> m_activeIndicators = new List<EntityIndicatorInfo>();
	private EntityIndicatorInfo m_currentIndicatorInfo = null;
	private EntityIndicatorInfo m_spawnIndicatorInfo = null;
	private EntityIndicatorInfo m_damageIndicatorInfo = null;
	private EntityIndicatorInfo m_repairIndicatorInfo = null;
	private EntityIndicatorInfo m_livesIndicatorInfo = null;

    // --------------------------------------------------------------------------
    // PROPERTIES

	public EntityManager Entity { get { return this.m_entity; } }
	public EntityInputHandler inputHandler { get { return this.m_entity.inputHandler; } }
	public EntityIndicatorInfo repairInfo { get { return this.m_repairIndicatorInfo; } }
	public EntityIndicatorInfo damageInfo { get { return this.m_damageIndicatorInfo; } }
	public EntityIndicatorInfo livesInfo { get { return this.m_livesIndicatorInfo; } }
	public EntityIndicatorInfo spawnInfo { get { return this.m_spawnIndicatorInfo; } }
	public EntityIndicatorInfo currentInfo { get { return this.m_currentIndicatorInfo; } }
	public List<EntityIndicatorInfo> activeIndicators { get { return this.m_activeIndicators; } }

    // --------------------------------------------------------------------------
    // METHODS

	public void Start () 
	{
		this.m_entity = this.gameObject.GetComponent<EntityManager>();
		this.m_currentIndicatorInfo = null;

	    if (this.m_entity.data.visual.outlineMaterial != null)
	    {
		    this.m_spawnIndicatorInfo = new EntityIndicatorInfo(); 
		    this.m_spawnIndicatorInfo.id = "spawn";
		    this.m_spawnIndicatorInfo.duration  = 5.0f;
		    this.m_spawnIndicatorInfo.outlineColor = this.m_entity.data.visual.outlineMaterial.color;
		    this.m_spawnIndicatorInfo.textures = null;
		    this.m_spawnIndicatorInfo.textureIndex = 0;
	    }

	    if (this.m_entity.data.visual.healthDamage.Length > 0)
	    {
		    this.m_damageIndicatorInfo = new EntityIndicatorInfo(); 
		    this.m_damageIndicatorInfo.id = "damage";
		    this.m_damageIndicatorInfo.duration  = 5.0f;
		    this.m_damageIndicatorInfo.outlineColor = new Color(1.0f, 0.3f, 0.0f, 1.0f);
		    this.m_damageIndicatorInfo.textures = this.m_entity.data.visual.healthDamage;
		    this.m_damageIndicatorInfo.textureIndex = this.m_entity.data.visual.healthDamage.Length - 1;
	    }

	    if (this.m_entity.data.visual.health.Length > 0)
	    {
		    this.m_repairIndicatorInfo = new EntityIndicatorInfo(); 
		    this.m_repairIndicatorInfo.id = "repair";
		    this.m_repairIndicatorInfo.duration  = 5.0f;
		    this.m_repairIndicatorInfo.outlineColor = new Color(0.0f, 0.5f, 1.0f, 1.0f);
		    this.m_repairIndicatorInfo.textures = this.m_entity.data.visual.health;
		    this.m_repairIndicatorInfo.textureIndex = this.m_entity.data.visual.health.Length - 1;
		}

	    if (this.m_entity.data.visual.lives.Length > 0)
	    {
		    this.m_livesIndicatorInfo = new EntityIndicatorInfo(); 
		    this.m_livesIndicatorInfo.id = "livesIndicator";
		    this.m_livesIndicatorInfo.duration  = 5.0f;
		    this.m_livesIndicatorInfo.outlineColor = new Color(0.25f, 0.25f, 1.0f, 1.0f);
		    this.m_livesIndicatorInfo.textures = this.m_entity.data.visual.lives;
		    this.m_livesIndicatorInfo.textureIndex = this.m_entity.data.visual.lives.Length - 1;
		}
	}

	public void OnGUI() 
	{
	    if( this.m_currentIndicatorInfo != null )
	    {
	        this.StartCoroutine( this.AnimateCurrentIndicator(this.m_currentIndicatorInfo) );
	        this.m_currentIndicatorInfo = null;
	    }

	    // Generic Entity indicators, e.g. health, life
	    foreach (EntityIndicatorInfo info in this.m_activeIndicators)
	    {
	    	if( !this.Entity.data.visual.currentOutlineMaterial )
	    	{
	    		// /// Debug.LogWarning( "The current indicator info (=" + info.id + ") has no assigned outline material." );
	    		continue;
	    	}
	        this.Entity.data.visual.currentOutlineMaterial.color = info.currentOutlineColor;

	        Color color = GUI.color;

	        Color infoColor = Color.white;
	        infoColor.a = info.currentTextureOpacity;

	        GUI.color = infoColor;

	        GUI.Label(info.rect, info.textures[info.textureIndex]);

	        GUI.color = color;
	    }

	    /*
		// Display num lives indicator
		if(this.m_player.data.visual.livesGUI && this.m_player.numLives > 0 && this.m_player.livesCountdown > 0.0f)
		{
			this.m_player.livesCountdown -= Time.deltaTime;
			GUI.Label(Rect(playerScreenPos.x-(livesGUISize/2), playerScreenPos.y-(livesGUISize/2), livesGUISize, livesGUISize), this.m_player.data.visual.livesGUI[this.m_player.numLives-1]); 
		}
		*/

	    // Energy Bar
	    if (this.m_entity.data.visual.energyCurImage)
	    {
	        float energyBar = (Screen.width-20) * (this.m_entity.currentEnergyLevel / this.m_entity.data.game.energyCap);
	        GUI.DrawTexture (new Rect (10, Screen.height-10, energyBar, 5), this.m_entity.data.visual.energyCurImage, ScaleMode.StretchToFill);
	    }
	}

	public void Reset()
	{
		this.StopAllCoroutines();
		this.m_currentIndicatorInfo = null;
		this.m_activeIndicators.Clear();	
	}

	// --------------------------------------------------------------------------
	//! @brief 
	public int FindActiveIndicator(EntityIndicatorInfo targetInfo) 
	{
		int index = 0;
		foreach (EntityIndicatorInfo info in this.m_activeIndicators)
		{
			if (info == targetInfo)
			{
				return index;
			}
			++index;
		}
		return -1;
	}

	// --------------------------------------------------------------------------
	//! @brief 
	public bool IsIndicatorActive(EntityIndicatorInfo targetInfo) 
	{
		return (this.FindActiveIndicator(targetInfo) != -1);
	}

	// --------------------------------------------------------------------------
	//! @brief 
	public void SetCurrentIndicator(EntityIndicatorInfo info, int value)
	{
		if( info == null )
		{
			// /// Debug.LogWarning( "Received null indicator info!" );
			return;
		}

		info.textureIndex = value;

		if ((info == null) || this.IsIndicatorActive(info))
		{
			return; 
		}

		this.m_currentIndicatorInfo = info; 
		this.m_activeIndicators.Add(this.m_currentIndicatorInfo);
	}

	// --------------------------------------------------------------------------
	//! @brief 
	public IEnumerator AnimateCurrentIndicator(EntityIndicatorInfo info) 
	{ 
		if ( (info == null) || (info.textures == null) || (info.textures.Length == 0) )
		{
			/// Debug.LogWarning("Attempted to animate an indicator with an no assigned textures.\n"
				+ "Details={\n"
				+ "\tinfo.id: " + info.id + "\n"
				+ "\tinfo.textureIndex: " + info.textureIndex + "\n"
				+ "}");
			yield break;
		}

		if (info.textureIndex < 0)
		{
			/// Debug.LogWarning("Attempted to animate an indicator with an invalid texture index.\n"
				+ "Details={\n"
				+ "\tinfo.id: " + info.id + "\n"
				+ "\tinfo.textureIndex: " + info.textureIndex + "\n"
				+ "}");
			yield break;
		}

		float elapsed = 0.0f;
		float duration = info.duration;
		// Texture2D texture = info.textures[info.textureIndex];

		while (elapsed < duration)
		{
			if( this.m_entity.data.visual.outlineMaterial )
			{
				float lerpFactor = elapsed / duration;
			    var healthSize = 120.0f; //! @hack Hardcoded value!!!
			    Vector2 thisPos = Camera.main.WorldToScreenPoint(this.transform.position);
			    Color targetOutlineColor = this.m_entity.data.visual.outlineMaterial.color;
			    thisPos.y = Screen.height - thisPos.y;

			    info.rect = new Rect(thisPos.x-(healthSize/2), thisPos.y-(healthSize/2), healthSize, healthSize);
			    info.currentTextureOpacity = Mathf.Lerp(1.0f, 0.0f, lerpFactor);
			    info.currentOutlineColor = Color.Lerp(info.outlineColor, targetOutlineColor, lerpFactor);
			}

		    yield return null;

		    elapsed += Time.deltaTime;
		}

		info.currentTextureOpacity = 0.0f;
		info.currentOutlineColor = Color.white;
		info.currentOutlineColor.a = 0.0f;

		int index = this.FindActiveIndicator(info);
		this.m_activeIndicators.RemoveAt(index);
	}
#endif
}
