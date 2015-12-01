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



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
