﻿using UnityEngine;

/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// 
/// As a note, this is made as MonoBehaviour because we need Coroutines.
/// </summary>
public class Singleton<T> : MonoBehaviour
	where T : MonoBehaviour
{
	public static bool IsPersistent = false;
	
	private static T _instance;
	private static bool _applicationIsQuitting = false;    
	private static object _lock = new object ();
	
	public static T Instance
	{
		get
		{
			if (_applicationIsQuitting)
				return null;
			
			lock (_lock)
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType (typeof (T)) as T;
					
					if (FindObjectsOfType (typeof (T)).Length > 1)
						return _instance;
					
					if (_instance == null)
					{
						GameObject singleton = new GameObject ();
						_instance = singleton.AddComponent<T> ();
						singleton.name = "[SINGLETON] " + typeof (T).ToString ();
					}
				}
				
				if (IsPersistent)
					DontDestroyOnLoad (_instance.gameObject);
				
				return _instance;
			}
		}
	}
	
	/// <summary>
	/// When Unity quits, it destroys objects in a random order.
	/// In principle, a Singleton is only destroyed when application quits.
	/// If any script calls Instance after it have been destroyed, 
	///   it will create a buggy ghost object that will stay on the Editor scene
	///   even after stopping playing the Application. Really bad!
	/// So, this was made to be sure we're not creating that buggy ghost object.
	/// </summary>
	public void OnDestroy ()
	{
		_applicationIsQuitting = true;
	}
}