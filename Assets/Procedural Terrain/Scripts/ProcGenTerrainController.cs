using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProcGenTerrainController : MonoBehaviour
{
	
	//TODO:
	//Figure out if I need height as an input for Depth...
	//Do the Mesh() generation
	//Do the Terrain generation
	
	//Needs to track a target to be able to continue generating 'chunks'
	public bool hasTarget;
	public GameObject target;
	public float SoftEdge;
	
	//'Noise values'
	public float xScale;
	public float xSinCurveDegrees;
	
	public float yScale;
	public float ySinCurveDegrees;
	
	//Change in y Values;
	public float minHeightDifference;
	public float maxHeightDifference;
	
	//Min/Max Values.
	public float heightLimit;
	public float depthLimit;
	
	//Ground Values (How low will the floor go from the height value).
	public float floorDepth;
	public bool randomiseFloorDepth;
	public float minimumFloorDepth;
	public float maximumFloorDepth;
	
	//Heightmap Intervals
	public int xIntervals;
	public int zIntervals;
	
	//GameObject of level
	public FlooringType typeOfFlooring;
	
	
	//If Multiple Objects
	public GameObject flooringObject;
	private Bounds sizeOfFlooringObject;
	private int totalSpawned;
	
	//////////////////////
	// If TerrainObject //
	// If Single Mesh ////
	// Do Later //////////
	//////////////////////
	
	public enum FlooringType
	{
		SingleMesh,
		MultipleObjects,
		TerrainObject
	}
	
	public class Chunk
	{
		public float[,] heights;
		public float[,] depths;
		public List<GameObject> chunkObjects = new List<GameObject> ();
		public Bounds bounds;
		public bool generate = true;
		public Chunk[] connectingChunks = new Chunk[4];
		public GameObject parent = new GameObject ();
		
		public Chunk ()
		{
			heights = new float[0, 0];
			depths = new float[0, 0];
		}
		
		//For when using a gameobject for flooring
		public Chunk (float xInterval, float zInterval, Bounds boundsOfFloor)
		{
			heights = new float[Mathf.CeilToInt (xInterval / boundsOfFloor.size.x), Mathf.CeilToInt (zInterval / boundsOfFloor.size.z)];
			depths = new float[Mathf.CeilToInt (xInterval / boundsOfFloor.size.x), Mathf.CeilToInt (zInterval / boundsOfFloor.size.z)];
		}
		
		//For when not using a floor gameobject
		public Chunk (float xInterval, float yInterval)
		{
			heights = new float[Mathf.CeilToInt (xInterval), Mathf.CeilToInt (yInterval)];
			depths = new float[Mathf.CeilToInt (xInterval), Mathf.CeilToInt (yInterval)];
		}
		
		
		public void AddGameObjectToChunk (GameObject gameObj)
		{
			chunkObjects.Add (gameObj);
			bounds.Encapsulate (FindBounds (gameObj.transform, gameObj.GetComponent<Renderer> (), bounds));
		}
		
		public void CheckForConnectingChunks (List<Chunk> chunks, float maxX, float maxZ)
		{
			for (int i = 0; i < chunks.Count; ++i) {
				if (chunks [i] == this) {
					continue;
				}
				Vector3 dist = chunks [i].bounds.center - this.bounds.center;
				dist.y = 0;
				float distance = Mathf.Floor (dist.magnitude);
				dist = new Vector3 (Mathf.Floor (dist.x), Mathf.Floor (dist.y), Mathf.Floor (dist.z));
				Vector3 direction = (dist).normalized;
				if (direction == Vector3.forward && distance <= maxZ) {
					ConnectChunk (chunks [i], 2);
				} else if (direction == Vector3.left && distance <= maxX) {
					ConnectChunk (chunks [i], 3);
				} else if (direction == Vector3.back && distance <= maxZ) {
					ConnectChunk (chunks [i], 0);
				} else if (direction == Vector3.right && distance <= maxX) {
					ConnectChunk (chunks [i], 1);
				}
			}
		}
		
		public void ConnectChunk (Chunk chunk, int side)
		{
			if (chunk == null) {
				return;
			}
			
			if (chunk.connectingChunks [side] == null) {
				chunk.connectingChunks [side] = chunk;
			}
			if (this.connectingChunks [((side + 2) % 4)] == null) {
				this.connectingChunks [((side + 2) % 4)] = chunk;
			}
			
			CheckGenerate (this);
			CheckGenerate (chunk);	
			
		}
		
		public static void CheckGenerate (Chunk chunk)
		{
			chunk.generate = false;
			for (int i = 0; i < chunk.connectingChunks.Length; ++i) {
				if (chunk.connectingChunks [i] == null) {
					chunk.generate = true;
				}
			}
		}
		
		public void CheckDefaultChunkSides ()
		{
			if (this.connectingChunks [0] != null) {
				//If it is above this object
			
				//(0,max) = its (0,0) and (max,max) = its (max,0)
				
				this.heights [0, (this.heights.GetLength (1) - 1)] = this.connectingChunks [0].heights [0, 0];
				this.heights [(this.heights.GetLength (0) - 1), (this.heights.GetLength (1) - 1)] = this.connectingChunks [0].heights [(this.connectingChunks [0].heights.GetLength (0) - 1), 0];
			} 
			if (this.connectingChunks [1] != null) {
				//If it is left of this object
			
				// (0,0) = its (max,0) and (0,max) = its (max,max)
			
				this.heights [0, 0] = this.connectingChunks [1].heights [(this.heights.GetLength (0) - 1), 0];
				this.heights [0, (this.heights.GetLength (1) - 1)] = this.connectingChunks [1].heights [(this.heights.GetLength (0) - 1), (this.connectingChunks [1].heights.GetLength (1) - 1)];
			} 
			if (this.connectingChunks [2] != null) {
				//If it is below this object
			
				//(0,0) = its (0,max) and (max, 0) = its (max,max)
			
				this.heights [0, 0] = this.connectingChunks [2].heights [0, (this.heights.GetLength (1) - 1)];
				this.heights [(this.heights.GetLength (0) - 1), 0] = this.connectingChunks [2].heights [(this.connectingChunks [2].heights.GetLength (0) - 1), (this.heights.GetLength (1) - 1)];
			}
			if (this.connectingChunks [3] != null) {
				//If it is right of this object
			
				//(0,max) = its (0,0) and (max,max) = its (0,max)
				
				this.heights [0, (this.heights.GetLength (1) - 1)] = this.connectingChunks [3].heights [0, 0];
				this.heights [(this.heights.GetLength (0) - 1), (this.heights.GetLength (1) - 1)] = this.connectingChunks [3].heights [0, (this.connectingChunks [3].heights.GetLength (1) - 1)];
			}
		}
		
		
	}
	
	
	private List<Chunk> Chunks = new List<Chunk> ();
	
	private Vector3 targetPosition;
	
	void Awake ()
	{
		SetUpFloorObject ();
		if (target != null) {
			hasTarget = true;
		} else {
			hasTarget = false;
		}
		
	}
	
	// Use this for initialization
	void Start ()
	{
		GenerateChunk (null, 0);
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		if (hasTarget) {
			if (target.transform.position != targetPosition) {
				ArrangeSections ();
				targetPosition = target.transform.position;
			}
		}	
	}
	
	void ArrangeSections ()
	{
		List<Chunk> enableList = new List<Chunk> ();
		List<Chunk> disableList = new List<Chunk> ();
		for (int i = 0; i < Chunks.Count; ++i) {
			if (Vector3.Distance (Chunks [i].bounds.ClosestPoint (target.transform.position), target.transform.position) <= SoftEdge) {
				enableList.Add (Chunks [i]);
			} else {
				disableList.Add (Chunks [i]);
			}
		}
		
		ActivateSections (enableList, disableList);
	}
	
	void ActivateSections (List<Chunk> sectionsActive, List<Chunk> sectionsDisable)
	{
		for (int i = 0; i < sectionsActive.Count; ++i) {
			if (sectionsActive [i] != null) {
				if (!sectionsActive [i].parent.activeSelf) {
					sectionsActive [i].parent.SetActive (true);
				}
			}
			
		}
		{
			bool gen = false;
			int i = 0;
			while (!gen && i < sectionsActive.Count) {
				for (int j = 0; j < sectionsActive[i].connectingChunks.Length; ++j) {
					//This generates every section to attach onto this section.
					if (sectionsActive [i].generate) {
						if (sectionsActive [i].connectingChunks [j] == null) {
							Vector3 pos = GetPosition (sectionsActive [i], j);
							if (Vector3.Distance (pos, target.transform.position) <= (SoftEdge + Mathf.Sqrt (Mathf.Pow (xIntervals, 2) + Mathf.Pow (zIntervals, 2)))) {
								GenerateChunk (sectionsActive [i], j);
								gen = true;
								break;
							}
						}
					}
				}
				i++;
			}
		}
		//This just checks if the section is disabled and disables it if not.
		for (int k = 0; k < sectionsDisable.Count; ++k) {
			if (sectionsDisable [k].parent.activeSelf) {
				sectionsDisable [k].parent.SetActive (false);
			}
		}
		
	}
		
	void GenerateChunk (Chunk previousChunk, int side)
	{
	
		Vector3 position = GetPosition (previousChunk, side);
		
		Chunk ChunkToSpawn = new Chunk (xIntervals, zIntervals, sizeOfFlooringObject);
		totalSpawned++;
		ChunkToSpawn.bounds.center = position;
		ChunkToSpawn.parent.transform.position = position;
		ChunkToSpawn.parent.name = "Chunk " + totalSpawned.ToString ();
		Chunks.Add (ChunkToSpawn);
		if (previousChunk != null) {
			ChunkToSpawn.ConnectChunk (previousChunk, side);
		}
		ChunkToSpawn.CheckForConnectingChunks (Chunks, xIntervals, zIntervals);
		SetDefaultChunkValues (previousChunk, ChunkToSpawn, side);
		ChunkToSpawn.CheckDefaultChunkSides ();
		
		{
			GetHeightMap (ChunkToSpawn);
		}
		{
			for (int i = 0; i < ChunkToSpawn.depths.GetLength(0); ++i) {
				for (int j = 0; j < ChunkToSpawn.depths.GetLength(1); ++j) {
					ChunkToSpawn.depths [i, j] = GetDepth (ChunkToSpawn.heights [i, j]);
				}
			}
		}
		
		switch (typeOfFlooring) {
		case(FlooringType.MultipleObjects):
			GenerateObjectChunk (ChunkToSpawn);
			break;
		case(FlooringType.SingleMesh):
			GenerateMeshChunk (ChunkToSpawn);
			break;
		case(FlooringType.TerrainObject):
			break;
		default:
			break;
		}
		
		
	}

	void GenerateObjectChunk (Chunk ChunkToSpawn)
	{
		{
			for (int i = 0; i < ChunkToSpawn.heights.GetLength(0); ++i) {
				for (int j = 0; j < ChunkToSpawn.heights.GetLength(1); ++j) {
					if (ChunkToSpawn.depths [i, j] > 0) {
						GameObject chunkObj = Instantiate (flooringObject) as GameObject;
						Vector3 pos = GetLocalPositionOfFlooring (i, j, ChunkToSpawn.heights [i, j]);
						pos.x += ChunkToSpawn.parent.transform.position.x - (xIntervals / 2);
						pos.z += ChunkToSpawn.parent.transform.position.z - (zIntervals / 2);
						chunkObj.transform.position = pos;
						ChunkToSpawn.AddGameObjectToChunk (chunkObj);
					}
				}
			}
		}
		{
			for (int i = 0; i < ChunkToSpawn.depths.GetLength(0); ++i) {
				for (int j = 0; j < ChunkToSpawn.depths.GetLength(1); ++j) {
					if (ChunkToSpawn.depths [i, j] > 0) {
						for (int k = 0; k < Mathf.CeilToInt(ChunkToSpawn.depths [i, j] / sizeOfFlooringObject.size.y); ++k) {
							GameObject chunkObj = Instantiate (flooringObject) as GameObject;
							Vector3 pos = GetLocalPositionOfFlooring (i, j, (ChunkToSpawn.heights [i, j] - (sizeOfFlooringObject.size.y * (k + 1))));
							pos.x += ChunkToSpawn.parent.transform.position.x - (xIntervals / 2);
							pos.z += ChunkToSpawn.parent.transform.position.z - (zIntervals / 2);
							chunkObj.transform.position = pos;
							ChunkToSpawn.AddGameObjectToChunk (chunkObj);
						}
					}
				}
			}
		}
		{
			for (int i = 0; i < ChunkToSpawn.chunkObjects.Count; ++i) {
				ChunkToSpawn.chunkObjects [i].transform.SetParent (ChunkToSpawn.parent.transform);
			}
		}
	}

	void GenerateMeshChunk (Chunk chunk)
	{
		
	}

	void SetDefaultChunkValues (Chunk prevChunk, Chunk chunk, int side)
	{
		float changeVal1 = Random.Range (minHeightDifference, (maxHeightDifference + 1));
		float changeVal2 = Random.Range (minHeightDifference, (maxHeightDifference + 1));
		
		if (Random.value <= .5f)
			changeVal1 = -changeVal1;
		if (Random.value <= .5f)
			changeVal2 = -changeVal2;
		
		
		if (prevChunk == null) {
			float changeVal3 = Random.Range (minHeightDifference, (maxHeightDifference + 1));
			
			if (Random.value <= .5f)
				changeVal3 = -changeVal3;
			
			chunk.heights [0, 0] = Random.Range (minHeightDifference, (maxHeightDifference + 1));
			chunk.heights [0, (chunk.heights.GetLength (1) - 1)] = chunk.heights [0, 0] + changeVal1;
			chunk.heights [(chunk.heights.GetLength (0) - 1), 0] = chunk.heights [0, 0] + changeVal2;
			chunk.heights [(chunk.heights.GetLength (0) - 1), (chunk.heights.GetLength (1) - 1)] = ((chunk.heights [0, (chunk.heights.GetLength (1) - 1)] + chunk.heights [(chunk.heights.GetLength (0) - 1), 0]) / 2) + changeVal3;
		} else {
			switch (side) {
			case(0):
				chunk.heights [0, 0] = prevChunk.heights [0, (prevChunk.heights.GetLength (1) - 1)];
				chunk.heights [(chunk.heights.GetLength (0) - 1), 0] = prevChunk.heights [(prevChunk.heights.GetLength (0) - 1), (prevChunk.heights.GetLength (1) - 1)];
				
				chunk.heights [0, (chunk.heights.GetLength (1) - 1)] = chunk.heights [0, 0] + changeVal1;
				chunk.heights [(chunk.heights.GetLength (0) - 1), (chunk.heights.GetLength (1) - 1)] = ((chunk.heights [0, (chunk.heights.GetLength (1) - 1)] + chunk.heights [(chunk.heights.GetLength (0) - 1), 0]) / 2) + changeVal2;
				break;
			case(1):
				chunk.heights [(chunk.heights.GetLength (0) - 1), 0] = prevChunk.heights [0, 0];
				chunk.heights [(chunk.heights.GetLength (0) - 1), (chunk.heights.GetLength (1) - 1)] = prevChunk.heights [0, (prevChunk.heights.GetLength (1) - 1)];
				
				chunk.heights [0, (chunk.heights.GetLength (1) - 1)] = chunk.heights [(chunk.heights.GetLength (0) - 1), (chunk.heights.GetLength (1) - 1)] + changeVal1;
				chunk.heights [0, 0] = ((chunk.heights [0, (chunk.heights.GetLength (1) - 1)] + chunk.heights [(chunk.heights.GetLength (0) - 1), 0]) / 2) + changeVal2;
				break;
			case(2):
				chunk.heights [0, (chunk.heights.GetLength (1) - 1)] = prevChunk.heights [0, 0];
				chunk.heights [(chunk.heights.GetLength (0) - 1), (chunk.heights.GetLength (1) - 1)] = prevChunk.heights [(prevChunk.heights.GetLength (0) - 1), 0];
				
				chunk.heights [(chunk.heights.GetLength (0) - 1), 0] = chunk.heights [(chunk.heights.GetLength (0) - 1), (chunk.heights.GetLength (1) - 1)] + changeVal1;
				chunk.heights [0, 0] = ((chunk.heights [0, (chunk.heights.GetLength (1) - 1)] + chunk.heights [(chunk.heights.GetLength (0) - 1), 0]) / 2) + changeVal2;
				break;
			case(3):
				chunk.heights [0, 0] = prevChunk.heights [(prevChunk.heights.GetLength (0) - 1), 0];
				chunk.heights [0, (chunk.heights.GetLength (1) - 1)] = prevChunk.heights [(prevChunk.heights.GetLength (0) - 1), (prevChunk.heights.GetLength (1) - 1)];
				
				chunk.heights [0, (chunk.heights.GetLength (1) - 1)] = chunk.heights [0, 0] + changeVal1;
				chunk.heights [(chunk.heights.GetLength (0) - 1), (chunk.heights.GetLength (1) - 1)] = ((chunk.heights [0, (chunk.heights.GetLength (1) - 1)] + chunk.heights [(chunk.heights.GetLength (0) - 1), 0]) / 2) + changeVal2;
				break;
			default:
				break;
			}
		}
	}

	
	Vector3 GetPosition (Chunk chunk, int side)
	{
		//Get the position for the next chunk to spawn.
		if (Chunks.Count == 0 || chunk == null) {
			return transform.position;
		}
		Vector3 pos = chunk.bounds.center;
		switch (side) {
		case(0):
			pos.z += zIntervals;
			break;
		case(1):
			pos.x -= xIntervals;
			break;
		case(2):
			pos.z -= zIntervals;
			break;
		case(3):
			pos.x += xIntervals;
			break;
		default:
			break;
		}
		
		return pos;
	}
	
	float GetHeight (Chunk chunk, int iIndex, int jIndex)
	{
		//If it is a corner, just return what it was.
		if ((iIndex == 0 && jIndex == 0) 
			|| (iIndex == 0 && jIndex == (chunk.heights.GetLength (1) - 1)) 
			|| (iIndex == (chunk.heights.GetLength (0) - 1) && jIndex == 0) 
			|| (iIndex == (chunk.heights.GetLength (0) - 1) && jIndex == (chunk.heights.GetLength (1) - 1))) {
			return chunk.heights [iIndex, jIndex];
		}
		
	
		float value = 0;
		
		if (jIndex == 0) {
			
			//value = (0,0) + (iIndex / length of i-1) * ((end, 0) - (0,0))
			
			
			value = chunk.heights [0, 0] 
				+ ((float)iIndex / (float)(chunk.heights.GetLength (0) - 1)) 
				* (chunk.heights [(chunk.heights.GetLength (0) - 1), 0] - chunk.heights [0, 0]);
				
		} else if (jIndex == (chunk.heights.GetLength (0) - 1)) {
			
			//value = (0,end) + (iIndex / lenght of i-1) * ((end,end) - (0,end))
			
			value = chunk.heights [0, chunk.heights.GetLength (1) - 1] 
				+ ((float)iIndex / ((float)chunk.heights.GetLength (0) - 1)) 
				* (chunk.heights [(chunk.heights.GetLength (0) - 1), chunk.heights.GetLength (1) - 1] - chunk.heights [0, chunk.heights.GetLength (1) - 1]);
			
		
		} else if (iIndex == 0) {
			
			//value = (0,0) + (jIndex / length-1) * ((0, end) - (0,0))
			
			value = chunk.heights [0, 0];
			value += ((float)jIndex / (float)(chunk.heights.GetLength (1) - 1))
				* (chunk.heights [0, (chunk.heights.GetLength (1) - 1)] - chunk.heights [0, 0]);
				
		} else if (iIndex == (chunk.heights.GetLength (1) - 1)) {
		
			//value = (end,0) + (jIndex / length-1) * ((end, end) - (end,0))
			
			value = chunk.heights [chunk.heights.GetLength (0) - 1, 0] 
				+ ((float)(jIndex / (float)(chunk.heights.GetLength (1) - 1)) 
				* (chunk.heights [chunk.heights.GetLength (0) - 1, (chunk.heights.GetLength (1) - 1)] - chunk.heights [chunk.heights.GetLength (0) - 1, 0]));
		
		} else {
			
			//value = (0,jIndex) + (iIndex / length-1) * ((end,jIndex) - (start,jIndex)) + noise
			
			value = chunk.heights [0, jIndex] 
				+ ((float)(iIndex / (float)(chunk.heights.GetLength (0) - 1)) 
				* (chunk.heights [(chunk.heights.GetLength (0) - 1), jIndex] - chunk.heights [0, jIndex]));
		}
		value += (xScale * Random.value * Mathf.Sin (Mathf.Deg2Rad * (iIndex * xSinCurveDegrees)) 
			+ yScale * Random.value * Mathf.Sin (Mathf.Deg2Rad * (jIndex * ySinCurveDegrees)));
		
		Mathf.Clamp (value, depthLimit, heightLimit);
		
		return value;
	}	
	
	//Get the depth of the flooring. (If 0 or less, it won't generate floor there)
	//return Random.Range(min-1, max+1) or set value.
	float GetDepth (float height)
	{
		if (randomiseFloorDepth) {
			return Random.Range ((minimumFloorDepth - 1), (maximumFloorDepth + 1));
		} else {
			return floorDepth;
		}
	}
	
	Vector3 GetLocalPositionOfFlooring (int x, int y, float height)
	{
		return new Vector3 (x * sizeOfFlooringObject.size.x, height, y * sizeOfFlooringObject.size.z);
	}
	
	void SetUpFloorObject ()
	{
		switch (typeOfFlooring) {
		case(FlooringType.SingleMesh):
			break;
		case(FlooringType.TerrainObject):
			break;
		case(FlooringType.MultipleObjects):
			GameObject floorObj = Instantiate (flooringObject) as GameObject;
			sizeOfFlooringObject = FindBounds (floorObj.transform, floorObj.GetComponent<Renderer> (), sizeOfFlooringObject);
			Destroy (floorObj);
			break;
		default:
			break;
		}
	}
	
	
	public static Bounds FindBounds (Transform t, Renderer r, Bounds bound)
	{
	
		if (t == null) {
			return bound;
		}

		bound.Encapsulate (t.position);
		
		for (int i = 0; i < t.childCount; ++i) {
			bound.Encapsulate (FindBounds (t.GetChild (i).transform, t.GetChild (i).GetComponent<Renderer> (), bound));
		}
		
		if (r == null) {
			return bound;
		}
		
		bound.Encapsulate (r.bounds);
		
		return bound;
	}

	void GetHeightMap (Chunk chunk)
	{
		float value = Mathf.Max (chunk.heights.GetLength (0), chunk.heights.GetLength (1));
		for (int x = 0; x < (value); ++x) {
			{
				for (int i = 0; i < (chunk.heights.GetLength(1)); ++i) {
					chunk.heights [(0 + x), i] = GetHeight (chunk, (0 + x), i);
				}
			}
			{
				for (int i = 0; i < (chunk.heights.GetLength(1)); ++i) {
					chunk.heights [((chunk.heights.GetLength (0) - 1) - x), i] = GetHeight (chunk, ((chunk.heights.GetLength (0) - 1) - x), i);
				}
			}
			{
				for (int i = 0; i < (chunk.heights.GetLength(0)); ++i) {
					chunk.heights [i, (0 + x)] = GetHeight (chunk, i, (0 + x));
				}
			}
			{
				for (int i = 0; i < (chunk.heights.GetLength(0)); ++i) {
					chunk.heights [i, ((chunk.heights.GetLength (1) - 1) - x)] = GetHeight (chunk, i, ((chunk.heights.GetLength (0) - 1) - x));
				}
			}
		}
	}
	
}
