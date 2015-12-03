using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarPathfinding : MonoBehaviour
{
	//To generalise a star pathfinding, either the player should put a script on each 'node' or the player can set positions
	//Or it could be created to find the bouding boxes of every object in the scene and use that as the points. Then every duplicate 'position' it would
	//remove from the list at the very start. (or ignore from the very start).
	
	//I think I will do automatic, then use either a tag or a layer check to be able to use as nodes.
	
	//To DO:
	// 1. Make it so Nodes are automatically put in.
	// 2. Make it so there is a function called SetPath(Vector3 pos);
	// 3. Make it so it uses world position not grid position.
	// 4. Make it so it sets the starting position and the end position node position.
	// 5. Make it so it will follow the path in Update, (have both code for just moving and code for making it look where it is going).

	//Debugging Line for Nodes ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//Debug.Log("Node: " + allNodes[i] + " at Index: " + allNodes[i].gridPosition + " Parent Set to: " + node + " at grid index " + node.gridPosition + " F Cost is: " + allNodes[i].fCost);//
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//Public variables
	/// <summary>
	/// This is whether or not they wish to have diagonal movement in their game.
	/// If they have diagonal movement then it may go through walls in an arrangement like this:
	/// | |
	///    | |
	/// As pathfinding doesn't currently detect whether it has to go through a wall.
	/// </summary>
	public bool allowDiagonalMovement;
	public LayerMask pathfindingLayers;
	
	//Checks if the physics is 2D or 3D. (Just use the getcomponent on the player/object holding this script).
	//Look for rigidbody or look for collider.
	private bool physics3D;

	///<summary>
	/// This is where the open nodes are stored (Possible movement Positions)
	/// </summary>
	private List<NodeClass> openList = new List<NodeClass> ();
	private List<NodeClass> closedList = new List<NodeClass> ();
	private List<NodeClass> allNodes = new List<NodeClass> ();
	private List<NodeClass> pathTakingNodes = new List<NodeClass> ();
	private NodeClass endNode;
	private NodeClass initialNode;
	private bool findingPath;

	[System.Serializable]
	public class NodeClass
	{
		public NodeClass parent;
		public Vector2 gridPosition;
		public NodeClass targetNode;
		public int fCost;
		public int gScore;
		public int hScore;
		public NodeClass (Vector2 _gridPosition)
		{
			gridPosition = _gridPosition;
		}
		public void CalculateFCost ()
		{
			if (parent != null) {
				//G Cost (Use absolutes to get +ve Answers)
				gScore = Mathf.RoundToInt (Mathf.Abs (Mathf.Abs (gridPosition.x - parent.gridPosition.x) + Mathf.Abs (gridPosition.y - parent.gridPosition.y))) + parent.gScore;
			} else {
				//If there is no parent then it is the starting position
				//Starting positions always have a g score of zero
				gScore = 0;
			}
			//H Cost (Use absolutes to get +ve Answers)
			hScore = Mathf.RoundToInt (Mathf.Abs (Mathf.Abs (gridPosition.x - targetNode.gridPosition.x) + Mathf.Abs (gridPosition.y - targetNode.gridPosition.y)));
			//Calculate fCost by adding G cost and H cost
			fCost = Mathf.RoundToInt (gScore + hScore);
		}
	}

	void Update ()
	{
		if (findingPath) {
			findPath ();
		}
	}
	void findPath ()
	{
		findWaypoints ();
		findingPath = false;
		setRenderer ();
	}

	void Awake ()
	{
	}

	// Use this for initialization
	void Start ()
	{
//		if (worldGen != null) {
//			findTiles ();
//			if (player != null)
//				assignTargetPosition (player.movementPos);
//			initialNode = findStartPos ();
//		}
	}
	void findWaypoints ()
	{
		initialNode = findStartPos ();
		if (initialNode != null) {
			initialNode.CalculateFCost ();
			assignParents (initialNode);
			addToClosed (initialNode);
		} else {
			Debug.LogError ("Initial Node is null");
		}
		bool continueWhileLoop = true;
		while (((!closedList.Contains(endNode) && endNode != null) && continueWhileLoop) && openList.Count != 0) {
			NodeClass node = findLowestFCost ();
			if (node != null) {
				addToClosed (node);
				if (node.parent == null) {
					Debug.LogWarning ("Node " + node + " at position " + node.gridPosition + " Has no parent");
				}
				//compareGScore(node);
				assignParents (node);
			} else {
				continueWhileLoop = false;
				Debug.LogWarning ("Something went wrong, the node is null");
			}
		}
		bool continueWhileLoop2 = true;
		NodeClass prevNode = endNode;
		pathTakingNodes.Add (endNode);
		while (!pathTakingNodes.Contains(initialNode) && continueWhileLoop2) {
			if (prevNode.parent != null) {
				pathTakingNodes.Add (prevNode.parent);
				prevNode = prevNode.parent;
			} else {
				Debug.LogWarning ("Ran out of Parents before reaching beggining");
				continueWhileLoop2 = false;
			}
		} 
		if (!pathTakingNodes.Contains (initialNode) || !pathTakingNodes.Contains (endNode)) {
			Debug.LogWarning ("There was no path");
		}
	}
	
	//Fix this... the G score is supposed to check whether this node will have a lower G score with it's current parent or this node.
	void compareGScore (NodeClass nodeIn)
	{
//		for (int i = 0; i < openList.Count; ++i) {
//			if (nodeIn.gScore + (openList [i].gScore - openList [i].parent.gScore) < openList [i].parent.gScore) {
//				//Debug.Log ("Changing " + openPositions[i].gridPosition + " parent to: " + nodeIn.gridPosition + " from " + openPositions[i].parent.gridPosition);
//				//If the g score on this object is greater than the g score on the parent of any node, then change their parent to this one
//				Debug.Log (openList [i].gridPosition + " G Score Before: " + openList [i].gScore);
//				openList [i].parent = nodeIn;
//				//Calculate the f cost again
//				openList [i].CalculateFCost ();
//				Debug.Log (openList [i].gridPosition + " G Score After: " + openList [i].gScore);
//			}
//		}
	}
	
	void addToClosed (NodeClass node)
	{
		if (openList.Contains (node)) {
			openList.Remove (node);
		}
		if (!closedList.Contains (node)) {
			closedList.Add (node);
		}
	}
	void addToOpen (NodeClass node)
	{
		if (!openList.Contains (node) && !closedList.Contains (node)) {
			openList.Add (node);
		}
	}
	void addToOpenOverride (NodeClass node)
	{
		if (closedList.Contains (node)) {
			closedList.Remove (node);
		}
		if (!openList.Contains (node)) {
			openList.Add (node);
		}
	}

	//This needs to be updated so the starting position is just created as this's position.
	//The end position also needs to do the same.
	NodeClass findStartPos ()
	{
//		Vector2 startingGridPos = new Vector2 (Mathf.RoundToInt (transform.position.x / worldGen.nodeSize), Mathf.RoundToInt (transform.position.y / worldGen.nodeSize));
//		//Finds the initial starting position of the AI
//		for (int i = 0; i < allNodes.Count; ++i) {
//			if (allNodes [i].gridPosition == startingGridPos) {
//				return allNodes [i];
//			}
//		}
		return null;
	}

	
	void assignParents (NodeClass node)
	{
		for (int i = 0; i < allNodes.Count; ++i) {
			float difference = Vector2.Distance (node.gridPosition, allNodes [i].gridPosition);
			if (allowDiagonalMovement) {
				//Since the difference (due to using grid positions) can only be either a factor 1 or sqrt2, this means that
				//Flooring the value will allow for using the diagonal values. Since the Left/Right/Up/Down values will
				//be 1, and the diagonal values will be sqrt 2. 
				//After this, the Left/Right/Up/Down values will be 2. (Left twice... etc)
				//Then after that the diagonal will be a difference of sqrt8 (or 2sqrt2) (Diagonal twice (up, left, up, left)).
				difference = Mathf.Floor (difference);
			}
			if (node != allNodes [i] && !(closedList.Contains (allNodes [i])))
			if (difference <= 1) {
				if (allNodes [i].parent == null) {
					allNodes [i].parent = node;
					allNodes [i].CalculateFCost ();
				}
			}
			addToOpen (allNodes [i]);
		}
	}

	//This needs to be updated so it just finds either the edge of cubes or the player can set where they are.
	void findTiles ()
	{
//		//This finds the possible tiles that the player can walk on.
//		for (int i = 0; i < worldGen.worldSize * worldGen.nodeScale; ++i) {
//			for (int j = 0; j < worldGen.worldSize * worldGen.nodeScale; ++j) {
//				//If the player can walk on the tile then make a new tile 
//				if (worldGen.walkableTiles [i, j]) {
//					NodeClass node = new NodeClass (new Vector2 (i, j));
//					allNodes.Add (node);
//				}
//			}
//		}
	}

//This needs to be updated so it sets the position as a new node and just moves its position every time.
	public void assignTargetPosition (Vector3 movementPos)
	{
//		initialNode = findStartPos ();
//		Vector2 targetPos = new Vector2 (Mathf.RoundToInt (movementPos.x / worldGen.nodeSize), Mathf.RoundToInt (movementPos.y / worldGen.nodeSize));
//		for (int i = 0; i < allNodes.Count; ++i) {
//			if (endNode == null) {
//				endNode = allNodes [i];
//			} else {
//				if (allNodes [i].gridPosition == targetPos) {
//					endNode = allNodes [i];
//				} else if ((allNodes [i].gridPosition - targetPos).magnitude <= (endNode.gridPosition - targetPos).magnitude) {
//					endNode = allNodes [i];
//				}
//			}
//		}
//		for (int i = 0; i < allNodes.Count; ++i) {
//			allNodes [i].targetNode = endNode;
//			resetLists (allNodes [i]);
//		}
//		findingPath = true;
	}
	void resetLists (NodeClass node)
	{
		if (openList.Contains (node)) {
			openList.Remove (node);
		} 
		if (closedList.Contains (node)) {
			closedList.Remove (node);
		}
		if (pathTakingNodes.Contains (node)) {
			pathTakingNodes.Remove (node);
		}
		node.parent = null;
	}

	NodeClass findLowestFCost ()
	{
		NodeClass node = null;
		for (int i = 0; i < openList.Count; ++i) {
			if (node == null) {
				node = openList [i];
			} else if (openList [i].fCost < node.fCost) {
				node = openList [i];
			}
		}
		return node;
	}

	//Needs to be updated to use world position.
	public void setRenderer ()
	{
		LineRenderer lineRenderer = GetComponent<LineRenderer> ();
		if (lineRenderer != null) {
			lineRenderer.SetVertexCount (pathTakingNodes.Count);
			for (int i = 0; i < pathTakingNodes.Count; ++i) {
				//lineRenderer.SetPosition (i, pathTakingNodes [i].gridPosition * worldGen.nodeSize);
			}
		}
	}

}
