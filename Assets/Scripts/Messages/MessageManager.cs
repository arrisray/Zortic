using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageManager : Singleton<MessageManager>
{
    // --------------------------------------------------------------------------
    // DELEGATES

    public delegate void MessageHandler(Message message);

    // --------------------------------------------------------------------------
    // DATA MEMBERS
	
    private Dictionary<System.Type, List<MessageHandler> > m_handlers = new Dictionary<System.Type, List<MessageHandler> >();

    // --------------------------------------------------------------------------
    // PROPERTIES

    // --------------------------------------------------------------------------
    // METHODS  

    public void RaiseEvent( Message message )
    {
        System.Type messageType = message.GetType();    
        if( !this.m_handlers.ContainsKey(messageType) )
        {
            // /// Debug.LogWarning( "No listeners registered for message (type=" + message.GetType() + ")." );
            return;
        }
        
        // /// Debug.Log( "Raising event: " + messageType + ", name=" + message );
        //! @hack Copying the list every time because other entities in other threads will modify this collection upon destruction!!!
        //! @todo Find a more robust method to make this operation (and class) thread-safe...
		//! @todo Can we just reverse iterate as a solution here...?
        List<MessageHandler> handlers = new List<MessageHandler>( this.m_handlers[messageType] );
        foreach( MessageHandler messageHandler in handlers )
        {
            messageHandler( message );
        }
    }

    public bool RegisterMessageHandler<T>( MessageHandler messageHandler )
    {
        System.Type messageType = typeof(T);
        if( !this.m_handlers.ContainsKey(messageType) )
        {
            this.m_handlers.Add( messageType, new List<MessageHandler>() );
        }

        List<MessageHandler> messageHandlers = this.m_handlers[messageType];
        lock( messageHandlers )
        {
            if( messageHandlers.Contains(messageHandler) )
            {
                return false;
            }

            // /// Debug.Log( "Registered for message: " + messageType );
            messageHandlers.Add( messageHandler );
        }
        return true;
    }

    public bool UnregisterMessageHandler<T>( MessageHandler messageHandler )
    {
        System.Type messageType = typeof(T);
        if( !this.m_handlers.ContainsKey(messageType) )
        {
            return false;
        }

        List<MessageHandler> messageHandlers = this.m_handlers[messageType];
        lock(messageHandlers)
        {
            if( !messageHandlers.Contains(messageHandler) )
            {
                return false;
            }

            messageHandlers.Remove( messageHandler );
        }
        return true;
    }
}
