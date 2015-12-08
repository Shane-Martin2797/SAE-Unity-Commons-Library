using UnityEngine;
using System.Collections;

public class ProcGenSection : MonoBehaviour
{

	//This is the size of this current Section. It is how far it will be delayed.
	public float generationOffset;
	
	//This is whether or not the section will add after a certain amount of distance or will be added to the list after a certain amount of sections.
	public bool delayedBySectionsNotDistance = true;
	
	//This is the distance before it adds this section to the list.
	public float distanceToAddition = 0;
	

	// Use this for initialization
	void Awake ()
	{
		if (generationOffset <= 0) {
			Debug.LogError ("The offset is " + generationOffset);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
