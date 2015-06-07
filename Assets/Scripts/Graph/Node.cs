using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.InteropServices;
using UnityEngine;
using TSDC;

[Serializable]
public class Node : MonoBehaviour
{
	// --------------------------------------------------------------------------
    // ENUMS
	
	public enum IoType
	{
		Input,
		Output
	}

	// --------------------------------------------------------------------------
    // DATA MEMBERS

	[SerializeField] protected List<Port> m_inputs = new List<Port>();
	[SerializeField] protected List<Port> m_outputs = new List<Port>();

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
	
	// --------------------------------------------------------------------------
    // METHODS
	/*
	public Port AddPort( IoType ioType )
	{
		Port port = this.gameObject.AddComponent<Port>();
		if( ioType == IoType.Input )
		{
			this.m_inputs.Add( port );
		}
		else
		{
			this.m_outputs.Add( port );
		}
		return port;
	}

	public bool RemovePort( Port port, IoType ioType )
	{
		bool result = false;
		if( ioType == IoType.Input )
		{
			result = this.m_inputs.Remove( port );
		}
		else
		{
			result = this.m_outputs.Remove( port );
		}

		Destroy( port );
		return result;
	}
	*/
	/*
	public bool AddInput( Port port )
	{
		return this.AddIo( port, this.m_inputs );
	}

	public bool AddOutput( Port port )
	{
		return this.AddIo( port, this.m_outputs );
	}

	public bool RemoveInput( string name )
	{
		return this.RemoveIo( name, this.m_inputs );
	}

	public bool RemoveOutput( string name )
	{
		return this.RemoveIo( name, this.m_outputs );
	}

	public void ClearInputs()
	{
		this.m_inputs.Clear();
	}

	public void ClearOutputs()
	{
		this.m_outputs.Clear();
	}

	public void Clear()
	{
		this.ClearInputs();
		this.ClearOutputs();
	}

	protected bool AddIo( Port port, Dictionary<string, Port> ports )
	{
		if( ports.ContainsKey( port.name ) )
		{
			ports[port.name] = port;
			return false;
		}

		ports.Add( port.name, port );
		return true;
	}

	protected bool RemoveIo( string name, Dictionary<string, Port> ports )
	{
		return ports.Remove( name );
	}
*/
} // end class Node

