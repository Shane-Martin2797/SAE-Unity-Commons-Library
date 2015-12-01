using UnityEngine;
using System.Collections;

public abstract class PooledObject : MonoBehaviour
{
	[HideInInspector]
	public PoolingTypeII
		pool;
		
	[HideInInspector]
	public float
		lifetime;
	public float defaultLifetime = 3;
	

	public virtual void OnEnterPool ()
	{
		lifetime = defaultLifetime;
	}
	
	public virtual void OnExitPool ()
	{
		lifetime = defaultLifetime;
	}
	
}
