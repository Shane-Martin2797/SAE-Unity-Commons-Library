using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolingTypeI : MonoBehaviour
{

	public PooledObjectProperties pooledObject;
	public int poolSize;
	public List<GameObject> inactiveObjects;

	[System.Serializable]
	public class PooledObjectProperties
	{
		//Needs hold all components from the object
		//Needs to have a method thing (Like Unity UI) that calls when pooling is completed and when pooling is started
		//Or All Objects need to derrive from Pooled object.
		
		public GameObject pooledObjectPrefab;
		public List<Component> componentsOnObject = new List<Component> ();
		public PoolingTypeI originPool;
		//public methodtocall onenterpool;
		//public methodtocall onexitpool;
	}
	void PopulatePool ()
	{
		for (int i = 0; i < poolSize; i++)
		{
			BuildPool ();
		}
	}


	GameObject BuildPool ()
	{
		GameObject newObject = Instantiate (pooledObject.pooledObjectPrefab) as GameObject;
		newObject.SetActive (false);
		newObject.transform.SetParent (gameObject.transform);
		inactiveObjects.Add (newObject);
		return newObject;
	}
	
	public void ReturnObject (GameObject pooledObj)
	{
		pooledObj.gameObject.SetActive (false);
		pooledObj.transform.SetParent (gameObject.transform);
		//pooledObject.callonenterpoolmethod();
		inactiveObjects.Add (pooledObj);
	}
	
	public GameObject GetPooledObject ()
	{
		GameObject poolObject;
		if (inactiveObjects.Count == 0)
		{
			Debug.LogWarning (pooledObject.pooledObjectPrefab.name + " Pool too small");
			poolObject = BuildPool ();
		}
		else
		{
			poolObject = inactiveObjects [0];
		}
		poolObject.gameObject.SetActive (true);
		poolObject.transform.SetParent (null);
		//pooledObject.callonexitpoolmethod();
		inactiveObjects.Remove (poolObject);
		return poolObject;
	}


	void Awake ()
	{
		//componentsOnObject = pooledObject.GetComponents ();
		inactiveObjects = new List<GameObject> ();
		PopulatePool ();
	}
	
}
