using System;
using System.Collections.Generic;
using UnityEngine;

namespace MM
{
    public class EventBus : MonoBehaviour
    {
        private static EventBus _instance;

        // Diccionario que almacena listas de callbacks basados en el tipo de evento
        private Dictionary<Type, List<Delegate>> _eventListeners = new Dictionary<Type, List<Delegate>>();

        // Propiedad estática para obtener el EventBus
        public static EventBus Instance
        {
            get
            {
                if ( _instance == null )
                {
                    var eventBusObject = new GameObject( "EventBus" );
                    _instance = eventBusObject.AddComponent<EventBus>();
                    DontDestroyOnLoad( eventBusObject );
                }
                return _instance;
            }
        }

        // Suscribirse a un evento de tipo T
        public void Subscribe<T>( Action<T> listener ) where T : class
        {
            var eventType = typeof( T );

            if ( !_eventListeners.ContainsKey( eventType ) )
            {
                _eventListeners[ eventType ] = new List<Delegate>();
            }
            _eventListeners[ eventType ].Add( listener );
        }

        // Desuscribirse de un evento de tipo T
        public void Unsubscribe<T>( Action<T> listener ) where T : class
        {
            var eventType = typeof( T );

            if ( _eventListeners.ContainsKey( eventType ) )
            {
                _eventListeners[ eventType ].Remove( listener );

                // Eliminar la entrada si no hay más suscriptores
                if ( _eventListeners[ eventType ].Count == 0 )
                {
                    _eventListeners.Remove( eventType );
                }
            }
        }

        // Publicar un evento de tipo T
        public void Publish<T>( T eventData ) where T : class
        {
            var eventType = typeof( T );

            if ( _eventListeners.ContainsKey( eventType ) )
            {
                foreach ( var listener in _eventListeners[ eventType ] )
                {
                    ( ( Action<T> ) listener )?.Invoke( eventData );
                }
            }
        }
    }


    // Clase base para todos los eventos
    public abstract class EventBase
    {
    }

    // Clase base para los escuchadores de eventos
    public abstract class EventListenerBase
    {
    }

    // Clase que contiene el callback del escuchador de eventos
    public class EventListener<T> : EventListenerBase where T : EventBase
    {
        public readonly Action<T> Callback;

        public EventListener( Action<T> callback )
        {
            Callback = callback;
        }

        public void OnEventPublished( T publishedEvent )
        {
            Callback.Invoke( publishedEvent );
        }
    }
}
