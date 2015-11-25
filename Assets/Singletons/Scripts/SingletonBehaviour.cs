using UnityEngine;
using System.Collections;

public class SingletonBehaviour<T> : MonoBehaviour 
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
