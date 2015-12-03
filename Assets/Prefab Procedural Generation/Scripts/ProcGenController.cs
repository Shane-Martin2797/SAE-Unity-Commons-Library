using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProcGenController : MonoBehaviour 
{
    //A list containing all of the level section variations.
    public List<GameObject/*ProcGenSection*/> listOfSections;

    //If true, generates sections in order, else, randomises outputs.
    public bool generatesInOrder;

    //The number of times that a single section can be spawned in a row.
    public int maxConsecutiveSectionRepeats;
    private int currentRepeatNum;

    //public bool isPooled;

    //The sections that will be added during runtime after certain conditions have been met.
    public List<GameObject> sectionsToAddDuringRuntime;

    //If the cleanup of the sections is from the center of the screen, or relative to an object's location.
    public bool hasTarget;
    public GameObject targetGameObject;
    public float cleanupDistance;

    //The prefab that will spawn at the start of the generation
    public GameObject /*ProcGenSection*/ startingSection;

    //How many sections will be spawned before finishing, and the final section generated.
    public bool hasEndOfGeneration;
    public int numSectionsToSpawn;
    public GameObject /*ProcGenSection*/ finalSection;

    //The current and previously spawned sections
    private int currentSectionIndex;
    private int previousSectionIndex;

    /// <summary>
    /// Used to set up arrays and perform checks before procedural generation occurs
    /// </summary>
    private void InitialiseGenerator ()
    {
        //Checks if there are any sections within the list, if not, it logs an error.
        if (listOfSections.Count <= 0)
        {
            Debug.LogError("No sections in the Procedural Generation section-list!");
        }

        //Checks if hasTarget is true, if so, checks if targetGameObject is null. If it is, it resets hasTarget to false.
        if (hasTarget && (targetGameObject == null))
        {
            hasTarget = false;
            Debug.LogWarning("hasTarget is true; however, Procedural Generation's target object is null. Setting hasTarget to false.");
        }

        if (hasEndOfGeneration)
        {
            if (numSectionsToSpawn <= 0)
            {
                hasEndOfGeneration = false;
                Debug.LogWarning("hasEndOfGeneration is true; however, Procedural Generation's numSectionsToSpawn <= 0. Setting hasEndOfGeneration to false.");
            }
            else
            {
                if (finalSection == null)
                {
                    Debug.LogWarning("No finalSection specified, dynamically allocating. (unless random, then first section chosen)");
                    finalSection = listOfSections[CalculateFinalSection()];
                }
            }
        }
    }
	
    /// <summary>
    /// Returns the index of the final section if one was not selected.
    /// </summary>
	private int CalculateFinalSection () 
    {
        if (generatesInOrder)
        {
            return numSectionsToSpawn % listOfSections.Count;
        }
        else
        {
            return 0;
        }
	}
}
