using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityTags : MonoBehaviour
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS

	public List<EntityTag> entityTags = new List<EntityTag>();

	// --------------------------------------------------------------------------
    // PROPERTIES

	// --------------------------------------------------------------------------
    // METHODS	

    public void OnEnable()
    {
    }

    public void OnDisable()
    {
    }

	public bool Add( EntityTag tag )
	{
		if( !this.entityTags.Contains( tag ) )
		{
			this.entityTags.Add( tag );
			GameManager.Instance.RegisterPrefab( tag, this.gameObject );
			return true;
		}
		return false;
	}

	public bool Remove( EntityTag tag )
	{
		if( this.entityTags.Remove( tag ) )
		{
			GameManager.Instance.UnregisterPrefab( tag, this.gameObject );
			return true;
		}
		return false;
	}

	public bool Contains( List<UnityEngine.Object> tags )
	{
		foreach( UnityEngine.Object tagObject in tags )
        {
			EntityTag tag = tagObject as EntityTag;
            if( tag && this.Contains( tag ) )
			{
				return true;
			}
        }
		return false;
	}

	public bool Contains( EntityTag tag ) 
	{
		if( !tag )
		{
			// /// Debug.LogWarning( "Requested a search for a NULL tag." );
			return false;
		}

		int index = 0;
		foreach( EntityTag entityTag in this.entityTags )
        {
			if( !entityTag )
			{
				/// Debug.LogError( "Detected a NULL entity tag (=" + this + ") at index " + index + "." );
				continue;
			}
			
			if( EntityTag.IsEqual( entityTag, tag ) )
			{
				return true;
			}

			++index;
        }
		return false;
	}

    protected void Register()
    {
        foreach( EntityTag entityTag in this.entityTags )
        {
            GameManager.Instance.RegisterPrefab( entityTag, this.gameObject );
        }
    }

    protected void Unregister()
    {
        //! @hack !!!!!!!
        if( GameManager.Instance == null )
        {
            return;
        }

        foreach( EntityTag entityTag in this.entityTags )
        {
            GameManager.Instance.UnregisterPrefab( entityTag, this.gameObject );
        }
    }
}
