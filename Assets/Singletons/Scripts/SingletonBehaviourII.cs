/// <summary>
/// Singleton Behaviour Type 1 is better as you do not have to use OnSingletonAwake() or OnSingletonDestroy.
/// This code is still left in though as some may prefer to use it.
/// </summary>

using UnityEngine;
using System.Collections;

public class SingletonBehaviourII<T> : MonoBehaviour 
		where T : MonoBehaviour
{
		
	protected virtual void OnSingletonAwake ()
	{
			
	}
	protected virtual void OnSingletonDestroy ()
	{
			
	}
	public static T Instance { get; private set; }
	void Awake ()
	{
		if (Instance == null) {
			Instance = this as T;
		} else {
			Destroy (gameObject);
		}
		OnSingletonAwake ();
	}
		
	void OnDestroy ()
	{
		if (Instance == this) {
			Instance = null;
		}
		OnSingletonDestroy ();
	}
}
