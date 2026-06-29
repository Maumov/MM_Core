using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
	EventManager<bool, bool>.TriggerEvent(GunDetection.BaseMessage.GunNotFound.ToString (), true, true);
	EventManager<bool, bool>.StartListening (GunDetection.BaseMessage.GunNotFound.ToString (), GunFatalError);
	EventManager<bool, bool>.StopListening(GunDetection.BaseMessage.GunNotFound.ToString (), GunFatalError);

	// Creer un message unique, le nom de la classe c'est le nom du message
	public class ScreenHitMsg : EventMessagID<ScreenHitMsg> { }
*/
namespace MM
{
	public class EventMessageID<T> where T : class
	{
		public static string ID = typeof( T ).Name;
	}

	public class BaseInternalEvent
	{
		protected bool isTargetValid( object i_Target )
		{
			if ( i_Target == null )
				return false;

			return true;
		}
	}

	public class InternalEvent : BaseInternalEvent
	{
		Dictionary<int, LinkedListNode<UnityAction>> m_HashCode = new Dictionary<int, LinkedListNode<UnityAction>>();
		LinkedList<UnityAction> m_EventList = new LinkedList<UnityAction>();
		LinkedList<UnityAction> m_RecycleList = new LinkedList<UnityAction>( new UnityAction[ 200 ] );
		List<UnityAction> m_ToRemove = new List<UnityAction>();
		List<UnityAction> m_ToAdd = new List<UnityAction>();
		private bool m_Freeze = false;

		public int NbListener
		{
			get
			{
				return m_EventList.Count - m_ToRemove.Count + m_ToAdd.Count;
			}
		}
		public string GetInfo()
		{
			string s = string.Empty;

			foreach ( var v in m_EventList )
			{
				if ( !string.IsNullOrEmpty( s ) )
					s += " | ";

				System.Object o = v.Target;
				s += "Object: " + ( o == null ? "[NULL]" : o is UnityEngine.Object ? ( ( UnityEngine.Object ) o ).name : "Not a GameObject" );

				s += " Type: " + v.Target.GetType() + " Method: " + v.Method;
			}

			return s;
		}


		public void Purge()
		{
			m_ToRemove.Clear();
			m_Freeze = true;
			foreach ( var n in m_EventList )
			{
				if ( !isTargetValid( n.Target ) )
				{
					m_ToRemove.Add( n );
					continue;
				}
			}

			m_Freeze = false;
			foreach ( var n in m_ToRemove )
				RemoveListener( n );

			m_ToRemove.Clear();
			m_RecycleList.Clear();
		}

		public void AddListener( UnityAction i_Delegate )
		{
			if ( m_Freeze )
			{
				m_ToAdd.Add( i_Delegate );
				return;
			}

			int hash = i_Delegate.GetHashCode();
			if ( !m_HashCode.ContainsKey( hash ) )
			{
				LinkedListNode<UnityAction> n;
				if ( m_RecycleList.Count != 0 )
				{
					n = m_RecycleList.First;
					m_RecycleList.RemoveFirst();
					n.Value = i_Delegate;
				}
				else
				{
					n = new LinkedListNode<UnityAction>( i_Delegate );
				}

				m_EventList.AddFirst( n );
				m_HashCode.Add( hash, n );
			}
		}

		public void RemoveListener( UnityAction i_Delegate )
		{
			if ( m_Freeze )
			{
				m_ToRemove.Add( i_Delegate );
				return;
			}

			int hash = i_Delegate.GetHashCode();

			LinkedListNode<UnityAction> n;

			if ( m_HashCode.TryGetValue( hash, out n ) )
			{
				m_EventList.Remove( n );
				n.Value = null;
				m_RecycleList.AddFirst( n );
				m_HashCode.Remove( hash );
			}
		}

		public void Invoke()
		{
			m_ToRemove.Clear();
			m_ToAdd.Clear();
			m_Freeze = true;
			for ( var node = m_EventList.First ; node != null ; node = node.Next )
			{
				var n = node.Value;

				if ( m_ToRemove.Contains( n ) )
					continue;

				try
				{
					n.Invoke();
				}
				catch ( System.Exception e )
				{
					Debug.Log( "ERROR: EventManager Fail to invoke -> " + n.Target.GetType().ToString() + " " + n.Method.ToString() + " " + e.Message + "\n" + e.StackTrace );
				}
			}

			m_Freeze = false;
			foreach ( var n in m_ToRemove )
				RemoveListener( n );

			foreach ( var n in m_ToAdd )
				AddListener( n );

			m_ToRemove.Clear();
			m_ToAdd.Clear();
		}
	}

	public class InternalEvent<T> : BaseInternalEvent
	{

		Dictionary<int, LinkedListNode<UnityAction<T>>> m_HashCode = new Dictionary<int, LinkedListNode<UnityAction<T>>>();
		LinkedList<UnityAction<T>> m_EventList = new LinkedList<UnityAction<T>>();
		LinkedList<UnityAction<T>> m_RecycleList = new LinkedList<UnityAction<T>>( new UnityAction<T>[ 200 ] );
		List<UnityAction<T>> m_ToRemove = new List<UnityAction<T>>();
		List<UnityAction<T>> m_ToAdd = new List<UnityAction<T>>();
		private bool m_Freeze = false;

		public int NbListener
		{
			get
			{
				return m_EventList.Count - m_ToRemove.Count + m_ToAdd.Count;
			}
		}

		public string GetInfo()
		{
			string s = string.Empty;
			foreach ( var v in m_EventList )
			{
				if ( !string.IsNullOrEmpty( s ) )
					s += " | ";

				UnityEngine.Object obj = v.Target as UnityEngine.Object;
				s += "Object: " + ( obj == null ? "[NULL]" : obj.name );

				s += " Type: " + v.Target.GetType() + " Method: " + v.Method;
			}

			return s;
		}

		public void Purge()
		{
			m_ToRemove.Clear();
			m_Freeze = true;
			foreach ( var n in m_EventList )
			{

				if ( !isTargetValid( n.Target ) )
				{
					m_ToRemove.Add( n );
					continue;
				}
			}

			m_Freeze = false;
			foreach ( var n in m_ToRemove )
				RemoveListener( n );

			m_ToRemove.Clear();
			m_RecycleList.Clear();
		}

		public void AddListener( UnityAction<T> i_Delegate )
		{
			if ( m_Freeze )
			{
				m_ToAdd.Add( i_Delegate );
				return;
			}

			int hash = i_Delegate.GetHashCode();
			if ( !m_HashCode.ContainsKey( hash ) )
			{
				LinkedListNode<UnityAction<T>> n;
				if ( m_RecycleList.Count != 0 )
				{
					n = m_RecycleList.First;
					m_RecycleList.RemoveFirst();
					n.Value = i_Delegate;
				}
				else
				{
					n = new LinkedListNode<UnityAction<T>>( i_Delegate );
				}

				m_EventList.AddFirst( n );
				m_HashCode.Add( hash, n );
			}
		}

		public void RemoveListener( UnityAction<T> i_Delegate )
		{
			if ( m_Freeze )
			{
				m_ToRemove.Add( i_Delegate );
				return;
			}
			int hash = i_Delegate.GetHashCode();

			LinkedListNode<UnityAction<T>> n;

			if ( m_HashCode.TryGetValue( hash, out n ) )
			{
				m_EventList.Remove( n );
				n.Value = null;
				m_RecycleList.AddFirst( n );
				m_HashCode.Remove( hash );
			}
		}

		public void Invoke( T i_0 )
		{
			m_ToRemove.Clear();
			m_ToAdd.Clear();
			m_Freeze = true;
			for ( var node = m_EventList.First ; node != null ; node = node.Next )
			{
				var n = node.Value;

				if ( m_ToRemove.Contains( n ) )
					continue;

				try
				{
					n.Invoke( i_0 );
				}
				catch ( System.Exception e )
				{
					Debug.Log( "ERROR: EventManager Fail to invoke -> " + n.Target.GetType().ToString() + " " + n.Method.ToString() + " " + e.Message + "\n" + e.StackTrace );
				}
			}

			m_Freeze = false;
			foreach ( var n in m_ToRemove )
				RemoveListener( n );

			foreach ( var n in m_ToAdd )
				AddListener( n );

			m_ToRemove.Clear();
			m_ToAdd.Clear();
		}
	}

	public class InternalEvent<T_0, T_1> : BaseInternalEvent
	{

		Dictionary<int, LinkedListNode<UnityAction<T_0, T_1>>> m_HashCode = new Dictionary<int, LinkedListNode<UnityAction<T_0, T_1>>>();
		LinkedList<UnityAction<T_0, T_1>> m_EventList = new LinkedList<UnityAction<T_0, T_1>>();
		LinkedList<UnityAction<T_0, T_1>> m_RecycleList = new LinkedList<UnityAction<T_0, T_1>>( new UnityAction<T_0, T_1>[ 200 ] );
		List<UnityAction<T_0, T_1>> m_ToRemove = new List<UnityAction<T_0, T_1>>();
		List<UnityAction<T_0, T_1>> m_ToAdd = new List<UnityAction<T_0, T_1>>();
		private bool m_Freeze = false;

		public int NbListener
		{
			get
			{
				return m_EventList.Count - m_ToRemove.Count + m_ToAdd.Count;
			}
		}

		public string GetInfo()
		{
			string s = String.Empty;

			foreach ( var v in m_EventList )
			{
				if ( !string.IsNullOrEmpty( s ) )
					s += " | ";

				UnityEngine.Object obj = v.Target as UnityEngine.Object;
				s += "Object: " + ( obj == null ? "[NULL]" : obj.name );

				s += " Type: " + v.Target.GetType() + " Method: " + v.Method;
			}

			return s;
		}

		public void Purge()
		{
			m_ToRemove.Clear();
			m_Freeze = true;
			foreach ( var n in m_EventList )
			{

				if ( !isTargetValid( n.Target ) )
				{
					m_ToRemove.Add( n );
					continue;
				}
			}

			m_Freeze = false;
			foreach ( var n in m_ToRemove )
				RemoveListener( n );

			m_ToRemove.Clear();
			m_RecycleList.Clear();
		}

		public void AddListener( UnityAction<T_0, T_1> i_Delegate )
		{
			if ( m_Freeze )
			{
				m_ToAdd.Add( i_Delegate );
				return;
			}

			int hash = i_Delegate.GetHashCode();
			if ( !m_HashCode.ContainsKey( hash ) )
			{
				LinkedListNode<UnityAction<T_0, T_1>> n;

				if ( m_RecycleList.Count != 0 )
				{
					n = m_RecycleList.First;
					m_RecycleList.RemoveFirst();
					n.Value = i_Delegate;
				}
				else
				{
					n = new LinkedListNode<UnityAction<T_0, T_1>>( i_Delegate );
				}

				m_EventList.AddFirst( n );
				m_HashCode.Add( hash, n );
			}
		}

		public void RemoveListener( UnityAction<T_0, T_1> i_Delegate )
		{
			if ( m_Freeze )
			{
				m_ToRemove.Add( i_Delegate );
				return;
			}

			int hash = i_Delegate.GetHashCode();

			LinkedListNode<UnityAction<T_0, T_1>> n;

			if ( m_HashCode.TryGetValue( hash, out n ) )
			{
				m_EventList.Remove( n );
				n.Value = null;
				m_RecycleList.AddFirst( n );
				m_HashCode.Remove( hash );
			}
		}

		public void Invoke( T_0 i_0, T_1 i_1 )
		{
			m_ToRemove.Clear();
			m_ToAdd.Clear();
			m_Freeze = true;
			for ( var node = m_EventList.First ; node != null ; node = node.Next )
			{
				var n = node.Value;

				if ( m_ToRemove.Contains( n ) )
					continue;

				try
				{
					n.Invoke( i_0, i_1 );
				}
				catch ( System.Exception e )
				{
					Debug.Log( "ERROR: EventManager Fail to invoke -> " + n.Target.GetType().ToString() + " " + n.Method.ToString() + " " + e.Message + "\n" + e.StackTrace );
				}
			}

			m_Freeze = false;
			foreach ( var n in m_ToRemove )
				RemoveListener( n );

			foreach ( var n in m_ToAdd )
				AddListener( n );

			m_ToRemove.Clear();
			m_ToAdd.Clear();
		}
	}

	public class InternalEvent<T_0, T_1, T_2> : BaseInternalEvent
	{
		Dictionary<int, LinkedListNode<UnityAction<T_0, T_1, T_2>>> m_HashCode = new Dictionary<int, LinkedListNode<UnityAction<T_0, T_1, T_2>>>();
		LinkedList<UnityAction<T_0, T_1, T_2>> m_EventList = new LinkedList<UnityAction<T_0, T_1, T_2>>();
		LinkedList<UnityAction<T_0, T_1, T_2>> m_RecycleList = new LinkedList<UnityAction<T_0, T_1, T_2>>( new UnityAction<T_0, T_1, T_2>[ 200 ] );
		List<UnityAction<T_0, T_1, T_2>> m_ToRemove = new List<UnityAction<T_0, T_1, T_2>>();
		List<UnityAction<T_0, T_1, T_2>> m_ToAdd = new List<UnityAction<T_0, T_1, T_2>>();
		private bool m_Freeze = false;

		public int NbListener
		{
			get
			{
				return m_EventList.Count - m_ToRemove.Count + m_ToAdd.Count;
			}
		}

		public string GetInfo()
		{
			string s = string.Empty;

			foreach ( var v in m_EventList )
			{
				if ( !string.IsNullOrEmpty( s ) )
					s += " | ";

				UnityEngine.Object obj = v.Target as UnityEngine.Object;
				s += "Object: " + ( obj == null ? "[NULL]" : obj.name );

				s += " Type: " + v.Target.GetType() + " Method: " + v.Method;
			}

			return s;
		}

		public void Purge()
		{
			m_ToRemove.Clear();
			m_Freeze = true;
			foreach ( var n in m_EventList )
			{

				if ( !isTargetValid( n.Target ) )
				{
					m_ToRemove.Add( n );
					continue;
				}
			}

			m_Freeze = false;
			foreach ( var n in m_ToRemove )
				RemoveListener( n );

			m_ToRemove.Clear();
			m_RecycleList.Clear();
		}

		public void AddListener( UnityAction<T_0, T_1, T_2> i_Delegate )
		{
			if ( m_Freeze )
			{
				m_ToAdd.Add( i_Delegate );
				return;
			}

			int hash = i_Delegate.GetHashCode();
			if ( !m_HashCode.ContainsKey( hash ) )
			{
				LinkedListNode<UnityAction<T_0, T_1, T_2>> n;
				if ( m_RecycleList.Count != 0 )
				{
					n = m_RecycleList.First;
					m_RecycleList.RemoveFirst();
					n.Value = i_Delegate;
				}
				else
				{
					n = new LinkedListNode<UnityAction<T_0, T_1, T_2>>( i_Delegate );
				}

				m_EventList.AddFirst( n );
				m_HashCode.Add( hash, n );
			}
		}

		public void RemoveListener( UnityAction<T_0, T_1, T_2> i_Delegate )
		{
			if ( m_Freeze )
			{
				m_ToRemove.Add( i_Delegate );
				return;
			}

			int hash = i_Delegate.GetHashCode();

			LinkedListNode<UnityAction<T_0, T_1, T_2>> n;

			if ( m_HashCode.TryGetValue( hash, out n ) )
			{
				m_EventList.Remove( n );
				n.Value = null;
				m_RecycleList.AddFirst( n );
				m_HashCode.Remove( hash );
			}
		}

		public void Invoke( T_0 i_0, T_1 i_1, T_2 i_2 )
		{
			m_ToRemove.Clear();
			m_ToAdd.Clear();
			m_Freeze = true;
			for ( var node = m_EventList.First ; node != null ; node = node.Next )
			{
				var n = node.Value;

				if ( m_ToRemove.Contains( n ) )
					continue;

				try
				{
					n.Invoke( i_0, i_1, i_2 );
				}
				catch ( System.Exception e )
				{
					Debug.Log( "ERROR: EventManager Fail to invoke -> " + n.Target.GetType().ToString() + " " + n.Method.ToString() + " " + e.Message + "\n" + e.StackTrace );
				}
			}

			m_Freeze = false;
			foreach ( var n in m_ToRemove )
				RemoveListener( n );

			foreach ( var n in m_ToAdd )
				AddListener( n );

			m_ToRemove.Clear();
			m_ToAdd.Clear();
		}
	}

	public class InternalEvent<T_0, T_1, T_2, T3> : BaseInternalEvent
	{
		Dictionary<int, LinkedListNode<UnityAction<T_0, T_1, T_2, T3>>> m_HashCode = new Dictionary<int, LinkedListNode<UnityAction<T_0, T_1, T_2, T3>>>();
		LinkedList<UnityAction<T_0, T_1, T_2, T3>> m_EventList = new LinkedList<UnityAction<T_0, T_1, T_2, T3>>();
		LinkedList<UnityAction<T_0, T_1, T_2, T3>> m_RecycleList = new LinkedList<UnityAction<T_0, T_1, T_2, T3>>( new UnityAction<T_0, T_1, T_2, T3>[ 200 ] );
		List<UnityAction<T_0, T_1, T_2, T3>> m_ToRemove = new List<UnityAction<T_0, T_1, T_2, T3>>();
		List<UnityAction<T_0, T_1, T_2, T3>> m_ToAdd = new List<UnityAction<T_0, T_1, T_2, T3>>();
		private bool m_Freeze = false;

		public int NbListener
		{
			get
			{
				return m_EventList.Count - m_ToRemove.Count + m_ToAdd.Count;
			}
		}
		public string GetInfo()
		{
			string s = string.Empty;

			foreach ( var v in m_EventList )
			{
				if ( !string.IsNullOrEmpty( s ) )
					s += " | ";

				UnityEngine.Object obj = v.Target as UnityEngine.Object;
				s += "Object: " + ( obj == null ? "[NULL]" : obj.name );

				s += " Type: " + v.Target.GetType() + " Method: " + v.Method;
			}

			return s;
		}

		public void Purge()
		{
			m_ToRemove.Clear();
			m_Freeze = true;
			foreach ( var n in m_EventList )
			{
				if ( !isTargetValid( n.Target ) )
				{
					m_ToRemove.Add( n );
					continue;
				}
			}

			m_Freeze = false;
			foreach ( var n in m_ToRemove )
				RemoveListener( n );

			m_ToRemove.Clear();
			m_RecycleList.Clear();
		}

		public void AddListener( UnityAction<T_0, T_1, T_2, T3> i_Delegate )
		{
			if ( m_Freeze )
			{
				m_ToAdd.Add( i_Delegate );
				return;
			}

			int hash = i_Delegate.GetHashCode();
			if ( !m_HashCode.ContainsKey( hash ) )
			{
				LinkedListNode<UnityAction<T_0, T_1, T_2, T3>> n;

				if ( m_RecycleList.Count != 0 )
				{
					n = m_RecycleList.First;
					m_RecycleList.RemoveFirst();
					n.Value = i_Delegate;
				}
				else
				{
					n = new LinkedListNode<UnityAction<T_0, T_1, T_2, T3>>( i_Delegate );
				}

				m_EventList.AddFirst( n );
				m_HashCode.Add( hash, n );
			}
		}

		public void RemoveListener( UnityAction<T_0, T_1, T_2, T3> i_Delegate )
		{
			if ( m_Freeze )
			{
				m_ToRemove.Add( i_Delegate );
				return;
			}

			int hash = i_Delegate.GetHashCode();

			LinkedListNode<UnityAction<T_0, T_1, T_2, T3>> n;

			if ( m_HashCode.TryGetValue( hash, out n ) )
			{
				m_EventList.Remove( n );
				n.Value = null;
				m_RecycleList.AddFirst( n );
				m_HashCode.Remove( hash );
			}
		}

		public void Invoke( T_0 i_0, T_1 i_1, T_2 i_2, T3 i_3 )
		{
			m_ToRemove.Clear();
			m_ToAdd.Clear();
			m_Freeze = true;
			for ( var node = m_EventList.First ; node != null ; node = node.Next )
			{
				var n = node.Value;

				if ( m_ToRemove.Contains( n ) )
					continue;

				try
				{
					n.Invoke( i_0, i_1, i_2, i_3 );
				}
				catch ( System.Exception e )
				{
					Debug.Log( "ERROR: EventManager Fail to invoke -> " + n.Target.GetType().ToString() + " " + n.Method.ToString() + " " + e.Message + "\n" + e.StackTrace );
				}
			}

			m_Freeze = false;
			foreach ( var n in m_ToRemove )
				RemoveListener( n );

			foreach ( var n in m_ToAdd )
				AddListener( n );

			m_ToRemove.Clear();
			m_ToAdd.Clear();
		}
	}

	public class EventManagerInfos
	{
		public delegate string GetInfo();
		public delegate void Purge();
		private static List<GetInfo> m_InfoCallback = new List<GetInfo>();
		private static List<Purge> m_Purge = new List<Purge>();

		public EventManagerInfos( GetInfo i_Action, Purge i_Purge )
		{
			m_InfoCallback.Add( i_Action );
			m_Purge.Add( i_Purge );
		}

		public static string GetInfos()
		{
			string s = string.Empty;
			foreach ( var GetInf in m_InfoCallback )
				s += GetInf();

			return s;
		}

		public static void PurgeAction()
		{
			foreach ( var p in m_Purge )
				p();
		}
	}

	public static class EventManager
	{
		private static Dictionary<string, InternalEvent> eventDictionary = new Dictionary<string, InternalEvent>();
		private static List<string> m_ToRemove = new List<string>();
		private static EventManagerInfos Info = new EventManagerInfos( GetInfo, Purge );

		private static string GetInfo()
		{
			string s = string.Empty;
			foreach ( var keyvalue in eventDictionary )
			{
				s += "Message " + keyvalue.Key + "\n" + keyvalue.Value.GetInfo() + "\n";
			}
			return s;
		}

		private static void Purge()
		{
			m_ToRemove.Clear();
			foreach ( var keyvalue in eventDictionary )
			{
				keyvalue.Value.Purge();
				if ( keyvalue.Value.NbListener == 0 )
					m_ToRemove.Add( keyvalue.Key );
			}

			foreach ( var key in m_ToRemove )
				eventDictionary.Remove( key );
		}

		public static void StartListening( string eventName, UnityAction listener )
		{
			InternalEvent thisEvent = null;
			if ( eventDictionary.TryGetValue( eventName, out thisEvent ) )
			{
				thisEvent.AddListener( listener );
			}
			else
			{
				thisEvent = new InternalEvent();
				thisEvent.AddListener( listener );
				eventDictionary.Add( eventName, thisEvent );
			}
		}

		public static void StopListening( string eventName, UnityAction listener )
		{
			InternalEvent thisEvent = null;
			if ( eventDictionary.TryGetValue( eventName, out thisEvent ) )
			{
				thisEvent.RemoveListener( listener );
				if ( thisEvent.NbListener == 0 )
					eventDictionary.Remove( eventName );
			}
		}

		public static void TriggerEvent( string eventName )
		{
			InternalEvent thisEvent = null;
			if ( eventDictionary.TryGetValue( eventName, out thisEvent ) )
			{
				thisEvent.Invoke();
			}
		}

		public static void RemoveEvent( string eventName )
		{
			if ( eventDictionary.ContainsKey( eventName ) )
			{
				eventDictionary.Remove( eventName );
			}
		}
	}

	public static class EventManager<T>
	{
		private static Dictionary<string, InternalEvent<T>> eventDictionary = new Dictionary<string, InternalEvent<T>>();
		private static List<string> m_ToRemove = new List<string>();
		private static EventManagerInfos Info = new EventManagerInfos( GetInfo, Purge );

		private static string GetInfo()
		{
			string s = string.Empty;
			foreach ( var keyvalue in eventDictionary )
				s += "Message " + keyvalue.Key + "\n" + keyvalue.Value.GetInfo() + "\n";

			return s;
		}

		private static void Purge()
		{
			m_ToRemove.Clear();
			foreach ( var keyvalue in eventDictionary )
			{
				keyvalue.Value.Purge();
				if ( keyvalue.Value.NbListener == 0 )
					m_ToRemove.Add( keyvalue.Key );
			}

			foreach ( var key in m_ToRemove )
			{
				eventDictionary.Remove( key );
			}
			m_ToRemove.Clear();
		}

		public static void StartListening( string eventName, UnityAction<T> listener )
		{
			InternalEvent<T> thisEvent = null;
			if ( eventDictionary.TryGetValue( eventName, out thisEvent ) )
			{
				thisEvent.AddListener( listener );
			}
			else
			{
				thisEvent = new InternalEvent<T>();
				thisEvent.AddListener( listener );
				eventDictionary.Add( eventName, thisEvent );
			}
		}

		public static void StopListening( string eventName, UnityAction<T> listener )
		{
			InternalEvent<T> thisEvent = null;
			if ( eventDictionary.TryGetValue( eventName, out thisEvent ) )
			{
				thisEvent.RemoveListener( listener );
				if ( thisEvent.NbListener == 0 )
					eventDictionary.Remove( eventName );
			}
		}

		public static void TriggerEvent( string eventName, T i_Arg )
		{
			InternalEvent<T> thisEvent = null;
			if ( eventDictionary.TryGetValue( eventName, out thisEvent ) )
			{
				thisEvent.Invoke( i_Arg );
			}
		}

		public static void RemoveEvent( string eventName )
		{
			if ( eventDictionary.ContainsKey( eventName ) )
				eventDictionary.Remove( eventName );
		}
	}

	public static class EventManager<T1, T2>
	{
		private static Dictionary<string, InternalEvent<T1, T2>> eventDictionary = new Dictionary<string, InternalEvent<T1, T2>>();
		private static List<string> m_ToRemove = new List<string>();
		private static EventManagerInfos Info = new EventManagerInfos( GetInfo, Purge );

		private static void Purge()
		{
			m_ToRemove.Clear();
			foreach ( var keyvalue in eventDictionary )
			{
				keyvalue.Value.Purge();
				if ( keyvalue.Value.NbListener == 0 )
					m_ToRemove.Add( keyvalue.Key );
			}

			foreach ( var key in m_ToRemove )
			{
				eventDictionary.Remove( key );
			}
			m_ToRemove.Clear();
		}

		private static string GetInfo()
		{
			string s = string.Empty;
			foreach ( var keyvalue in eventDictionary )
			{
				s += "Message " + keyvalue.Key + "\n" + keyvalue.Value.GetInfo() + "\n";
			}
			return s;

		}

		public static void StartListening( string eventName, UnityAction<T1, T2> listener )
		{
			InternalEvent<T1, T2> thisEvent = null;
			if ( eventDictionary.TryGetValue( eventName, out thisEvent ) )
			{
				thisEvent.AddListener( listener );
			}
			else
			{
				thisEvent = new InternalEvent<T1, T2>();
				thisEvent.AddListener( listener );
				eventDictionary.Add( eventName, thisEvent );
			}
		}

		public static void StopListening( string eventName, UnityAction<T1, T2> listener )
		{
			InternalEvent<T1, T2> thisEvent = null;
			if ( eventDictionary.TryGetValue( eventName, out thisEvent ) )
			{
				thisEvent.RemoveListener( listener );
				if ( thisEvent.NbListener == 0 )
					eventDictionary.Remove( eventName );
			}
		}

		public static void TriggerEvent( string eventName, T1 i_Arg1, T2 i_Arg2 )
		{
			InternalEvent<T1, T2> thisEvent = null;
			if ( eventDictionary.TryGetValue( eventName, out thisEvent ) )
			{
				thisEvent.Invoke( i_Arg1, i_Arg2 );
			}
		}

		public static void RemoveEvent( string eventName )
		{
			if ( eventDictionary.ContainsKey( eventName ) )
				eventDictionary.Remove( eventName );
		}
	}

	public static class EventManager<T1, T2, T3>
	{
		private static Dictionary<string, InternalEvent<T1, T2, T3>> eventDictionary = new Dictionary<string, InternalEvent<T1, T2, T3>>();
		private static List<string> m_ToRemove = new List<string>();
		private static EventManagerInfos Info = new EventManagerInfos( GetInfo, Purge );

		private static string GetInfo()
		{
			string s = string.Empty;
			foreach ( var keyvalue in eventDictionary )
				s += "Message " + keyvalue.Key + "\n" + keyvalue.Value.GetInfo() + "\n";

			return s;
		}

		private static void Purge()
		{
			m_ToRemove.Clear();
			foreach ( var keyvalue in eventDictionary )
			{
				keyvalue.Value.Purge();
				if ( keyvalue.Value.NbListener == 0 )
					m_ToRemove.Add( keyvalue.Key );
			}

			foreach ( var key in m_ToRemove )
			{
				eventDictionary.Remove( key );
			}
			m_ToRemove.Clear();
		}

		public static void StartListening( string eventName, UnityAction<T1, T2, T3> listener )
		{
			InternalEvent<T1, T2, T3> thisEvent = null;
			if ( eventDictionary.TryGetValue( eventName, out thisEvent ) )
			{
				thisEvent.AddListener( listener );
			}
			else
			{
				thisEvent = new InternalEvent<T1, T2, T3>();
				thisEvent.AddListener( listener );
				eventDictionary.Add( eventName, thisEvent );
			}
		}

		public static void StopListening( string eventName, UnityAction<T1, T2, T3> listener )
		{
			InternalEvent<T1, T2, T3> thisEvent = null;
			if ( eventDictionary.TryGetValue( eventName, out thisEvent ) )
			{
				thisEvent.RemoveListener( listener );
				if ( thisEvent.NbListener == 0 )
					eventDictionary.Remove( eventName );
			}
		}

		public static void TriggerEvent( string eventName, T1 i_Arg1, T2 i_Arg2, T3 i_Arg3 )
		{
			InternalEvent<T1, T2, T3> thisEvent = null;
			if ( eventDictionary.TryGetValue( eventName, out thisEvent ) )
			{
				thisEvent.Invoke( i_Arg1, i_Arg2, i_Arg3 );
			}
		}

		public static void RemoveEvent( string eventName )
		{
			if ( eventDictionary.ContainsKey( eventName ) )
				eventDictionary.Remove( eventName );
		}
	}

	public static class EventManager<T1, T2, T3, T4>
	{
		private static Dictionary<string, InternalEvent<T1, T2, T3, T4>> eventDictionary = new Dictionary<string, InternalEvent<T1, T2, T3, T4>>();
		private static List<string> m_ToRemove = new List<string>();
		private static EventManagerInfos Info = new EventManagerInfos( GetInfo, Purge );

		private static string GetInfo()
		{
			string s = string.Empty;
			foreach ( var keyvalue in eventDictionary )
				s += "Message " + keyvalue.Key + "\n" + keyvalue.Value.GetInfo() + "\n";

			return s;
		}

		private static void Purge()
		{
			m_ToRemove.Clear();
			foreach ( var keyvalue in eventDictionary )
			{
				keyvalue.Value.Purge();
				if ( keyvalue.Value.NbListener == 0 )
					m_ToRemove.Add( keyvalue.Key );
			}

			foreach ( var key in m_ToRemove )
			{
				eventDictionary.Remove( key );
			}
			m_ToRemove.Clear();
		}

		public static void StartListening( string eventName, UnityAction<T1, T2, T3, T4> listener )
		{
			InternalEvent<T1, T2, T3, T4> thisEvent = null;
			if ( eventDictionary.TryGetValue( eventName, out thisEvent ) )
			{
				thisEvent.AddListener( listener );
			}
			else
			{
				thisEvent = new InternalEvent<T1, T2, T3, T4>();
				thisEvent.AddListener( listener );
				eventDictionary.Add( eventName, thisEvent );
			}
		}

		public static void StopListening( string eventName, UnityAction<T1, T2, T3, T4> listener )
		{
			InternalEvent<T1, T2, T3, T4> thisEvent = null;
			if ( eventDictionary.TryGetValue( eventName, out thisEvent ) )
			{
				thisEvent.RemoveListener( listener );
				if ( thisEvent.NbListener == 0 )
					eventDictionary.Remove( eventName );
			}
		}

		public static void TriggerEvent( string eventName, T1 i_Arg1, T2 i_Arg2, T3 i_Arg3, T4 i_Arg4 )
		{
			InternalEvent<T1, T2, T3, T4> thisEvent = null;
			if ( eventDictionary.TryGetValue( eventName, out thisEvent ) )
			{
				thisEvent.Invoke( i_Arg1, i_Arg2, i_Arg3, i_Arg4 );
			}
		}

		public static void RemoveEvent( string eventName )
		{
			if ( eventDictionary.ContainsKey( eventName ) )
				eventDictionary.Remove( eventName );
		}
	}
}