using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class GenerationSection : MonoBehaviour
{
	//This is the enum of entry points that you can choose from.
	public enum EntryPoints
	{
		//These all follow their axis. Therefore:
		//Up is Positive (+ve) Y
		//Down is Negative (-ve) Y
		//Left is Negative (-ve) X
		//Right is Positive (+ve) X
		//Forward is Positive (+ve) Z
		//Backward is Negative (-ve) Z
		
		Up = 0,       //+ve Y
		Left = 1,     //-ve X
		Down = 2,     //-ve Y
		Right = 3,    //+ve X
		Forward = 4,  //+ve Z
		Backward = 5  //-ve Z
	}
	
	[System.Serializable]
	public class Door
	{
		public EntryPoints location;
		public Vector3 offset;
		public bool available = true;
		public GenerationSection attachedSection;
	}
	
	
	public Door[] AllEntries;
	public Bounds bounds;
		
	public List<GenerationSection> attachedSections;
	
	public int index = int.MaxValue;
	
	public GenerationController parent;
	
	void Awake ()
	{
		index = int.MaxValue;
		
	}

	void Start ()
	{
		SetupObject ();
		FindAllTouching ();
	}
	
	void SetupObject ()
	{
		//Firstly we need to find the bounds of this object.
		bounds = new Bounds (transform.position, Vector3.zero);
		FindBounds (transform, GetComponent<Renderer> ());
		
		//If this object has an index (Given by the parent) it is an object that was spawned to set default values.
		//Therefore this object needs to give the values to the list and then destroy itself.
		//(index will not be int.MaxValue) if this is the case. it will be (0-list.count).
		
		if (index != int.MaxValue)
		{
			parent.sectionPrefabs [index].section.bounds = bounds;
			parent.sectionPrefabs [index].section.bounds.size = bounds.size;
			parent.sectionPrefabs [index].section.bounds.extents = bounds.extents;
			Destroy (this.gameObject);
		}
	}
	
	// TODO:
	// Add a thing that, when objects are touching, checks if there is any doorway there,
	// and if there is then it makes that doorway unavailable and sets that this section is the next.
	// It should also do this vice-versa for the section that it is touching.
	void FindAllTouching ()
	{
		for (int i = 0; i < parent.procSections.Count; ++i)
		{
			if (Operations.CheckTouching (this, parent.procSections [i]))
			{
				AddTouching (parent.procSections [i]);
			}
		}
	}
	
	void AddTouching (GenerationSection section)
	{
		section.attachedSections.Add (this);
		this.attachedSections.Add (section);
	}
	
	void FindBounds (Transform t, Renderer r)
	{
	
		if (t == null)
		{
			return;
		}
		
		bounds.Encapsulate (t.position);
		
		for (int i = 0; i < t.childCount; ++i)
		{
			FindBounds (t.GetChild (i).transform, t.GetChild (i).GetComponent<Renderer> ());
		}
		
		if (r == null)
		{
			return;
		}
		
		bounds.Encapsulate (r.bounds);
	}
}
