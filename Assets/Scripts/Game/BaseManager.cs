using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager : Vehicle
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

    protected GameManager gameManager = null;

	// --------------------------------------------------------------------------
    // PROPERTIES

    public EntityTags entityTags
    {
        get
        {
            return this.gameObject.GetComponent<EntityTags>();
        }
    }

	public Collider[] colliders
	{
		get
		{
			return this.GetComponentsInChildren<Collider>();
		}
	}

	// --------------------------------------------------------------------------
    // METHODS

    public virtual void OnEnable()
    {
        this.Setup();
    }

    public virtual void OnDestroy()
    {
        this.Teardown();
    }

    public virtual void OnApplicationQuit()
    {
    }

    public virtual void Setup()
    {
        this.gameManager = GameManager.Instance;
    }

    public virtual void Teardown()
    {
    }
} // public class BaseManager
