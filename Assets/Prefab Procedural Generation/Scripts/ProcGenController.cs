using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProcGenController : MonoBehaviour
{
	//A list containing all of the level section variations.
	public List<ProcGenRatios> listOfSections;
	private List<ProcGenRatios> sectionsToAdd;

	//If true, generates sections in order, else, randomises outputs.
	public bool generatesInOrder;

	//The number of times that a single section can be spawned in a row.
	public int maxConsecutiveSectionRepeats;
	private int currentRepeatNum;

	//public bool isPooled;

	//The sections that will be added during runtime after certain conditions have been met.
	//public List<GameObject> sectionsToAddDuringRuntime;
	//No longer needed as we are going to move the add during runtime values to the proc gen section.

	//If the cleanup of the sections is from the center of the screen, or relative to an object's location.
	public bool hasTarget;
	public GameObject targetGameObject;
	public float cleanupDistance;

	//The prefab that will spawn at the start of the generation
	public ProcGenSection startingSection;

	//How many sections will be spawned before finishing, and the final section generated.
	public bool hasEndOfGeneration;
	public int numSectionsToSpawn;
	public ProcGenSection finalSection;

	//The current and previously spawned sections
	private int currentSectionIndex;
	private int previousSectionIndex;
	
	//The current distance and the current amount of spawned sections
	private float currentDistance;
	private int currentTotalSectionsGenerated;

	/// <summary>
	/// Used to set up arrays and perform checks before procedural generation occurs
	/// </summary>
	private void InitialiseGenerator ()
	{
		//Checks if there are any sections within the list, if not, it logs an error.
		if (listOfSections.Count <= 0) {
			Debug.LogError ("No sections in the Procedural Generation section-list!");
		}

		//Checks if hasTarget is true, if so, checks if targetGameObject is null. If it is, it resets hasTarget to false.
		if (hasTarget && (targetGameObject == null)) {
			hasTarget = false;
			Debug.LogWarning ("hasTarget is true; however, Procedural Generation's target object is null. Setting hasTarget to false.");
		}

		if (hasEndOfGeneration) {
			if (numSectionsToSpawn <= 0) {
				hasEndOfGeneration = false;
				Debug.LogWarning ("hasEndOfGeneration is true; however, Procedural Generation's numSectionsToSpawn <= 0. Setting hasEndOfGeneration to false.");
			} else {
				if (finalSection == null) {
					Debug.LogWarning ("No finalSection specified, dynamically allocating. (unless random, then first section chosen)");
					finalSection = listOfSections [CalculateFinalSection ()].section;
				}
			}
		}
		
		InitialiseSections ();
	}
	
	/// <summary>
	/// This changes all of the ratios so they are a percentage (out of 1)
	/// This should run every time a new section is added to the list/removed from sectionsToAdd list.
	/// </summary>
	private void InitialiseRatios ()
	{
	
		//We could do this by using a 'Total' variable that just gets reset and it does this calculation
		//in the calculate section to spawn script.
	
		float total = 0;
		for (int i = 0; i < listOfSections.Count; ++i) {
			if (!sectionsToAdd.Contains (listOfSections [i])) {
				total += listOfSections [i].ratio;
			}
		}
		for (int i = 0; i < listOfSections.Count; ++i) {
			if (!sectionsToAdd.Contains (listOfSections [i])) {
				listOfSections [i].ratio = (listOfSections [i].ratio / total);
			}
		}
		
	}
	
	/// <summary>
	/// Returns the index of the final section if one was not selected.
	/// </summary>
	private int CalculateFinalSection ()
	{
		if (generatesInOrder) {
			return numSectionsToSpawn % listOfSections.Count;
		} else {
			return 0;
		}
	}
	
	/// <summary>
	/// Calculates the section to generate using the ratios.
	/// </summary>
	/// <returns>The section (int) to generate.</returns>
	private int CalculateSectionToGenerate ()
	{
		float value = Random.value;
		int i = 0;
		while (value >= 0) {
			value -= listOfSections [i].ratio;
			i++;
			if (i > listOfSections.Count) {
				Debug.LogError ("Ran out of sections before finding the right section to generate");
				return 0;
			}
		}
		//Now i is equal to the value of the section we want.
		//If the i value is over the repeated amount then repick (Maybe just add 1 to i and % Count)? else return i;
		
		if (listOfSections.Count >= i) {
			Debug.LogError ("The picked random number is larger than the size of List Of Sections");
		}
		return i;
	}
	
	/// <summary>
	/// This adds the sections that need to be added to the sectionsToAdd list to their list.
	/// This should only run once.
	/// </summary>
	private void InitialiseSections ()
	{
		for (int i = 0; i < listOfSections.Count; ++i) {
			if (listOfSections [i].section.distanceToAddition > 0) {
				sectionsToAdd.Add (listOfSections [i]);
			}
		}
	}
	
	/// <summary>
	/// Adds the sections when they are required to be added to the list.
	/// This should run every time a section generates or the object moves in the appropriate direction
	/// </summary>
	private void AddSections ()
	{
		for (int i = 0; i < sectionsToAdd.Count; ++i) {
			if (sectionsToAdd [i].section.delayedBySectionsNotDistance) {
				if (sectionsToAdd [i].section.distanceToAddition <= currentTotalSectionsGenerated) {
					sectionsToAdd.Remove (sectionsToAdd [i]);
					InitialiseRatios ();
				}
			} else {
				if (sectionsToAdd [i].section.distanceToAddition <= currentDistance) {
					sectionsToAdd.Remove (sectionsToAdd [i]);
					InitialiseRatios ();
				}
			}
		}
	}
}
