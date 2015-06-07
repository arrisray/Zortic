using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// --------------------------------------------------------------------------
//! @brief 
[Serializable]
public class EntityIndicatorInfo
{
    // --------------------------------------------------------------------------
    // PROPERTIES

	public String id;
    public Color currentOutlineColor;
    public float currentTextureOpacity;
    public float duration;
    public Color outlineColor;
    public Texture2D[] textures;
    public int textureIndex;
    public Rect rect;

    // --------------------------------------------------------------------------
    // METHODS

    public EntityIndicatorInfo()
    {
    	this.id = "";
        this.currentOutlineColor = Color.white;
        this.currentTextureOpacity = 0.0f;
        this.duration = 0.0f;
        this.outlineColor = Color.white;
        this.textures = null;
        this.textureIndex = 0; 
    }

    public override String ToString() 
    {
    	return "EntityIndicatorInfo={\n" 
    		+ "\t.id: " + this.id + "\n"
    		+ "\t.currentOutlineColor: " + this.currentOutlineColor + "\n"
    		+ "\t.currentTextureOpacity: " + this.currentTextureOpacity + "\n"
    		+ "\t.duration: " + this.duration + "\n"
    		+ "\t.outlineColor: " + this.outlineColor + "\n"
    		+ "\t.numTextures: " + this.textures.Length + "\n"
    		+ "\t.textureIndex: " + this.textureIndex + "\n"
    		+ "\t.rect: " + this.rect + "\n"
    		+ "}";
    }
}