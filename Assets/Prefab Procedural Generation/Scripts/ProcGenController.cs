using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProcGenController : MonoBehaviour
{
	public static event System.Action<float> OnCurrentDistanceChange;
	public static event System.Action<int> OnTotalIndexChange;
	
	//Direction that we want the random generation to spawn in.
	public Vector3 direction = Vector3.up;
	
	//A list containing all of the level section variations.
	public List<ProcGenRatios> listOfSections = new List<ProcGenRatios> ();
	private List<ProcGenRatios> sectionsToAdd = new List<ProcGenRatios> ();
	private List<ProcGenRatios> removedSections = new List<ProcGenRatios> ();

	//If true, generates sections in order, else, randomises outputs.
	public bool generatesInOrder;
	
	public float generationSoftEdge;

	//The number of times that a single section can be spawned in a row.
	public int maxConsecutiveSectionRepeats;
	private int currentRepeatNum = 1;

	//public bool isPooled;

	//If the cleanup of the sections is from the center of the screen, or relative to an object's location.
	public bool hasTarget;
	public GameObject targetGameObject;
	public float cleanupDistance = 1;

	//The prefab that will spawn at the start of the generation
	public ProcGenSection startingSection;

	//How many sections will be spawned before finishing, and the final section generated.
	public bool hasEndOfGeneration;
	public int numSectionsToSpawn;
	public ProcGenSection finalSection;

	//The current and previously spawned sections
	private int currentSectionIndex;
	private int previousSectionIndex;
	private ProcGenSection previousSection;
	
	//The current distance and the current amount of spawned sections
	public float currentDistance { get; private set; }
	public int currentTotalSectionsGenerated { get; private set; }

	int whileLoopBreakAmount = 1000;

	void Awake ()
	{
		InitialiseGenerator ();
	}
	
	void OnEnable ()
	{
		ProcGenController.OnCurrentDistanceChange += HandleOnCurrentDistanceChange;
		ProcGenController.OnTotalIndexChange += HandleOnTotalIndexChange;
	}
	
	void Start ()
	{
		SpawnStartingSection ();
	}

	
	void Update ()
	{
	
        

		//Recording of distance
		if (hasTarget) {
			Vector3 pos = (transform.position + targetGameObject.transform.position);
			float dist = MathsSharp.VectorAdditive.Vector3Multiplication (pos, direction).magnitude;
			if (dist > currentDistance) {
				currentDistance = dist;
				OnCurrentDistanceChange (currentDistance);
			}
		}
		
		//Generation
		if (currentTotalSectionsGenerated <= numSectionsToSpawn || numSectionsToSpawn == 0) {
			//Do spawning stuff in here.
			if (hasTarget) {
				if (previousSection != null) {
					float distOfTopOfGeneratedSection = ((previousSection.transform.position + (direction * previousSection.sizeOfSection / 2)).magnitude * direction).magnitude;
					if (currentDistance > (distOfTopOfGeneratedSection - generationSoftEdge)) {
						GenerateSection ();
					}
				} else {
					Debug.LogError ("Change the Cleanup Distance, it is destroying before the next generation happens");
					Abort ();
				}
			}
		}
	}
	
	void OnDisable ()
	{
		ProcGenController.OnCurrentDistanceChange -= HandleOnCurrentDistanceChange;
		ProcGenController.OnTotalIndexChange -= HandleOnTotalIndexChange;
	}
	
	
	/// <summary>
	/// Spawns the starting section.
	/// </summary>
	void SpawnStartingSection ()
	{
		if (startingSection == null) {
			GenerateSection ();
			Debug.Log ("Starting section was null, so it Generated a section instead.");
			return;
		}
		ProcGenSection generatedSection = Instantiate (startingSection) as ProcGenSection;
		
		generatedSection.transform.position = transform.position +
			((generatedSection.sizeOfSection / 2) * direction);
	
		previousSection = generatedSection;
		
		currentTotalSectionsGenerated++;
		OnTotalIndexChange (currentTotalSectionsGenerated);
		
		if (!hasTarget) {
			Vector3 pos = generatedSection.transform.position + (generatedSection.sizeOfSection / 2 * direction);
			currentDistance = MathsSharp.VectorAdditive.Vector3Multiplication (pos, direction).magnitude;
			OnCurrentDistanceChange (currentDistance);
		}
		generatedSection.SetSpawnObject (this);
		
	}
	
	/// <summary>
	/// Used to set up arrays and perform checks before procedural generation occurs
	/// </summary>
	private void InitialiseGenerator ()
	{
        if (generationSoftEdge >= cleanupDistance){
            Debug.LogWarning("Having a generation soft edge that is greater than the cleanup may cause undesired results.");
        }

		direction.Normalize ();
		if (direction == Vector3.zero) {
			Debug.LogError ("Cannot generate in no direction, please change direction to a suitable value");
			Abort ();
			return;
		}
		//Checks if there are any sections within the list, if not, it logs an error.
		if (listOfSections.Count == 0) {
			Debug.LogError ("No sections in the Procedural Generation section-list!");
			Abort ();
			return;
		}

		//Checks if hasTarget is true, if so, checks if targetGameObject is null. If it is, it resets hasTarget to false.
		if (hasTarget && (targetGameObject == null)) {
			hasTarget = false;
			Debug.LogWarning ("hasTarget is true; however, Procedural Generation's target object is null. Setting hasTarget to false.");
		}

        // If NumSections is above 0, change end generation bool to true
        if (numSectionsToSpawn > 0)
        {
            hasEndOfGeneration = true;
        }

        if (hasEndOfGeneration) {
			if (numSectionsToSpawn <= 0) {
				hasEndOfGeneration = false;
				Debug.LogWarning ("hasEndOfGeneration is true; however, Procedural Generation's numSectionsToSpawn <= 0. Setting hasEndOfGeneration to false.");
			} else {
				if (finalSection == null) {
					Debug.LogWarning ("No finalSection specified, dynamically allocating. (unless random, then random section chosen)");
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
			if (!sectionsToAdd.Contains (listOfSections [i]) && !removedSections.Contains (listOfSections [i])) {
				total += listOfSections [i].ratio;
			}
		}
		for (int i = 0; i < listOfSections.Count; ++i) {
			if (!sectionsToAdd.Contains (listOfSections [i]) && !removedSections.Contains (listOfSections [i])) {
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
			return Random.Range (0, listOfSections.Count);
		}
	}
	
	/// <summary>
	/// Calculates the section to generate using the ratios.
	/// </summary>
	/// <returns>The section (int) to generate.</returns>
	private int CalculateSectionToGenerate ()
	{
		float value = Random.value; //Always between 0 and 1
		int i = 0;
		bool whileStillChecking = true;
		while (whileStillChecking) {
			value -= listOfSections [i].ratio;
			if (value <= 0) {
				whileStillChecking = false;
			} else {
				i++;
			}
			if (i > listOfSections.Count) {
				Debug.LogError ("Ran out of sections before finding the right section to generate");
				whileStillChecking = false;
			}
		}
		
		if (i >= listOfSections.Count) {
			Debug.LogWarning ("The picked random number is larger than the size of List Of Sections"
				+ "\n" + "Modulating the random number for listOfSections.Count");
			i = i % listOfSections.Count;
		}
		return i;
	}
	
	private void GenerateSection ()
	{
		previousSectionIndex = currentSectionIndex;
		int randomNumber = currentSectionIndex;
		if (!generatesInOrder) {
			if (maxConsecutiveSectionRepeats > 0) {
				if (currentRepeatNum >= maxConsecutiveSectionRepeats) {
					int i = 0;  //Used to safely break the loop
					while (randomNumber == currentSectionIndex) {
						randomNumber = CalculateSectionToGenerate ();
						i++;
						if (i > whileLoopBreakAmount) {
							Debug.LogError ("Change ratios or allow unlimited repeats, it iterated 1000 times and still didn't get the correct number.");
							break;
						}
					}
				} else {
					//If we can still repeat this number, just pick a random number.
					randomNumber = CalculateSectionToGenerate ();
				}
			} else {
				//If we can repeat a section as many times as we want, just pick a number.
				randomNumber = CalculateSectionToGenerate ();
			}
			
			currentSectionIndex = randomNumber;
		} else {
			//If we don't pick randomly, then the next section is the next section.
			currentSectionIndex = (currentSectionIndex + 1) % listOfSections.Count;
		}
		//Checks if the section is the same as the previous, if it is then it adds 1 to the current repeat times.
		if (currentSectionIndex == previousSectionIndex) {
			currentRepeatNum++;
		} else {
			//Otherwise, reset the repeat count.
			currentRepeatNum = 1;
		}
		//Instantiates the object
		
		ProcGenSection generatedSection;
		//if(!isPooled)
		{
			if (currentTotalSectionsGenerated == numSectionsToSpawn) {
				generatedSection = Instantiate (finalSection) as ProcGenSection;
			
			} else {
				generatedSection = Instantiate (listOfSections [currentSectionIndex].section) as ProcGenSection;
			}
		}
		
		if (previousSection != null) {
			//Since transform.position is the midpoint, we want the sizeOfSection / 2 * direction
			generatedSection.transform.position = previousSection.transform.position +
				(((previousSection.sizeOfSection / 2) + (generatedSection.sizeOfSection / 2)) * direction);
		} else {
			generatedSection.transform.position = transform.position +
				((generatedSection.sizeOfSection / 2) * direction);
			Debug.LogWarning ("Previous seciton was null, if this was the first generation ignore this.");
		}
		previousSection = generatedSection;
		
		currentTotalSectionsGenerated++;
		OnTotalIndexChange (currentTotalSectionsGenerated);
		
		if (!hasTarget) {
			Vector3 pos = generatedSection.transform.position + (generatedSection.sizeOfSection / 2 * direction);
			currentDistance = MathsSharp.VectorAdditive.Vector3Multiplication (pos, direction).magnitude;
			OnCurrentDistanceChange (currentDistance);
		}
		generatedSection.SetSpawnObject (this);
		
	}
	
	/// <summary>
	/// This method should be called whenever the distance changes as multiple
	/// scripts need to called when the distance changes.
	/// </summary>
	private void OnDistanceChange ()
	{
		AddSections ();
		RemoveSections ();
	}
	
	/// <summary>
	/// This adds the sections that need to be added to the sectionsToAdd list to their list.
	/// This should only run once.
	/// </summary>
	private void InitialiseSections ()
	{
		for (int i = 0; i < listOfSections.Count; ++i) 
        {
            listOfSections[i].section.InitialiseProcGenSection();

            if (listOfSections[i].section.distanceUntilAdditionToList > 0)
            {
                sectionsToAdd.Add(listOfSections[i]);
            }
		}
		InitialiseRatios ();
	}
	
	/// <summary>
	/// Adds the sections when they are required to be added to the list.
	/// This should run every time a section generates or the object moves in the appropriate direction
	/// </summary>
	private void AddSections ()
	{
		for (int i = 0; i < sectionsToAdd.Count; ++i) {
			if (sectionsToAdd [i].section.delayedBySectionsNotDistance) {
				if (sectionsToAdd [i].section.distanceUntilAdditionToList <= currentTotalSectionsGenerated) {
					sectionsToAdd.Remove (sectionsToAdd [i]);
					InitialiseRatios ();
				}
			} else {
				if (sectionsToAdd [i].section.distanceUntilAdditionToList <= currentDistance) {
					sectionsToAdd.Remove (sectionsToAdd [i]);
					InitialiseRatios ();
				}
			}
		}
	}
	
	/// <summary>
	/// Removes the sections when they are required to be removed from the list.
	/// This should run every time a section generates or the object moves in the appropriate direction
	/// </summary>
	private void RemoveSections ()
	{
		for (int i = 0; i < listOfSections.Count; ++i) {
			//If it doesn't get removed, continue.
			if (listOfSections [i].section.distanceUntilRemoveFromList <= 0) {
				continue;
			}
			
			if (listOfSections [i].section.delayedBySectionsNotDistance) {
				if (listOfSections [i].section.distanceUntilRemoveFromList <= currentTotalSectionsGenerated) {
					removedSections.Add (listOfSections [i]);
					InitialiseRatios ();
				}
			} else {
				if (listOfSections [i].section.distanceUntilRemoveFromList <= currentDistance) {
					removedSections.Add (listOfSections [i]);
					InitialiseRatios ();
				}
			}
		}
	}
	
	void HandleOnTotalIndexChange (int obj)
	{
		OnDistanceChange ();
	}
	
	void HandleOnCurrentDistanceChange (float obj)
	{
        OnDistanceChange();
	}
	
	
	void Abort ()
	{
#if UNITY_EDITOR
		Debug.LogError ("Exiting the Editor due to an error");
		UnityEditor.EditorApplication.isPlaying = false;
		
		UnityEditor.EditorApplication.isPaused = true;
#else
		Application.Quit();
#endif
	}
}
