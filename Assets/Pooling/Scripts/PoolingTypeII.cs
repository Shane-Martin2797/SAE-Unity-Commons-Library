using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolingTypeII : MonoBehaviour
{
	
	public PooledObject pooledObject;
	public int poolSize;
	public List<PooledObject> inactiveObjects;
	
	void PopulatePool ()
	{
		for (int i = 0; i < poolSize; i++) {
			BuildPool ();
		}
	}
	
	
	PooledObject BuildPool ()
	{
		PooledObject newObject = PooledObject.Instantiate (pooledObject) as PooledObject;
		newObject.gameObject.SetActive (false);
		newObject.transform.SetParent (gameObject.transform);
		newObject.pool = this;
		inactiveObjects.Add (newObject);
		return newObject;
	}
	
	public void ReturnObject (PooledObject pooledObj)
	{
		pooledObj.gameObject.SetActive (false);
		pooledObj.transform.SetParent (gameObject.transform);
		pooledObj.OnEnterPool ();
		inactiveObjects.Add (pooledObj);
	}
	
	public PooledObject GetPooledObject ()
	{
		PooledObject poolObject;
		if (inactiveObjects.Count == 0) {
			Debug.LogWarning (pooledObject.name + " Pool too small");
			poolObject = BuildPool ();
		} else {
			poolObject = inactiveObjects [0];
		}
		poolObject.gameObject.SetActive (true);
		poolObject.transform.SetParent (null);
		pooledObject.OnExitPool ();
		inactiveObjects.Remove (poolObject);
		return poolObject;
	}
	
	
	void Awake ()
	{
		inactiveObjects = new List<PooledObject> ();
		PopulatePool ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
