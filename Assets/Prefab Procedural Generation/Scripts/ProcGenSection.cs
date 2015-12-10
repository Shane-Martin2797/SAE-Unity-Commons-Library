using UnityEngine;
using System.Collections;

public class ProcGenSection : MonoBehaviour
{

	//This is the size of this current Section. It is how far it will be delayed.
	public float sizeOfSection;
	
	//This is whether or not the section will add after a certain amount of distance or will be added to the list after a certain amount of sections.
	public bool delayedBySectionsNotDistance = true;
	
	//This is the distance before it adds this section to the list.
	public float distanceUntilAdditionToList = 0;
	
	//This is the distance before it removes this section from the list.
	public float distanceUntilRemoveFromList;
	
	//
	private ProcGenController spawner;
	private float distanceSpawned;
	

	// Use this for initialization
	void Awake ()
	{
		InitialiseProcGenSection ();
	}
	
	void InitialiseProcGenSection ()
	{
		if (sizeOfSection <= 0) {
			Debug.LogError ("The offset is " + sizeOfSection 
				+ "\n" + " This section will not add to the list of sections to generate");
			//If it's offset is wrong, make it never get added to the list of things that can generate.
			distanceUntilAdditionToList = Mathf.Infinity;
		}
	}
	
	public void SetSpawnObject (ProcGenController spawn)
	{
		spawner = spawn;
		distanceSpawned = spawn.currentDistance;
	}
	
	void CleanUp ()
	{
		//if(!spawner.isPooled)
		{
			Destroy (this.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		//If the spawners distance that it was spawned at is 
		if (spawner != null) {
			if ((spawner.currentDistance - distanceSpawned) >= spawner.cleanupDistance) {
				CleanUp ();
			}
		}
	}
}
