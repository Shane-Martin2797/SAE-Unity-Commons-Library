using UnityEngine;
using System.Collections;
using System.Collections.Generic;



//TODO:
//Fix checks for when it returns IntReturnValue
//Fix it so that it calculates properly
//FIX OPERATION CHECKS


public class GenerationController : MonoBehaviour
{
	public const int MinNumberOfDoors = 5;
	public GenRatios[] sectionPrefabs;
	
	public float generationSoftEdge;
	public GameObject target;
	public bool hasTarget;
	
	public bool maximumRepeats;
	public int maxConsecutiveRepeats;
	
	private int previousIndex;
	private int currentIndex;
	private int repetitions;
	
	private int numberOfDoors;
	
	private int whileLoopBreakAmount = 10000;
	private int totalPrefabsSpawned;

	public List<GenerationSection> procSections;
	
	private Vector3 previousPosition;


	/// <summary>
	/// Code to do when encountering a place where nothing can spawn.
	/// For example if there are no pieces small enough to spawn something in a gap.
	/// This is the function you fill out. Whether you wish for a wall to spawn or whatever.
	/// </summary>
	void CodeToDoWhenEncounteringAPlaceWhereNothingCanSpawn (GenerationSection section, Vector3 positionOfDoor, GenerationSection.EntryPoints side)
	{
	
		Debug.Log ("You can change this Function/Method to change what happens when nothing can generate.");
	
		//An Example that spawns a wall.
		//Change the false to a true to get it to spawn a wall.
		if (false)
		{
			GameObject wall = GameObject.CreatePrimitive (PrimitiveType.Cube);
			wall.transform.position = positionOfDoor;
			wall.name = "Wall to Place When Nothing Can Spawn";
			Vector3 scale = Vector3.one;
			//This was a switch case, but since duplicates up/down, I changed it to an if.
			if (side == GenerationSection.EntryPoints.Up || side == GenerationSection.EntryPoints.Down)
			{
				scale.x = section.bounds.size.x;
				scale.z = section.bounds.size.z;
			}
			else if (side == GenerationSection.EntryPoints.Left || side == GenerationSection.EntryPoints.Right)
			{
				scale.y = section.bounds.size.y;
				scale.z = section.bounds.size.z;
			}
			else if (side == GenerationSection.EntryPoints.Forward || side == GenerationSection.EntryPoints.Backward)
			{
				scale.x = section.bounds.size.x;
				scale.y = section.bounds.size.y;
			}
			wall.transform.localScale = scale;
			wall.transform.SetParent (section.transform);
		}
		
	}

	void Awake ()
	{
		for (int i = 0; i < sectionPrefabs.Length; ++i)
		{
			GenerationSection section = Instantiate (sectionPrefabs [i].section) as GenerationSection;
			sectionPrefabs [i].section.bounds = section.bounds;
			section.index = i;
			section.parent = this;
		}
		
		sectionPrefabs = Operations.InitialiseRatios (sectionPrefabs);
		if (target != null)
		{
			hasTarget = true;
		}
		else
		{
			hasTarget = false;
		}
		
		procSections = new List<GenerationSection> ();
	}
	
	// Use this for initialization
	void Start ()
	{
		GenerateProceduralSection (null);
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	
		if (hasTarget)
		{
			if (target.transform.position != previousPosition)
			{
				ArrangeSections ();
				previousPosition = target.transform.position;
			}
		}	
	}
	
	
	private void ArrangeSections ()
	{
		List<GenerationSection> enableList = new List<GenerationSection> ();
		List<GenerationSection> disableList = new List<GenerationSection> ();
		for (int i = 0; i < procSections.Count; ++i)
		{
			if (Vector3.Distance (Operations.CalculateClosestTargetBounds (procSections [i], target.transform.position), target.transform.position) <= generationSoftEdge)
			{
				enableList.Add (procSections [i]);
			}
			else
			{
				disableList.Add (procSections [i]);
			}
		}
		
		ActivateSections (enableList, disableList);
	}
	
	
	void ActivateSections (List<GenerationSection> sectionsActive, List<GenerationSection> sectionsDisable)
	{
		//For every section that needs to be set to active
		//IF they are not active already, set them to active.
		//For every section that connects to them, set them to active and remove them from the list of disable
		//
		//ELSE (if the section is null) remove it from the list of sections to set active.
		for (int i = 0; i < sectionsActive.Count; ++i)
		{
			if (sectionsActive [i] != null)
			{
				if (!sectionsActive [i].gameObject.activeSelf)
				{
					sectionsActive [i].gameObject.SetActive (true);
				}
			}
			
			//This generates every section to attach onto this section.
			GenerateAttachedSections (sectionsActive [i]);
			
			for (int j = 0; j < sectionsActive[i].attachedSections.Count; ++j)
			{
				
				if (sectionsActive [i].attachedSections [j] != null)
				{
					if (sectionsDisable.Contains (sectionsActive [i].attachedSections [j]))
					{
						sectionsDisable.Remove (sectionsActive [i].attachedSections [j]);
					}
					if (!sectionsActive [i].attachedSections [j].gameObject.activeSelf)
					{
						sectionsActive [i].attachedSections [j].gameObject.SetActive (true);
					}
				}
				else
				{
					sectionsActive [i].attachedSections.Remove (sectionsActive [i].attachedSections [j]);
				}
			}
		}
		//This just checks if the section is disabled and disables it if not.
		Operations.DisableSections (sectionsDisable);
	}
	
	void GenerateAttachedSections (GenerationSection section)
	{
		for (int i = 0; i < section.AllEntries.Length; ++i)
		{
			if (section.AllEntries [i].available)
			{
				GenerateProceduralSection (section);
			}
		}
	}
	
	private int CalculateSectionToGenerate (GenerationSection prevSect)
	{
		//Sections that have nothing - get rid of
		//Sections that do not have a joining door, get rid of
		//If the number of doors is 1 then remove sections that only have 1 door.
		//Make sure that there are no overlaps so it can be added.
		//Create a new list with these sections and re-do the ratios
		
		
		
		List<GenRatios> temporaryListOfSections = new List<GenRatios> ();
		List<int> temporaryListOfIndicies = new List<int> ();
		
		if (prevSect != null)
		{
			for (int i = 0; i < sectionPrefabs.Length; ++i)
			{
				int doorInt = Operations.GetPreviousSectionIndex (prevSect, sectionPrefabs [i].section);
				
				if (doorInt == Operations.IntReturnValue)
				{
					continue;
				}
				
				if ((sectionPrefabs [i].section.AllEntries.Length >= 2 || numberOfDoors >= MinNumberOfDoors)
					&& (Operations.CheckDoorways (prevSect, sectionPrefabs [i].section))
					&& (CheckOverlap (prevSect, sectionPrefabs [i].section, doorInt)))
				{
					temporaryListOfSections.Add (sectionPrefabs [i]);
					temporaryListOfIndicies.Add (i);
				}
			}
		}
		else
		{
			for (int i = 0; i < sectionPrefabs.Length; ++i)
			{
				if (sectionPrefabs [i].section.AllEntries.Length > 0)
				{
					temporaryListOfSections.Add (sectionPrefabs [i]);
					temporaryListOfIndicies.Add (i);
				}
			}
		}
		
		temporaryListOfSections = Operations.InitialiseRatios (temporaryListOfSections);
		
		float value = Random.value;
		
		for (int index = 0; index < temporaryListOfSections.Count; ++index)
		{
			value -= temporaryListOfSections [index].ratio;
			if (value <= 0)
			{
				return temporaryListOfIndicies [index];
			}
		}
		
		return Operations.IntReturnValue;
	}
	
	private void GenerateProceduralSection (GenerationSection prevSection)
	{
		//Previous Index is now the current index (Since we are picking a new section to generate)
		previousIndex = currentIndex;
		
		//Current index picks to get a random index.
		currentIndex = GetRandomIndex (prevSection);
		
		
		if (currentIndex == Operations.IntReturnValue)
		{
			//Checks for the door that was trying to be generated on and makes it unavailable.
			for (int i = 0; i < prevSection.AllEntries.Length; ++i)
			{
				if (prevSection.AllEntries [i].available)
				{
					prevSection.AllEntries [i].available = false;
					CodeToDoWhenEncounteringAPlaceWhereNothingCanSpawn (prevSection, Operations.GetPosition (prevSection, null, i), prevSection.AllEntries [i].location);
					numberOfDoors -= 1;
					break;
				}
			}
		}
		else
		{
			//Checks if the section is the same as the previous, if it is then it adds 1 to the current repeat times.
			if (currentIndex == previousIndex)
			{
				repetitions++;
			}
			else
			{
				//Otherwise, reset the repeat count.
				repetitions = 1;
			}
			
			//Instantiates the object
			GenerationSection generatedSection = Instantiate (sectionPrefabs [currentIndex].section) as GenerationSection;
		
			generatedSection.parent = this;
		
			int doorInt = Operations.GetPreviousSectionIndex (prevSection, generatedSection);
		
			generatedSection.transform.position = Operations.GetPosition (prevSection, generatedSection, doorInt);
		
			if (prevSection != null)
			{
				int index2 = Operations.GetGeneratedSectionIndexFromPreviousSectionIndex (prevSection, generatedSection, doorInt);
			
				prevSection.AllEntries [doorInt].available = false;
				prevSection.AllEntries [doorInt].attachedSection = generatedSection;
			
				generatedSection.AllEntries [index2].available = false;
				generatedSection.AllEntries [index2].attachedSection = prevSection;
			
				generatedSection.attachedSections.Add (prevSection);
				prevSection.attachedSections.Add (generatedSection);
			}
		
			procSections.Add (generatedSection);

			generatedSection.transform.SetParent (this.transform);
		
			//The number of doors has decreased by 2 due to the opposite sides of 2 prefabs connecting.
			//The number of doors has also increased by the amount of doors the prefab has.
			numberOfDoors += generatedSection.AllEntries.Length;
			if (numberOfDoors != 0)
			{
				numberOfDoors -= 2;
			}
		}
			
	}	
	
	private int GetRandomIndex (GenerationSection previousSect)
	{
		int randomNumber = currentIndex;
		
		if (maxConsecutiveRepeats > 0)
		{
			if (repetitions >= maxConsecutiveRepeats)
			{
				int i = 0;  //Used to safely break the loop
				while (randomNumber == currentIndex)
				{
					randomNumber = CalculateSectionToGenerate (previousSect);
					i++;
					if (i > whileLoopBreakAmount)
					{
						Debug.LogError ("Change ratios or allow unlimited repeats, it iterated " + whileLoopBreakAmount + " times and still didn't get the correct number.");
						break;
					}
				}
			}
			else
			{
				//If we can still repeat this number, just pick a random number.
				randomNumber = CalculateSectionToGenerate (previousSect);
			}
		}
		else
		{
			//If we can repeat a section as many times as we want, just pick a number.
			randomNumber = CalculateSectionToGenerate (previousSect);
		}
		return randomNumber;
	}
	
	void SetPosition (GenerationSection prevSect, GenerationSection section)
	{
		if (prevSect == null)
		{
			section.transform.position = transform.position;
			return;
		}
		section.transform.position = Vector3.zero;
		
	}
	
	public bool CheckOverlap (GenerationSection previousSection, GenerationSection generatedSection, int index)
	{
		Vector3 pos = Operations.GetPosition (previousSection, generatedSection, index);
		for (int i = 0; i < procSections.Count; ++i)
		{
			if (Operations.CheckBounds (procSections [i], (pos - generatedSection.bounds.extents), (pos + generatedSection.bounds.extents)))
			{
				return false;
			}
		}
		return true;
	}
	
}