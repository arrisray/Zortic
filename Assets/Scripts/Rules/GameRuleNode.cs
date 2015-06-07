using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public abstract class GameRuleNode : ScriptableObject
{
	// --------------------------------------------------------------------------
    // DATA MEMBERS
	
	[SerializeField] public string id = string.Empty;
	[SerializeField] [HideInInspector] public List<Port> m_inputs = new List<Port>();
	[SerializeField] [HideInInspector] public List<Port> m_outputs = new List<Port>();
	
	// --------------------------------------------------------------------------
    // PROPERTIES

	public List<Port> inputs
	{
		get { return this.m_inputs; }
	}	

	public List<Port> outputs
	{
		get { return this.m_outputs; }
	}	

	public Port this[string id]
	{
	    get
	    {
	        Port port = this.FindPort( id, this.m_inputs );
			if( port == null )
			{
				port = this.FindPort( id, this.m_outputs );
			}
			return port;
	    }
	}

	// --------------------------------------------------------------------------
    // METHODS
	
	public virtual void OnStart() {}
	public virtual void OnLoad() {}
	public virtual void OnUnload() {}
	public virtual void OnReset() {}
	public virtual void OnUpdate() {}
	public abstract void CreateInputs();
	public abstract void CreateOutputs();
	public virtual void OnPostEventRaised() {}
	
	public virtual void Load()
	{
		if( this.m_inputs.Count == 0 )
        {
            this.CreateInputs();
        }
  
        if( this.m_outputs.Count == 0 )
        {      
            this.CreateOutputs();
        }
		
		this.OnLoad();
	}
	
	public virtual void Unload()
	{
		this.Reset();
		this.OnUnload();
	}
	
	public virtual void Update()
	{
		this.OnUpdate();
	}
	
	public virtual void Reset()
	{
		this.ClearPorts();
		this.OnReset();
	}
	
	private void ClearPorts()
    {
        this.ClearPortData( this.m_inputs );
        this.ClearPortData( this.m_outputs );
    } 
   
    private void ClearPortData( List<Port> ports )
    {
        foreach( Port port in ports )
        {
            port.data = string.Empty;
        }
    }
	
	private Port FindPort( string id, List<Port> ports )
    {
        foreach( Port port in ports )
        {
            if( port.id == id )
            {
                return port;
            }
        }
        return null;
    }
}

