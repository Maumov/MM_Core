using UnityEngine;

/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// </summary>

namespace MM
{
	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T _instance;
		protected static bool m_DontDestroyOnLoad = false;

		private static object _lock = new object();

		public static bool IsDestroyed()
		{
			return ( ( applicationIsQuitting == true ) && ( _instance == null ) );
		}

		public static bool Exists()
		{
			return ( ( _instance != null ) && !applicationIsQuitting );
		}

		public static T Instance
		{
			get
			{
				if ( applicationIsQuitting )
				{
					Debug.LogWarning( "[Singleton] Instance '" + typeof( T ) +
									  "' already destroyed on application quit." +
									  " Won't create again - returning null." );
					return null;
				}

				lock ( _lock )
				{
					if ( ( object ) _instance == null )
					{
						//Debug.Log( "Singleton created from Instance call()" );
						_instance = ( T ) FindFirstObjectByType( typeof( T ) );

#if UNITY_EDITOR
					if( FindObjectsByType(typeof(T), FindObjectsSortMode.None ).Length > 1 )
					{
						Debug.LogError( "[Singleton] Something went really wrong " +
						                " - there should never be more than 1 singleton!" +
						                " Reopenning the scene might fix it. (" + typeof(T).ToString() + ")" );
						return _instance;
					}
#endif

						if ( _instance == null )
						{
							GameObject singleton = new GameObject();
							_instance = singleton.AddComponent<T>();
							singleton.name = typeof( T ).ToString();

							if ( m_DontDestroyOnLoad )
								DontDestroyOnLoad( singleton );

							/*Debug.Log("[Singleton] An instance of " + typeof(T) + 
									  " is needed in the scene, so '" + singleton +
									  "' was created with DontDestroyOnLoad.");*/
						}
						/*else
						{
							Debug.Log("[Singleton] Using instance already created: " +
									  _instance.gameObject.name);
						}*/
					}

					return _instance;
				}
			}
		}

		private static bool applicationIsQuitting = false;
		private bool destroyingDuplicate = false;
		/// <summary>
		/// When Unity quits, it destroys objects in a random order.
		/// In principle, a Singleton is only destroyed when application quits.
		/// If any script calls Instance after it have been destroyed, 
		///   it will create a buggy ghost object that will stay on the Editor scene
		///   even after stopping playing the Application. Really bad!
		/// So, this was made to be sure we're not creating that buggy ghost object.
		/// </summary>
		/// 
		public virtual void OnDestroy()
		{
			if ( destroyingDuplicate == false )
			{
				applicationIsQuitting = true;

				if ( _instance == this )
				{
					_instance = null;
				}
			}
		}

		public virtual void Awake()
		{
			if ( _instance == null )
			{
				_instance = this as T;

				if ( m_DontDestroyOnLoad )
					DontDestroyOnLoad( gameObject );

				applicationIsQuitting = false;
			}
			else
			{
				Debug.LogError( "There is already a singleton created (" + typeof( T ).ToString() );
				destroyingDuplicate = true;
				GameObject.Destroy( gameObject );
			}
		}
	}
}